﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Concurrency;
using System.Refactive.Disposables;
using System.Refactive.Internal;
using System.Runtime.CompilerServices;

namespace System.Refactive.Linq
{
    internal static class Buffer<TSource>
    {
        internal sealed class CountExact : Producer<IList<TSource>, CountExact.ExactSink>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly int _count;

            public CountExact(IRefObservable<TSource> source, int count)
            {
                _source = source;
                _count = count;
            }

            protected override ExactSink CreateSink(IRefObserver<IList<TSource>> observer) => new ExactSink(observer, _count);

            protected override void Run(ExactSink sink) => sink.Run(_source);

            internal sealed class ExactSink : Sink<TSource, IList<TSource>>
            {
                private readonly int _count;
                private int _index;
                private IList<TSource>? _buffer;

                internal ExactSink(IRefObserver<IList<TSource>> observer, int count) : base(observer)
                {
                    _count = count;
                }

                public override void OnNext(ref TSource value)
                {
                    var buffer = _buffer;
                    if (buffer == null)
                    {
                        buffer = new List<TSource>();
                        _buffer = buffer;
                    }

                    buffer.Add(value);

                    var idx = _index + 1;
                    if (idx == _count)
                    {
                        _buffer = null;
                        _index = 0;
                        ForwardOnNext(ref buffer);
                    }
                    else
                    {
                        _index = idx;
                    }
                }

                public override void OnError(Exception error)
                {
                    _buffer = null;
                    ForwardOnError(error);
                }

                public override void OnCompleted()
                {
                    var buffer = _buffer;
                    _buffer = null;

                    if (buffer != null)
                    {
                        ForwardOnNext(ref buffer);
                    }
                    ForwardOnCompleted();
                }
            }
        }

        internal sealed class CountSkip : Producer<IList<TSource>, CountSkip.SkipSink>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly int _count;
            private readonly int _skip;

            public CountSkip(IRefObservable<TSource> source, int count, int skip)
            {
                _source = source;
                _count = count;
                _skip = skip;
            }

            protected override SkipSink CreateSink(IRefObserver<IList<TSource>> observer) => new SkipSink(observer, _count, _skip);

            protected override void Run(SkipSink sink) => sink.Run(_source);

            internal sealed class SkipSink : Sink<TSource, IList<TSource>>
            {
                private readonly int _count;
                private readonly int _skip;
                private int _index;
                private IList<TSource>? _buffer;

                internal SkipSink(IRefObserver<IList<TSource>> observer, int count, int skip) : base(observer)
                {
                    _count = count;
                    _skip = skip;
                }

                public override void OnNext(ref TSource value)
                {
                    var idx = _index;
                    var buffer = _buffer;
                    if (idx == 0)
                    {
                        buffer = new List<TSource>();
                        _buffer = buffer;
                    }

                    buffer?.Add(value);

                    if (++idx == _count)
                    {
                        _buffer = null;
                        ForwardOnNext(ref buffer!); // NB: Counting logic with _index ensures non-null.
                    }

                    if (idx == _skip)
                    {
                        _index = 0;
                    }
                    else
                    {
                        _index = idx;
                    }
                }

                public override void OnError(Exception error)
                {
                    _buffer = null;
                    ForwardOnError(error);
                }

                public override void OnCompleted()
                {
                    var buffer = _buffer;
                    _buffer = null;

                    if (buffer != null)
                    {
                        ForwardOnNext(ref buffer);
                    }
                    ForwardOnCompleted();
                }
            }
        }

        internal sealed class CountOverlap : Producer<IList<TSource>, CountOverlap.OverlapSink>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly int _count;
            private readonly int _skip;

            public CountOverlap(IRefObservable<TSource> source, int count, int skip)
            {
                _source = source;
                _count = count;
                _skip = skip;
            }

            protected override OverlapSink CreateSink(IRefObserver<IList<TSource>> observer) => new OverlapSink(observer, _count, _skip);

            protected override void Run(OverlapSink sink) => sink.Run(_source);

            internal sealed class OverlapSink : Sink<TSource, IList<TSource>>
            {
                private readonly Queue<IList<TSource>> _queue;
                private readonly int _count;
                private readonly int _skip;
                private int _n;

                public OverlapSink(IRefObserver<IList<TSource>> observer, int count, int skip)
                    : base(observer)
                {
                    _queue = new Queue<IList<TSource>>();
                    _count = count;
                    _skip = skip;
                    CreateWindow();
                }

                private void CreateWindow()
                {
                    var s = new List<TSource>();
                    _queue.Enqueue(s);
                }

                public override void OnNext(ref TSource value)
                {
                    foreach (var s in _queue)
                    {
                        s.Add(value);
                    }

                    var c = _n - _count + 1;
                    if (c >= 0 && c % _skip == 0)
                    {
                        var s = _queue.Dequeue();
                        if (s.Count > 0)
                        {
                            ForwardOnNext(ref s);
                        }
                    }

                    _n++;
                    if (_n % _skip == 0)
                    {
                        CreateWindow();
                    }
                }

                public override void OnError(Exception error)
                {
                    // just drop the ILists on the GC floor, no reason to clear them
                    _queue.Clear();

                    ForwardOnError(error);
                }

                public override void OnCompleted()
                {
                    while (_queue.Count > 0)
                    {
                        var s = _queue.Dequeue();
                        if (s.Count > 0)
                        {
                            ForwardOnNext(ref s);
                        }
                    }

                    ForwardOnCompleted();
                }
            }
        }

        internal sealed class TimeSliding : Producer<IList<TSource>, TimeSliding._>
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

            protected override _ CreateSink(IRefObserver<IList<TSource>> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(this);

            internal sealed class _ : Sink<TSource, IList<TSource>>
            {
                private readonly TimeSpan _timeShift;
                private readonly IScheduler _scheduler;
                private readonly object _gate = new object();
                private readonly Queue<List<TSource>> _q = new Queue<List<TSource>>();
                private SerialDisposableValue _timerSerial;

                public _(TimeSliding parent, IRefObserver<IList<TSource>> observer)
                    : base(observer)
                {
                    _timeShift = parent._timeShift;
                    _scheduler = parent._scheduler;
                }

                private TimeSpan _totalTime;
                private TimeSpan _nextShift;
                private TimeSpan _nextSpan;

                public void Run(TimeSliding parent)
                {
                    _totalTime = TimeSpan.Zero;
                    _nextShift = parent._timeShift;
                    _nextSpan = parent._timeSpan;

                    CreateWindow();
                    CreateTimer();

                    Run(parent._source);
                }

                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        _timerSerial.Dispose();
                    }
                    base.Dispose(disposing);
                }

                private void CreateWindow()
                {
                    var s = new List<TSource>();
                    _q.Enqueue(s);
                }

                private void CreateTimer()
                {
                    var m = new SingleAssignmentDisposable();

                    _timerSerial.Disposable = m;

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
                        // Before v2, the two operations below were reversed. This doesn't have an observable
                        // difference for Buffer, but is done to keep code consistent with Window, where we
                        // took a breaking change in v2 to ensure consistency across overloads. For more info,
                        // see the comment in Tick for Window.
                        //
                        if (isSpan)
                        {
                            if (_q.Count > 0)
                            {
                                ForwardOnNext(ref Unsafe.AsRef<IList<TSource>>(_q.Dequeue()));
                            }
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
                            s.Add(value);
                        }
                    }
                }

                public override void OnError(Exception error)
                {
                    lock (_gate)
                    {
                        while (_q.Count > 0)
                        {
                            _q.Dequeue().Clear();
                        }

                        ForwardOnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    lock (_gate)
                    {
                        while (_q.Count > 0)
                        {
                            ForwardOnNext(ref Unsafe.AsRef<IList<TSource>>(_q.Dequeue()));
                        }

                        ForwardOnCompleted();
                    }
                }
            }
        }

        internal sealed class TimeHopping : Producer<IList<TSource>, TimeHopping._>
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

            protected override _ CreateSink(IRefObserver<IList<TSource>> observer) => new _(observer);

            protected override void Run(_ sink) => sink.Run(this);

            internal sealed class _ : Sink<TSource, IList<TSource>>
            {
                private readonly object _gate = new object();
                private List<TSource> _list = new List<TSource>();

                public _(IRefObserver<IList<TSource>> observer)
                    : base(observer)
                {
                }

                private SingleAssignmentDisposableValue _periodicDisposable;

                public void Run(TimeHopping parent)
                {
                    _periodicDisposable.Disposable = parent._scheduler.SchedulePeriodic(this, parent._timeSpan, static @this => @this.Tick());
                    Run(parent._source);
                }

                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        _periodicDisposable.Dispose();
                    }
                    base.Dispose(disposing);
                }

                private void Tick()
                {
                    lock (_gate)
                    {
                        ForwardOnNext(ref Unsafe.AsRef<IList<TSource>>(_list));
                        _list = new List<TSource>();
                    }
                }

                public override void OnNext(ref TSource value)
                {
                    lock (_gate)
                    {
                        _list.Add(value);
                    }
                }

                public override void OnError(Exception error)
                {
                    lock (_gate)
                    {
                        _list.Clear();

                        ForwardOnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    lock (_gate)
                    {
                        ForwardOnNext(ref Unsafe.AsRef<IList<TSource>>(_list));
                        ForwardOnCompleted();
                    }
                }
            }
        }

        internal sealed class Ferry : Producer<IList<TSource>, Ferry._>
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

            protected override _ CreateSink(IRefObserver<IList<TSource>> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run();

            internal sealed class _ : Sink<TSource, IList<TSource>>
            {
                private readonly Ferry _parent;
                private readonly object _gate = new object();
                private List<TSource> _s = new List<TSource>();

                public _(Ferry parent, IRefObserver<IList<TSource>> observer)
                    : base(observer)
                {
                    _parent = parent;
                }

                private SerialDisposableValue _timerSerial;
                private int _n;
                private int _windowId;

                public void Run()
                {
                    _n = 0;
                    _windowId = 0;

                    CreateTimer(0);

                    SetUpstream(_parent._source.SubscribeSafe(this));
                }

                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        _timerSerial.Dispose();
                    }

                    base.Dispose(disposing);
                }

                private void CreateTimer(int id)
                {
                    var m = new SingleAssignmentDisposable();
                    _timerSerial.Disposable = m;

                    m.Disposable = _parent._scheduler.ScheduleAction((@this: this, id), _parent._timeSpan, static tuple => tuple.@this.Tick(tuple.id));
                }

                private void Tick(int id)
                {
                    lock (_gate)
                    {
                        if (id != _windowId)
                        {
                            return;
                        }

                        _n = 0;
                        var newId = ++_windowId;

                        var res = _s;
                        _s = new List<TSource>();
                        ForwardOnNext(ref Unsafe.AsRef<IList<TSource>>(res));

                        CreateTimer(newId);
                    }
                }

                public override void OnNext(ref TSource value)
                {
                    var newWindow = false;
                    var newId = 0;

                    lock (_gate)
                    {
                        _s.Add(value);

                        _n++;
                        if (_n == _parent._count)
                        {
                            newWindow = true;
                            _n = 0;
                            newId = ++_windowId;

                            var res = _s;
                            _s = new List<TSource>();
                            ForwardOnNext(ref Unsafe.AsRef<IList<TSource>>(res));
                        }

                        if (newWindow)
                        {
                            CreateTimer(newId);
                        }
                    }
                }

                public override void OnError(Exception error)
                {
                    lock (_gate)
                    {
                        _s.Clear();
                        ForwardOnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    lock (_gate)
                    {
                        ForwardOnNext(ref Unsafe.AsRef<IList<TSource>>(_s));
                        ForwardOnCompleted();
                    }
                }
            }
        }
    }

    internal static class Buffer<TSource, TBufferClosing>
    {
        internal sealed class Selector : Producer<IList<TSource>, Selector._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly Func<IRefObservable<TBufferClosing>> _bufferClosingSelector;

            public Selector(IRefObservable<TSource> source, Func<IRefObservable<TBufferClosing>> bufferClosingSelector)
            {
                _source = source;
                _bufferClosingSelector = bufferClosingSelector;
            }

            protected override _ CreateSink(IRefObserver<IList<TSource>> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<TSource, IList<TSource>>
            {
                private readonly object _gate = new object();
                private readonly AsyncLock _bufferGate = new AsyncLock();
                private readonly Func<IRefObservable<TBufferClosing>> _bufferClosingSelector;

                private List<TSource> _buffer = new List<TSource>();
                private SerialDisposableValue _bufferClosingSerialDisposable;

                public _(Selector parent, IRefObserver<IList<TSource>> observer)
                    : base(observer)
                {
                    _bufferClosingSelector = parent._bufferClosingSelector;
                }

                public override void Run(IRefObservable<TSource> source)
                {
                    base.Run(source);

                    _bufferGate.Wait(this, static @this => @this.CreateBufferClose());
                }

                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        _bufferClosingSerialDisposable.Dispose();
                    }
                    base.Dispose(disposing);
                }

                private void CreateBufferClose()
                {
                    IRefObservable<TBufferClosing> bufferClose;
                    try
                    {
                        bufferClose = _bufferClosingSelector();
                    }
                    catch (Exception exception)
                    {
                        lock (_gate)
                        {
                            ForwardOnError(exception);
                        }
                        return;
                    }

                    var closingObserver = new BufferClosingObserver(this);
                    _bufferClosingSerialDisposable.Disposable = closingObserver;
                    closingObserver.SetResource(bufferClose.SubscribeSafe(closingObserver));
                }

                private void CloseBuffer(IDisposable closingSubscription)
                {
                    closingSubscription.Dispose();

                    lock (_gate)
                    {
                        var res = _buffer;
                        _buffer = new List<TSource>();
                        ForwardOnNext(ref Unsafe.AsRef<IList<TSource>>(res));
                    }

                    _bufferGate.Wait(this, static @this => @this.CreateBufferClose());
                }

                private sealed class BufferClosingObserver : SafeObserver<TBufferClosing>
                {
                    private readonly _ _parent;

                    public BufferClosingObserver(_ parent)
                    {
                        _parent = parent;
                    }

                    public override void OnNext(ref TBufferClosing value)
                    {
                        _parent.CloseBuffer(this);
                    }

                    public override void OnError(Exception error)
                    {
                        _parent.OnError(error);
                    }

                    public override void OnCompleted()
                    {
                        _parent.CloseBuffer(this);
                    }
                }

                public override void OnNext(ref TSource value)
                {
                    lock (_gate)
                    {
                        _buffer.Add(value);
                    }
                }

                public override void OnError(Exception error)
                {
                    lock (_gate)
                    {
                        _buffer.Clear();
                        ForwardOnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    lock (_gate)
                    {
                        ForwardOnNext(ref Unsafe.AsRef<IList<TSource>>(_buffer));
                        ForwardOnCompleted();
                    }
                }
            }
        }

        internal sealed class Boundaries : Producer<IList<TSource>, Boundaries._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly IRefObservable<TBufferClosing> _bufferBoundaries;

            public Boundaries(IRefObservable<TSource> source, IRefObservable<TBufferClosing> bufferBoundaries)
            {
                _source = source;
                _bufferBoundaries = bufferBoundaries;
            }

            protected override _ CreateSink(IRefObserver<IList<TSource>> observer) => new _(observer);

            protected override void Run(_ sink) => sink.Run(this);

            internal sealed class _ : Sink<TSource, IList<TSource>>
            {
                private readonly object _gate = new object();

                private List<TSource> _buffer = new List<TSource>();
                private SingleAssignmentDisposableValue _boundariesDisposable;

                public _(IRefObserver<IList<TSource>> observer)
                    : base(observer)
                {
                }

                public void Run(Boundaries parent)
                {
                    Run(parent._source);
                    _boundariesDisposable.Disposable = parent._bufferBoundaries.SubscribeSafe(new BufferClosingObserver(this));
                }

                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        _boundariesDisposable.Dispose();
                    }

                    base.Dispose(disposing);
                }

                private sealed class BufferClosingObserver : IRefObserver<TBufferClosing>
                {
                    private readonly _ _parent;

                    public BufferClosingObserver(_ parent)
                    {
                        _parent = parent;
                    }

                    public void OnNext(ref TBufferClosing value)
                    {
                        lock (_parent._gate)
                        {
                            var res = _parent._buffer;
                            _parent._buffer = new List<TSource>();
                            _parent.ForwardOnNext(ref Unsafe.AsRef<IList<TSource>>(res));
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
                        _buffer.Add(value);
                    }
                }

                public override void OnError(Exception error)
                {
                    lock (_gate)
                    {
                        _buffer.Clear();
                        ForwardOnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    lock (_gate)
                    {
                        ForwardOnNext(ref Unsafe.AsRef<IList<TSource>>(_buffer));
                        ForwardOnCompleted();
                    }
                }
            }
        }
    }
}
