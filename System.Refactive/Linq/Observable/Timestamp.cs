// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Concurrency;
using System.Refactive.Internal;
using System.Runtime.CompilerServices;

namespace System.Refactive.Linq
{
    internal sealed class Timestamp<TSource> : Producer<Timestamped<TSource>, Timestamp<TSource>._>
    {
        private readonly IRefObservable<TSource> _source;
        private readonly IScheduler _scheduler;

        public Timestamp(IRefObservable<TSource> source, IScheduler scheduler)
        {
            _source = source;
            _scheduler = scheduler;
        }

        protected override _ CreateSink(IRefObserver<Timestamped<TSource>> observer) => new _(_scheduler, observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : Sink<TSource, Timestamped<TSource>>
        {
            private readonly IScheduler _scheduler;

            public _(IScheduler scheduler, IRefObserver<Timestamped<TSource>> observer)
                : base(observer)
            {
                _scheduler = scheduler;
            }

            public override void OnNext(ref TSource value)
            {
                ForwardOnNext(ref Unsafe.AsRef(new Timestamped<TSource>(value, _scheduler.Now)));
            }
        }
    }
}
