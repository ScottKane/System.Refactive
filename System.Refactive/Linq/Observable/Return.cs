// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Concurrency;
using System.Refactive.Disposables;
using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class Return<TResult> : Producer<TResult, Return<TResult>._>
    {
        private readonly TResult _value;
        private readonly IScheduler _scheduler;

        public Return(TResult value, IScheduler scheduler)
        {
            _value = value;
            _scheduler = scheduler;
        }

        protected override _ CreateSink(IRefObserver<TResult> observer) => new _(_value, observer);

        protected override void Run(_ sink) => sink.Run(_scheduler);

        internal sealed class _ : IdentitySink<TResult>
        {
            private TResult _value;

            public _(TResult value, IRefObserver<TResult> observer)
                : base(observer)
            {
                _value = value;
            }

            public void Run(IScheduler scheduler)
            {
                SetUpstream(scheduler.ScheduleAction(this, static @this => @this.Invoke()));
            }

            private void Invoke()
            {
                ForwardOnNext(ref _value);
                ForwardOnCompleted();
            }
        }
    }

    // There is no need for a full Producer/IdentitySink as there is no
    // way to stop a first task running on the immediate scheduler
    // as it is always synchronous.
    internal sealed class ReturnImmediate<TSource> : BasicProducer<TSource>
    {
        private TSource _value;

        public ReturnImmediate(TSource value)
        {
            _value = value;
        }

        protected override IDisposable Run(IRefObserver<TSource> observer)
        {
            observer.OnNext(ref _value);
            observer.OnCompleted();
            return Disposable.Empty;
        }
    }
}
