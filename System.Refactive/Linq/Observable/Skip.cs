﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Concurrency;
using System.Refactive.Disposables;
using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal static class Skip<TSource>
    {
        internal sealed class Count : Producer<TSource, Count._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly int _count;

            public Count(IRefObservable<TSource> source, int count)
            {
                _source = source;
                _count = count;
            }

            public IRefObservable<TSource> Combine(int count)
            {
                //
                // Sum semantics:
                //
                //   xs                    --o--o--o--o--o--o--|      xs                    --o--o--o--o--o--o--|
                //   xs.Skip(2)            --x--x--o--o--o--o--|      xs.Skip(3)            --x--x--x--o--o--o--|
                //   xs.Skip(2).Skip(3)    --------x--x--x--o--|      xs.Skip(3).Skip(2)    -----------x--x--o--|
                //
                return new Count(_source, _count + count);
            }

            protected override _ CreateSink(IRefObserver<TSource> observer) => new _(_count, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : IdentitySink<TSource>
            {
                private int _remaining;

                public _(int count, IRefObserver<TSource> observer)
                    : base(observer)
                {
                    _remaining = count;
                }

                public override void OnNext(ref TSource value)
                {
                    if (_remaining <= 0)
                    {
                        ForwardOnNext(ref value);
                    }
                    else
                    {
                        _remaining--;
                    }
                }
            }
        }

        internal sealed class Time : Producer<TSource, Time._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly TimeSpan _duration;
            internal readonly IScheduler _scheduler;

            public Time(IRefObservable<TSource> source, TimeSpan duration, IScheduler scheduler)
            {
                _source = source;
                _duration = duration;
                _scheduler = scheduler;
            }

            public IRefObservable<TSource> Combine(TimeSpan duration)
            {
                //
                // Maximum semantics:
                //
                //   t                     0--1--2--3--4--5--6--7->   t                     0--1--2--3--4--5--6--7->
                //                                                    
                //   xs                    --o--o--o--o--o--o--|      xs                    --o--o--o--o--o--o--|
                //   xs.Skip(2s)           xxxxxxx-o--o--o--o--|      xs.Skip(3s)           xxxxxxxxxx-o--o--o--|
                //   xs.Skip(2s).Skip(3s)  xxxxxxxxxx-o--o--o--|      xs.Skip(3s).Skip(2s)  xxxxxxx----o--o--o--|
                //
                if (duration <= _duration)
                {
                    return this;
                }

                return new Time(_source, duration, _scheduler);
            }

            protected override _ CreateSink(IRefObserver<TSource> observer) => new _(observer);

            protected override void Run(_ sink) => sink.Run(this);

            internal sealed class _ : IdentitySink<TSource>
            {
                private volatile bool _open;

                public _(IRefObserver<TSource> observer)
                    : base(observer)
                {
                }

                private SingleAssignmentDisposableValue _sourceDisposable;

                public void Run(Time parent)
                {
                    SetUpstream(parent._scheduler.ScheduleAction(this, parent._duration, state => state.Tick()));
                    _sourceDisposable.Disposable = parent._source.SubscribeSafe(this);
                }

                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        _sourceDisposable.Dispose();
                    }

                    base.Dispose(disposing);
                }

                private void Tick()
                {
                    _open = true;
                }

                public override void OnNext(ref TSource value)
                {
                    if (_open)
                    {
                        ForwardOnNext(ref value);
                    }
                }
            }
        }
    }
}
