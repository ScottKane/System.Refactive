// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Concurrency;
using System.Refactive.Disposables;
using System.Refactive.Internal;
using System.Runtime.CompilerServices;

namespace System.Refactive.Linq
{
    internal static class TakeLast<TSource>
    {
        internal sealed class Count : Producer<TSource, Count._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly int _count;
            private readonly IScheduler _loopScheduler;

            public Count(IRefObservable<TSource> source, int count, IScheduler loopScheduler)
            {
                _source = source;
                _count = count;
                _loopScheduler = loopScheduler;
            }

            protected override _ CreateSink(IRefObserver<TSource> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : IdentitySink<TSource>
            {
                private readonly int _count;
                private readonly IScheduler _loopScheduler;
                private readonly Queue<TSource> _queue;

                public _(Count parent, IRefObserver<TSource> observer)
                    : base(observer)
                {
                    _count = parent._count;
                    _loopScheduler = parent._loopScheduler;
                    _queue = new Queue<TSource>();
                }

                private MultipleAssignmentDisposableValue _loopDisposable;

                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        _loopDisposable.Dispose();
                    }

                    base.Dispose(disposing);
                }

                public override void OnNext(ref TSource value)
                {
                    _queue.Enqueue(value);

                    if (_queue.Count > _count)
                    {
                        _queue.Dequeue();
                    }
                }

                public override void OnCompleted()
                {
                    DisposeUpstream();

                    var longRunning = _loopScheduler.AsLongRunning();
                    if (longRunning != null)
                    {
                        _loopDisposable.TrySetFirst(longRunning.ScheduleLongRunning(this, static (@this, c) => @this.Loop(c)));
                    }
                    else
                    {
                        var first = _loopScheduler.Schedule(this, static (innerScheduler, @this) => @this.LoopRec(innerScheduler));
                        _loopDisposable.TrySetFirst(first);
                    }
                }

                private IDisposable LoopRec(IScheduler scheduler)
                {
                    if (_queue.Count > 0)
                    {
                        ForwardOnNext(ref Unsafe.AsRef(_queue.Dequeue()));

                        var next = scheduler.Schedule(this, static (innerScheduler, @this) => @this.LoopRec(innerScheduler));
                        _loopDisposable.Disposable = next;
                    }
                    else
                    {
                        ForwardOnCompleted();
                    }
                    return Disposable.Empty;
                }

                private void Loop(ICancelable cancel)
                {
                    var n = _queue.Count;

                    while (!cancel.IsDisposed)
                    {
                        if (n == 0)
                        {
                            ForwardOnCompleted();
                            break;
                        }

                        ForwardOnNext(ref Unsafe.AsRef(_queue.Dequeue()));

                        n--;
                    }

                    Dispose();
                }
            }
        }

        internal sealed class Time : Producer<TSource, Time._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly TimeSpan _duration;
            private readonly IScheduler _scheduler;
            private readonly IScheduler _loopScheduler;

            public Time(IRefObservable<TSource> source, TimeSpan duration, IScheduler scheduler, IScheduler loopScheduler)
            {
                _source = source;
                _duration = duration;
                _scheduler = scheduler;
                _loopScheduler = loopScheduler;
            }

            protected override _ CreateSink(IRefObserver<TSource> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(_source, _scheduler);

            internal sealed class _ : IdentitySink<TSource>
            {
                private readonly TimeSpan                       _duration;
                private readonly IScheduler                     _loopScheduler;
                private readonly Queue<Refactive.TimeInterval<TSource>> _queue;

                public _(Time parent, IRefObserver<TSource> observer)
                    : base(observer)
                {
                    _duration = parent._duration;
                    _loopScheduler = parent._loopScheduler;
                    _queue = new Queue<Refactive.TimeInterval<TSource>>();
                }

                private MultipleAssignmentDisposableValue _loopDisposable;
                private IStopwatch? _watch;

                public void Run(IRefObservable<TSource> source, IScheduler scheduler)
                {
                    _watch = scheduler.StartStopwatch();
                    Run(source);
                }

                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        _loopDisposable.Dispose();
                    }

                    base.Dispose(disposing);
                }

                public override void OnNext(ref TSource value)
                {
                    var now = _watch!.Elapsed;
                    _queue.Enqueue(new Refactive.TimeInterval<TSource>(value, now));
                    Trim(now);
                }

                public override void OnCompleted()
                {
                    DisposeUpstream();

                    var now = _watch!.Elapsed;
                    Trim(now);

                    var longRunning = _loopScheduler.AsLongRunning();
                    if (longRunning != null)
                    {
                        _loopDisposable.TrySetFirst(longRunning.ScheduleLongRunning(this, static (@this, c) => @this.Loop(c)));
                    }
                    else
                    {
                        var first = _loopScheduler.Schedule(this, static (innerScheduler, @this) => @this.LoopRec(innerScheduler));
                        _loopDisposable.TrySetFirst(first);
                    }
                }

                private IDisposable LoopRec(IScheduler scheduler)
                {
                    if (_queue.Count > 0)
                    {
                        ForwardOnNext(ref Unsafe.AsRef(_queue.Dequeue().Value));

                        var next = scheduler.Schedule(this, static (innerScheduler, @this) => @this.LoopRec(innerScheduler));
                        _loopDisposable.Disposable = next;
                    }
                    else
                    {
                        ForwardOnCompleted();
                    }

                    return Disposable.Empty;
                }

                private void Loop(ICancelable cancel)
                {
                    var n = _queue.Count;

                    while (!cancel.IsDisposed)
                    {
                        if (n == 0)
                        {
                            ForwardOnCompleted();
                            break;
                        }

                        ForwardOnNext(ref Unsafe.AsRef(_queue.Dequeue().Value));

                        n--;
                    }

                    Dispose();
                }

                private void Trim(TimeSpan now)
                {
                    while (_queue.Count > 0 && now - _queue.Peek().Interval >= _duration)
                    {
                        _queue.Dequeue();
                    }
                }
            }
        }
    }
}
