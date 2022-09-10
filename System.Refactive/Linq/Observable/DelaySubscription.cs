// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Concurrency;
using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal abstract class DelaySubscription<TSource> : Producer<TSource, DelaySubscription<TSource>._>
    {
        private readonly IRefObservable<TSource> _source;
        private readonly IScheduler _scheduler;

        protected DelaySubscription(IRefObservable<TSource> source, IScheduler scheduler)
        {
            _source = source;
            _scheduler = scheduler;
        }

        internal sealed class Relative : DelaySubscription<TSource>
        {
            private readonly TimeSpan _dueTime;

            public Relative(IRefObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler)
                : base(source, scheduler)
            {
                _dueTime = dueTime;
            }

            protected override _ CreateSink(IRefObserver<TSource> observer) => new _(observer);

            protected override void Run(_ sink) => sink.Run(_source, _scheduler, _dueTime);
        }

        internal sealed class Absolute : DelaySubscription<TSource>
        {
            private readonly DateTimeOffset _dueTime;

            public Absolute(IRefObservable<TSource> source, DateTimeOffset dueTime, IScheduler scheduler)
                : base(source, scheduler)
            {
                _dueTime = dueTime;
            }

            protected override _ CreateSink(IRefObserver<TSource> observer) => new _(observer);

            protected override void Run(_ sink) => sink.Run(_source, _scheduler, _dueTime);
        }

        internal sealed class _ : IdentitySink<TSource>
        {
            public _(IRefObserver<TSource> observer)
                : base(observer)
            {
            }

            public void Run(IRefObservable<TSource> source, IScheduler scheduler, DateTimeOffset dueTime)
            {
                SetUpstream(scheduler.ScheduleAction((@this: this, source), dueTime, static tuple => tuple.source.SubscribeSafe(tuple.@this)));
            }

            public void Run(IRefObservable<TSource> source, IScheduler scheduler, TimeSpan dueTime)
            {
                SetUpstream(scheduler.ScheduleAction((@this: this, source), dueTime, static tuple => tuple.source.SubscribeSafe(tuple.@this)));
            }
        }
    }
}
