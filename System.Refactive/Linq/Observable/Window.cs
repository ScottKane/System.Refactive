// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Concurrency;
using System.Refactive.Disposables;
using System.Refactive.Internal;
using System.Refactive.Subjects;
using System.Runtime.CompilerServices;

namespace System.Refactive.Linq
{
    internal static class Window<TSource>
    {
        internal sealed class Count : Producer<IRefObservable<TSource>, Count._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly int _count;
            private readonly int _skip;

            public Count(IRefObservable<TSource> source, int count, int skip)
            {
                _source = source;
                _count = count;
                _skip = skip;
            }

            protected override _ CreateSink(IRefObserver<IRefObservable<TSource>> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<TSource, IRefObservable<TSource>>
            {
                private readonly Queue<ISubject<TSource>> _queue = new Queue<ISubject<TSource>>();
                private readonly SingleAssignmentDisposable _m = new SingleAssignmentDisposable();
                private readonly RefCountDisposable _refCountDisposable;

                private readonly int _count;
                private readonly int _skip;

                public _(Count parent, IRefObserver<IRefObservable<TSource>> observer)
                    : base(observer)
                {
                    _refCountDisposable = new RefCountDisposable(_m);

                    _count = parent._count;
                    _skip = parent._skip;
                }

                private int _n;

                public override void Run(IRefObservable<TSource> source)
                {
                    var firstWindow = CreateWindow();
                    ForwardOnNext(ref firstWindow);

                    _m.Disposable = source.SubscribeSafe(this);

                    SetUpstream(_refCountDisposable);
                }

                private IRefObservable<TSource> CreateWindow()
                {
                    var s = new Subject<TSource>();
                    _queue.Enqueue(s);
                    return new WindowObservable<TSource>(s, _refCountDisposable);
                }

                public override void OnNext(ref TSource value)
                {
                    foreach (var s in _queue)
                    {
                        s.OnNext(ref value);
                    }

                    var c = _n - _count + 1;
                    if (c >= 0 && c % _skip == 0)
                    {
                        var s = _queue.Dequeue();
                        s.OnCompleted();
                    }

                    _n++;
                    if (_n % _skip == 0)
                    {
                        var newWindow = CreateWindow();
                        ForwardOnNext(ref newWindow);
                    }
                }

                public override void OnError(Exception error)
                {
                    while (_queue.Count > 0)
                    {
                        _queue.Dequeue().OnError(error);
                    }

                    ForwardOnError(error);
                }

                public override void OnCompleted()
                {
                    while (_queue.Count > 0)
                    {
                        _queue.Dequeue().OnCompleted();
                    }

                    ForwardOnCompleted();
                }
            }
        }

        internal sealed class TimeSliding : Producer<IRefObservable<TSource>, TimeSliding._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly TimeSpan _timeSpan;
            private readonly TimeSpan _timeShift;
            private readonly IScheduler _scheduler;

            public TimeSliding(IRefObservable<TSource> source, TimeSpan timeSpan, TimeSpan timeShift, IScheduler scheduler)
            {
                _source = source;
                _timeSpan = timeSpan;
                _timeShift = timeShift;
                _scheduler = scheduler;
            }

            protected override _ CreateSink(IRefObserver<IRefObservable<TSource>> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(this);

            internal sealed class _ : Sink<TSource, IRefObservable<TSource>>
            {
                private readonly object _gate = new object();
                private readonly Queue<ISubject<TSource>> _q = new Queue<ISubject<TSource>>();
                private readonly SerialDisposable _timerD = new SerialDisposable();

                private readonly IScheduler _scheduler;
                private readonly TimeSpan _timeShift;

                public _(TimeSliding parent, IRefObserver<IRefObservable<TSource>> observer)
                    : base(observer)
                {
                    _scheduler = parent._scheduler;
                    _timeShift = parent._timeShift;
                }

                private RefCountDisposable? _refCountDisposable;
                private TimeSpan _totalTime;
                private TimeSpan _nextShift;
                private TimeSpan _nextSpan;

                public void Run(TimeSliding parent)
                {
                    _totalTime = TimeSpan.Zero;
                    _nextShift = parent._timeShift;
                    _nextSpan = parent._timeSpan;

                    var groupDisposable = new CompositeDisposable(2) { _timerD };
                    _refCountDisposable = new RefCountDisposable(groupDisposable);

                    CreateWindow();
                    CreateTimer();

                    groupDisposable.Add(parent._source.SubscribeSafe(this));

                    SetUpstream(_refCountDisposable);
                }

                private void CreateWindow()
                {
                    var s = new Subject<TSource>();
                    _q.Enqueue(s);
                    ForwardOnNext(ref Unsafe.AsRef<IRefObservable<TSource>>(new WindowObservable<TSource>(s, _refCountDisposable!))); // NB: _refCountDisposable gets assigned in Run.
                }

                private void CreateTimer()
                {
                    var m = new SingleAssignmentDisposable();
                    _timerD.Disposable = m;

                    var isSpan = false;
                    var isShift = false;
                    if (_nextSpan == _nextShift)
                    {
                        isSpan = true;
                        isShift = true;
                    }
                    else if (_nextSpan < _nextShift)
                    {
                        isSpan = true;
                    }
                    else
                    {
                        isShift = true;
                    }

                    var newTotalTime = isSpan ? _nextSpan : _nextShift;
                    var ts = newTotalTime - _totalTime;
                    _totalTime = newTotalTime;

                    if (isSpan)
                    {
                        _nextSpan += _timeShift;
                    }

                    if (isShift)
                    {
                        _nextShift += _timeShift;
                    }

                    m.Disposable = _scheduler.ScheduleAction((@this: this, isSpan, isShift), ts, static tuple => tuple.@this.Tick(tuple.isSpan, tuple.isShift));
                }

                private void Tick(bool isSpan, bool isShift)
                {
                    lock (_gate)
                    {
                        //
                        // BREAKING CHANGE v2 > v1.x - Making behavior of sending OnCompleted to the window
                        //                             before sending out a new window consistent across all
                        //                             overloads of Window and Buffer. Before v2, the two
                        //                             operations below were reversed.
                        //
                        if (isSpan)
                        {
                            var s = _q.Dequeue();
                            s.OnCompleted();
                        }

                        if (isShift)
                        {
                            CreateWindow();
                        }
                    }

                    CreateTimer();
                }

                public override void OnNext(ref TSource value)
                {
                    lock (_gate)
                    {
                        foreach (var s in _q)
                        {
                            s.OnNext(ref value);
                        }
                    }
                }

                public override void OnError(Exception error)
                {
                    lock (_gate)
                    {
                        foreach (var s in _q)
                        {
                            s.OnError(error);
                        }

                        ForwardOnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    lock (_gate)
                    {
                        foreach (var s in _q)
                        {
                            s.OnCompleted();
                        }

                        ForwardOnCompleted();
                    }
                }
            }
        }

        internal sealed class TimeHopping : Producer<IRefObservable<TSource>, TimeHopping._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly TimeSpan _timeSpan;
            private readonly IScheduler _scheduler;

            public TimeHopping(IRefObservable<TSource> source, TimeSpan timeSpan, IScheduler scheduler)
            {
                _source = source;
                _timeSpan = timeSpan;
                _scheduler = scheduler;
            }

            protected override _ CreateSink(IRefObserver<IRefObservable<TSource>> observer) => new _(observer);

            protected override void Run(_ sink) => sink.Run(this);

            internal sealed class _ : Sink<TSource, IRefObservable<TSource>>
            {
                private readonly object _gate = new object();
                private Subject<TSource> _subject;

                public _(IRefObserver<IRefObservable<TSource>> observer)
                    : base(observer)
                {
                    _subject = new Subject<TSource>();
                }

                private RefCountDisposable? _refCountDisposable;

                public void Run(TimeHopping parent)
                {
                    var groupDisposable = new CompositeDisposable(2);
                    _refCountDisposable = new RefCountDisposable(groupDisposable);

                    NextWindow();

                    groupDisposable.Add(parent._scheduler.SchedulePeriodic(this, parent._timeSpan, static @this => @this.Tick()));
                    groupDisposable.Add(parent._source.SubscribeSafe(this));

                    SetUpstream(_refCountDisposable);
                }

                private void Tick()
                {
                    lock (_gate)
                    {
                        _subject.OnCompleted();

                        _subject = new Subject<TSource>();
                        NextWindow();
                    }
                }

                private void NextWindow()
                {
                    ForwardOnNext(ref Unsafe.AsRef<IRefObservable<TSource>>(new WindowObservable<TSource>(_subject, _refCountDisposable!))); // NB: _refCountDisposable gets assigned in Run.
                }

                public override void OnNext(ref TSource value)
                {
                    lock (_gate)
                    {
                        _subject.OnNext(ref value);
                    }
                }

                public override void OnError(Exception error)
                {
                    lock (_gate)
                    {
                        _subject.OnError(error);

                        ForwardOnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    lock (_gate)
                    {
                        _subject.OnCompleted();

                        ForwardOnCompleted();
                    }
                }
            }
        }

        internal sealed class Ferry : Producer<IRefObservable<TSource>, Ferry._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly int _count;
            private readonly TimeSpan _timeSpan;
            private readonly IScheduler _scheduler;

            public Ferry(IRefObservable<TSource> source, TimeSpan timeSpan, int count, IScheduler scheduler)
            {
                _source = source;
                _timeSpan = timeSpan;
                _count = count;
                _scheduler = scheduler;
            }

            protected override _ CreateSink(IRefObserver<IRefObservable<TSource>> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<TSource, IRefObservable<TSource>>
            {
                private readonly object _gate = new object();
                private readonly SerialDisposable _timerD = new SerialDisposable();

                private readonly int _count;
                private readonly TimeSpan _timeSpan;
                private readonly IScheduler _scheduler;

                private Subject<TSource> _s;

                public _(Ferry parent, IRefObserver<IRefObservable<TSource>> observer)
                    : base(observer)
                {
                    _count = parent._count;
                    _timeSpan = parent._timeSpan;
                    _scheduler = parent._scheduler;

                    _s = new Subject<TSource>();
                }

                private int _n;

                private RefCountDisposable? _refCountDisposable;

                public override void Run(IRefObservable<TSource> source)
                {
                    var groupDisposable = new CompositeDisposable(2) { _timerD };
                    _refCountDisposable = new RefCountDisposable(groupDisposable);

                    NextWindow();
                    CreateTimer(_s);

                    groupDisposable.Add(source.SubscribeSafe(this));

                    SetUpstream(_refCountDisposable);
                }

                private void CreateTimer(Subject<TSource> window)
                {
                    var m = new SingleAssignmentDisposable();
                    _timerD.Disposable = m;

                    m.Disposable = _scheduler.ScheduleAction((@this: this, window), _timeSpan, static tuple => tuple.@this.Tick(tuple.window));
                }

                private void NextWindow()
                {
                    ForwardOnNext(ref Unsafe.AsRef<IRefObservable<TSource>>(new WindowObservable<TSource>(_s, _refCountDisposable!))); // NB: _refCountDisposable gets assigned in Run.
                }

                private void Tick(Subject<TSource> window)
                {
                    Subject<TSource> newWindow;

                    lock (_gate)
                    {
                        if (window != _s)
                        {
                            return;
                        }

                        _n = 0;
                        newWindow = new Subject<TSource>();

                        _s.OnCompleted();
                        _s = newWindow;
                        NextWindow();
                    }

                    CreateTimer(newWindow);
                }

                public override void OnNext(ref TSource value)
                {
                    Subject<TSource>? newWindow = null;

                    lock (_gate)
                    {
                        _s.OnNext(ref value);

                        _n++;
                        if (_n == _count)
                        {
                            _n = 0;
                            newWindow = new Subject<TSource>();

                            _s.OnCompleted();
                            _s = newWindow;
                            NextWindow();
                        }
                    }

                    if (newWindow != null)
                    {
                        CreateTimer(newWindow);
                    }
                }

                public override void OnError(Exception error)
                {
                    lock (_gate)
                    {
                        _s.OnError(error);
                        ForwardOnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    lock (_gate)
                    {
                        _s.OnCompleted();
                        ForwardOnCompleted();
                    }
                }
            }
        }
    }

    internal static class Window<TSource, TWindowClosing>
    {
        internal sealed class Selector : Producer<IRefObservable<TSource>, Selector._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly Func<IRefObservable<TWindowClosing>> _windowClosingSelector;

            public Selector(IRefObservable<TSource> source, Func<IRefObservable<TWindowClosing>> windowClosingSelector)
            {
                _source = source;
                _windowClosingSelector = windowClosingSelector;
            }

            protected override _ CreateSink(IRefObserver<IRefObservable<TSource>> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<TSource, IRefObservable<TSource>>
            {
                private readonly object _gate = new object();
                private readonly AsyncLock _windowGate = new AsyncLock();
                private readonly SerialDisposable _m = new SerialDisposable();
                private readonly Func<IRefObservable<TWindowClosing>> _windowClosingSelector;

                private Subject<TSource> _window;

                public _(Selector parent, IRefObserver<IRefObservable<TSource>> observer)
                    : base(observer)
                {
                    _windowClosingSelector = parent._windowClosingSelector;

                    _window = new Subject<TSource>();
                }

                private RefCountDisposable? _refCountDisposable;

                public override void Run(IRefObservable<TSource> source)
                {
                    var groupDisposable = new CompositeDisposable(2) { _m };
                    _refCountDisposable = new RefCountDisposable(groupDisposable);

                    NextWindow();

                    groupDisposable.Add(source.SubscribeSafe(this));

                    _windowGate.Wait(this, static @this => @this.CreateWindowClose());

                    SetUpstream(_refCountDisposable);
                }

                private void NextWindow()
                {
                    ForwardOnNext(ref Unsafe.AsRef<IRefObservable<TSource>>(new WindowObservable<TSource>(_window, _refCountDisposable!)));
                }

                private void CreateWindowClose()
                {
                    IRefObservable<TWindowClosing> windowClose;
                    try
                    {
                        windowClose = _windowClosingSelector();
                    }
                    catch (Exception exception)
                    {
                        lock (_gate)
                        {
                            ForwardOnError(exception);
                        }
                        return;
                    }

                    var observer = new WindowClosingObserver(this);
                    _m.Disposable = observer;
                    observer.SetResource(windowClose.SubscribeSafe(observer));
                }

                private void CloseWindow(IDisposable closingSubscription)
                {
                    closingSubscription.Dispose();

                    lock (_gate)
                    {
                        _window.OnCompleted();
                        _window = new Subject<TSource>();

                        NextWindow();
                    }

                    _windowGate.Wait(this, static @this => @this.CreateWindowClose());
                }

                private sealed class WindowClosingObserver : SafeObserver<TWindowClosing>
                {
                    private readonly _ _parent;

                    public WindowClosingObserver(_ parent)
                    {
                        _parent = parent;
                    }

                    public override void OnNext(ref TWindowClosing value)
                    {
                        _parent.CloseWindow(this);
                    }

                    public override void OnError(Exception error)
                    {
                        _parent.OnError(error);
                    }

                    public override void OnCompleted()
                    {
                        _parent.CloseWindow(this);
                    }
                }

                public override void OnNext(ref TSource value)
                {
                    lock (_gate)
                    {
                        _window.OnNext(ref value);
                    }
                }

                public override void OnError(Exception error)
                {
                    lock (_gate)
                    {
                        _window.OnError(error);
                        ForwardOnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    lock (_gate)
                    {
                        _window.OnCompleted();
                        ForwardOnCompleted();
                    }
                }
            }
        }

        internal sealed class Boundaries : Producer<IRefObservable<TSource>, Boundaries._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly IRefObservable<TWindowClosing> _windowBoundaries;

            public Boundaries(IRefObservable<TSource> source, IRefObservable<TWindowClosing> windowBoundaries)
            {
                _source = source;
                _windowBoundaries = windowBoundaries;
            }

            protected override _ CreateSink(IRefObserver<IRefObservable<TSource>> observer) => new _(observer);

            protected override void Run(_ sink) => sink.Run(this);

            internal sealed class _ : Sink<TSource, IRefObservable<TSource>>
            {
                private readonly object _gate = new object();

                private Subject<TSource> _window;

                public _(IRefObserver<IRefObservable<TSource>> observer)
                    : base(observer)
                {
                    _window = new Subject<TSource>();
                }

                private RefCountDisposable? _refCountDisposable;

                public void Run(Boundaries parent)
                {
                    var d = new CompositeDisposable(2);
                    _refCountDisposable = new RefCountDisposable(d);

                    NextWindow();

                    d.Add(parent._source.SubscribeSafe(this));
                    d.Add(parent._windowBoundaries.SubscribeSafe(new WindowClosingObserver(this)));

                    SetUpstream(_refCountDisposable);
                }

                private void NextWindow()
                {
                    ForwardOnNext(ref Unsafe.AsRef<IRefObservable<TSource>>(new WindowObservable<TSource>(_window, _refCountDisposable!)));
                }

                private sealed class WindowClosingObserver : IRefObserver<TWindowClosing>
                {
                    private readonly _ _parent;

                    public WindowClosingObserver(_ parent)
                    {
                        _parent = parent;
                    }

                    public void OnNext(ref TWindowClosing value)
                    {
                        lock (_parent._gate)
                        {
                            _parent._window.OnCompleted();
                            _parent._window = new Subject<TSource>();

                            _parent.NextWindow();
                        }
                    }

                    public void OnError(Exception error)
                    {
                        _parent.OnError(error);
                    }

                    public void OnCompleted()
                    {
                        _parent.OnCompleted();
                    }
                }

                public override void OnNext(ref TSource value)
                {
                    lock (_gate)
                    {
                        _window.OnNext(ref value);
                    }
                }

                public override void OnError(Exception error)
                {
                    lock (_gate)
                    {
                        _window.OnError(error);
                        ForwardOnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    lock (_gate)
                    {
                        _window.OnCompleted();
                        ForwardOnCompleted();
                    }
                }
            }
        }
    }

    internal sealed class WindowObservable<TSource> : AddRef<TSource>
    {
        public WindowObservable(IRefObservable<TSource> source, RefCountDisposable refCount)
            : base(source, refCount)
        {
        }
    }
}
