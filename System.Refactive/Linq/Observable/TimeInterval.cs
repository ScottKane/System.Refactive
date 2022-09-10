// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Concurrency;
using System.Refactive.Internal;
using System.Runtime.CompilerServices;

namespace System.Refactive.Linq
{
    internal sealed class TimeInterval<TSource> : Producer<Refactive.TimeInterval<TSource>, TimeInterval<TSource>._>
    {
        private readonly IRefObservable<TSource> _source;
        private readonly IScheduler _scheduler;

        public TimeInterval(IRefObservable<TSource> source, IScheduler scheduler)
        {
            _source = source;
            _scheduler = scheduler;
        }

        protected override _ CreateSink(IRefObserver<Refactive.TimeInterval<TSource>> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(this);

        internal sealed class _ : Sink<TSource, Refactive.TimeInterval<TSource>>
        {
            public _(IRefObserver<Refactive.TimeInterval<TSource>> observer)
                : base(observer)
            {
            }

            private IStopwatch? _watch;
            private TimeSpan _last;

            public void Run(TimeInterval<TSource> parent)
            {
                _watch = parent._scheduler.StartStopwatch();
                _last = TimeSpan.Zero;

                SetUpstream(parent._source.Subscribe(this));
            }

            public override void OnNext(ref TSource value)
            {
                var now = _watch!.Elapsed; // NB: Watch is assigned during Run.
                var span = now.Subtract(_last);
                _last = now;
                ForwardOnNext(ref Unsafe.AsRef(new Refactive.TimeInterval<TSource>(value, span)));
            }
        }
    }
}
