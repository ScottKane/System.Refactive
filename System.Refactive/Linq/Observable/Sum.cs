// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Internal;
using System.Runtime.CompilerServices;

namespace System.Refactive.Linq
{
    internal sealed class SumDouble : Producer<double, SumDouble._>
    {
        private readonly IRefObservable<double> _source;

        public SumDouble(IRefObservable<double> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<double> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<double>
        {
            private double _sum;

            public _(IRefObserver<double> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref double value)
            {
                _sum += value;
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref _sum);
                ForwardOnCompleted();
            }
        }
    }

    internal sealed class SumSingle : Producer<float, SumSingle._>
    {
        private readonly IRefObservable<float> _source;

        public SumSingle(IRefObservable<float> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<float> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<float>
        {
            private double _sum; // This is what LINQ to Objects does (accumulates into double that is)!

            public _(IRefObserver<float> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref float value)
            {
                _sum += value; // This is what LINQ to Objects does!
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref Unsafe.AsRef((float)_sum)); // This is what LINQ to Objects does!
                ForwardOnCompleted();
            }
        }
    }

    internal sealed class SumDecimal : Producer<decimal, SumDecimal._>
    {
        private readonly IRefObservable<decimal> _source;

        public SumDecimal(IRefObservable<decimal> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<decimal> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<decimal>
        {
            private decimal _sum;

            public _(IRefObserver<decimal> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref decimal value)
            {
                _sum += value;
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref _sum);
                ForwardOnCompleted();
            }
        }
    }

    internal sealed class SumInt32 : Producer<int, SumInt32._>
    {
        private readonly IRefObservable<int> _source;

        public SumInt32(IRefObservable<int> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<int> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<int>
        {
            private int _sum;

            public _(IRefObserver<int> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref int value)
            {
                try
                {
                    checked
                    {
                        _sum += value;
                    }
                }
                catch (Exception exception)
                {
                    ForwardOnError(exception);
                }
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref _sum);
                ForwardOnCompleted();
            }
        }
    }

    internal sealed class SumInt64 : Producer<long, SumInt64._>
    {
        private readonly IRefObservable<long> _source;

        public SumInt64(IRefObservable<long> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<long> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<long>
        {
            private long _sum;

            public _(IRefObserver<long> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref long value)
            {
                try
                {
                    checked
                    {
                        _sum += value;
                    }
                }
                catch (Exception exception)
                {
                    ForwardOnError(exception);
                }
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref _sum);
                ForwardOnCompleted();
            }
        }
    }

    internal sealed class SumDoubleNullable : Producer<double?, SumDoubleNullable._>
    {
        private readonly IRefObservable<double?> _source;

        public SumDoubleNullable(IRefObservable<double?> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<double?> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<double?>
        {
            private double _sum;

            public _(IRefObserver<double?> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref double? value)
            {
                if (value != null)
                {
                    _sum += value.Value;
                }
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref Unsafe.AsRef<double?>(_sum));
                ForwardOnCompleted();
            }
        }
    }

    internal sealed class SumSingleNullable : Producer<float?, SumSingleNullable._>
    {
        private readonly IRefObservable<float?> _source;

        public SumSingleNullable(IRefObservable<float?> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<float?> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<float?>
        {
            private double _sum; // This is what LINQ to Objects does (accumulates into double that is)!

            public _(IRefObserver<float?> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref float? value)
            {
                if (value != null)
                {
                    _sum += value.Value; // This is what LINQ to Objects does!
                }
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref Unsafe.AsRef<float?>((float)_sum)); // This is what LINQ to Objects does!
                ForwardOnCompleted();
            }
        }
    }

    internal sealed class SumDecimalNullable : Producer<decimal?, SumDecimalNullable._>
    {
        private readonly IRefObservable<decimal?> _source;

        public SumDecimalNullable(IRefObservable<decimal?> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<decimal?> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<decimal?>
        {
            private decimal _sum;

            public _(IRefObserver<decimal?> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref decimal? value)
            {
                if (value != null)
                {
                    _sum += value.Value;
                }
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref Unsafe.AsRef<decimal?>(_sum));
                ForwardOnCompleted();
            }
        }
    }

    internal sealed class SumInt32Nullable : Producer<int?, SumInt32Nullable._>
    {
        private readonly IRefObservable<int?> _source;

        public SumInt32Nullable(IRefObservable<int?> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<int?> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<int?>
        {
            private int _sum;

            public _(IRefObserver<int?> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref int? value)
            {
                try
                {
                    checked
                    {
                        if (value != null)
                        {
                            _sum += value.Value;
                        }
                    }
                }
                catch (Exception exception)
                {
                    ForwardOnError(exception);
                }
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref Unsafe.AsRef<int?>(_sum));
                ForwardOnCompleted();
            }
        }
    }

    internal sealed class SumInt64Nullable : Producer<long?, SumInt64Nullable._>
    {
        private readonly IRefObservable<long?> _source;

        public SumInt64Nullable(IRefObservable<long?> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<long?> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<long?>
        {
            private long _sum;

            public _(IRefObserver<long?> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref long? value)
            {
                try
                {
                    checked
                    {
                        if (value != null)
                        {
                            _sum += value.Value;
                        }
                    }
                }
                catch (Exception exception)
                {
                    ForwardOnError(exception);
                }
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref Unsafe.AsRef<long?>(_sum));
                ForwardOnCompleted();
            }
        }
    }
}
