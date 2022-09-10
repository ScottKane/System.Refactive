// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class Scan<TSource, TAccumulate> : Producer<TAccumulate, Scan<TSource, TAccumulate>._>
    {
        private readonly IRefObservable<TSource> _source;
        private readonly TAccumulate _seed;
        private readonly Func<TAccumulate, TSource, TAccumulate> _accumulator;

        public Scan(IRefObservable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
        {
            _source = source;
            _seed = seed;
            _accumulator = accumulator;
        }

        protected override _ CreateSink(IRefObserver<TAccumulate> observer) => new _(this, observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : Sink<TSource, TAccumulate>
        {
            private readonly Func<TAccumulate, TSource, TAccumulate> _accumulator;
            private TAccumulate _accumulation;

            public _(Scan<TSource, TAccumulate> parent, IRefObserver<TAccumulate> observer)
                : base(observer)
            {
                _accumulator = parent._accumulator;
                _accumulation = parent._seed;
            }

            public override void OnNext(ref TSource value)
            {
                try
                {
                    _accumulation = _accumulator(_accumulation, value);
                }
                catch (Exception exception)
                {
                    ForwardOnError(exception);
                    return;
                }

                ForwardOnNext(ref _accumulation);
            }
        }
    }

    internal sealed class Scan<TSource> : Producer<TSource, Scan<TSource>._>
    {
        private readonly IRefObservable<TSource> _source;
        private readonly Func<TSource, TSource, TSource> _accumulator;

        public Scan(IRefObservable<TSource> source, Func<TSource, TSource, TSource> accumulator)
        {
            _source = source;
            _accumulator = accumulator;
        }

        protected override _ CreateSink(IRefObserver<TSource> observer) => new _(_accumulator, observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<TSource>
        {
            private readonly Func<TSource, TSource, TSource> _accumulator;
            private TSource? _accumulation;
            private bool _hasAccumulation;

            public _(Func<TSource, TSource, TSource> accumulator, IRefObserver<TSource> observer)
                : base(observer)
            {
                _accumulator = accumulator;
            }

            public override void OnNext(ref TSource value)
            {
                try
                {
                    if (_hasAccumulation)
                    {
                        _accumulation = _accumulator(_accumulation!, value);
                    }
                    else
                    {
                        _accumulation = value;
                        _hasAccumulation = true;
                    }
                }
                catch (Exception exception)
                {
                    ForwardOnError(exception);

                    return;
                }

                ForwardOnNext(ref _accumulation);
            }
        }
    }
}
