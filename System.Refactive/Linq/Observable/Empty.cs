// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Concurrency;
using System.Refactive.Disposables;
using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class Empty<TResult> : Producer<TResult, Empty<TResult>._>
    {
        private readonly IScheduler _scheduler;

        public Empty(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        protected override _ CreateSink(IRefObserver<TResult> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_scheduler);

        internal sealed class _ : IdentitySink<TResult>
        {
            public _(IRefObserver<TResult> observer)
                : base(observer)
            {
            }

            public void Run(IScheduler scheduler)
            {
                SetUpstream(scheduler.ScheduleAction(this, static target => target.OnCompleted()));
            }
        }
    }

    internal sealed class EmptyDirect<TResult> : BasicProducer<TResult>
    {
        internal static readonly IRefObservable<TResult> Instance = new EmptyDirect<TResult>();

        private EmptyDirect() { }

        protected override IDisposable Run(IRefObserver<TResult> observer)
        {
            observer.OnCompleted();
            return Disposable.Empty;
        }
    }
}
