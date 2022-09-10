// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Internal;
using System.Runtime.CompilerServices;

namespace System.Refactive.Linq
{
    internal sealed class AverageDouble : Producer<double, AverageDouble._>
    {
        private readonly IRefObservable<double> _source;

        public AverageDouble(IRefObservable<double> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<double> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<double>
        {
            private double _sum;
            private long _count;

            public _(IRefObserver<double> observer)
                : base(observer)
            {
                _sum = 0.0;
                _count = 0L;
            }

            public override void OnNext(ref double value)
            {
                try
                {
                    checked
                    {
                        _sum += value;
                        _count++;
                    }
                }
                catch (Exception ex)
                {
                    ForwardOnError(ex);
                }
            }

            public override void OnCompleted()
            {
                if (_count > 0)
                {
                    ForwardOnNext(ref Unsafe.AsRef(_sum / _count));
                    ForwardOnCompleted();
                }
                else
                {
                    try
                    {
                        throw new InvalidOperationException(Strings_Linq.NO_ELEMENTS);
                    }
                    catch (Exception e)
                    {
                        ForwardOnError(e);
                    }
                }
            }
        }
    }

    internal sealed class AverageSingle : Producer<float, AverageSingle._>
    {
        private readonly IRefObservable<float> _source;

        public AverageSingle(IRefObservable<float> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<float> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<float>
        {
            private double _sum; // NOTE: Uses a different accumulator type (double), conform LINQ to Objects.
            private long _count;

            public _(IRefObserver<float> observer)
                : base(observer)
            {
                _sum = 0.0;
                _count = 0L;
            }

            public override void OnNext(ref float value)
            {
                try
                {
                    checked
                    {
                        _sum += value;
                        _count++;
                    }
                }
                catch (Exception ex)
                {
                    ForwardOnError(ex);
                }
            }

            public override void OnCompleted()
            {
                if (_count > 0)
                {
                    ForwardOnNext(ref Unsafe.AsRef((float)(_sum / _count)));
                    ForwardOnCompleted();
                }
                else
                {
                    try
                    {
                        throw new InvalidOperationException(Strings_Linq.NO_ELEMENTS);
                    }
                    catch (Exception e)
                    {
                        ForwardOnError(e);
                    }
                }
            }
        }
    }

    internal sealed class AverageDecimal : Producer<decimal, AverageDecimal._>
    {
        private readonly IRefObservable<decimal> _source;

        public AverageDecimal(IRefObservable<decimal> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<decimal> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<decimal>
        {
            private decimal _sum;
            private long _count;

            public _(IRefObserver<decimal> observer)
                : base(observer)
            {
                _sum = 0M;
                _count = 0L;
            }

            public override void OnNext(ref decimal value)
            {
                try
                {
                    checked
                    {
                        _sum += value;
                        _count++;
                    }
                }
                catch (Exception ex)
                {
                    ForwardOnError(ex);
                }
            }

            public override void OnCompleted()
            {
                if (_count > 0)
                {
                    ForwardOnNext(ref Unsafe.AsRef(_sum / _count));
                    ForwardOnCompleted();
                }
                else
                {
                    try
                    {
                        throw new InvalidOperationException(Strings_Linq.NO_ELEMENTS);
                    }
                    catch (Exception e)
                    {
                        ForwardOnError(e);
                    }
                }
            }
        }
    }

    internal sealed class AverageInt32 : Producer<double, AverageInt32._>
    {
        private readonly IRefObservable<int> _source;

        public AverageInt32(IRefObservable<int> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<double> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : Sink<int, double>
        {
            private long _sum;
            private long _count;

            public _(IRefObserver<double> observer)
                : base(observer)
            {
                _sum = 0L;
                _count = 0L;
            }

            public override void OnNext(ref int value)
            {
                try
                {
                    checked
                    {
                        _sum += value;
                        _count++;
                    }
                }
                catch (Exception ex)
                {
                    ForwardOnError(ex);
                }
            }

            public override void OnCompleted()
            {
                if (_count > 0)
                {
                    ForwardOnNext(ref Unsafe.AsRef((double)_sum / _count));
                    ForwardOnCompleted();
                }
                else
                {
                    try
                    {
                        throw new InvalidOperationException(Strings_Linq.NO_ELEMENTS);
                    }
                    catch (Exception e)
                    {
                        ForwardOnError(e);
                    }
                }
            }
        }
    }

    internal sealed class AverageInt64 : Producer<double, AverageInt64._>
    {
        private readonly IRefObservable<long> _source;

        public AverageInt64(IRefObservable<long> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<double> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : Sink<long, double>
        {
            private long _sum;
            private long _count;

            public _(IRefObserver<double> observer)
                : base(observer)
            {
                _sum = 0L;
                _count = 0L;
            }

            public override void OnNext(ref long value)
            {
                try
                {
                    checked
                    {
                        _sum += value;
                        _count++;
                    }
                }
                catch (Exception ex)
                {
                    ForwardOnError(ex);
                }
            }

            public override void OnCompleted()
            {
                if (_count > 0)
                {
                    ForwardOnNext(ref Unsafe.AsRef((double)_sum / _count));
                    ForwardOnCompleted();
                }
                else
                {
                    try
                    {
                        throw new InvalidOperationException(Strings_Linq.NO_ELEMENTS);
                    }
                    catch (Exception e)
                    {
                        ForwardOnError(e);
                    }
                }
            }
        }
    }

    internal sealed class AverageDoubleNullable : Producer<double?, AverageDoubleNullable._>
    {
        private readonly IRefObservable<double?> _source;

        public AverageDoubleNullable(IRefObservable<double?> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<double?> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<double?>
        {
            private double _sum;
            private long _count;

            public _(IRefObserver<double?> observer)
                : base(observer)
            {
                _sum = 0.0;
                _count = 0L;
            }

            public override void OnNext(ref double? value)
            {
                try
                {
                    checked
                    {
                        if (value != null)
                        {
                            _sum += value.Value;
                            _count++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ForwardOnError(ex);
                }
            }

            public override unsafe void OnCompleted()
            {
                if (_count > 0)
                {
                    ForwardOnNext(ref Unsafe.AsRef<double?>(_sum / _count));
                }
                else
                {
                    ForwardOnNext(ref Unsafe.AsRef<double?>(null));
                }

                ForwardOnCompleted();
            }
        }
    }

    internal sealed class AverageSingleNullable : Producer<float?, AverageSingleNullable._>
    {
        private readonly IRefObservable<float?> _source;

        public AverageSingleNullable(IRefObservable<float?> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<float?> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<float?>
        {
            private double _sum; // NOTE: Uses a different accumulator type (double), conform LINQ to Objects.
            private long _count;

            public _(IRefObserver<float?> observer)
                : base(observer)
            {
                _sum = 0.0;
                _count = 0L;
            }

            public override void OnNext(ref float? value)
            {
                try
                {
                    checked
                    {
                        if (value != null)
                        {
                            _sum += value.Value;
                            _count++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ForwardOnError(ex);
                }
            }

            public override unsafe void OnCompleted()
            {
                if (_count > 0)
                {
                    ForwardOnNext(ref Unsafe.AsRef<float?>((float)(_sum / _count)));
                }
                else
                {
                    ForwardOnNext(ref Unsafe.AsRef<float?>(null));
                }

                ForwardOnCompleted();
            }
        }
    }

    internal sealed class AverageDecimalNullable : Producer<decimal?, AverageDecimalNullable._>
    {
        private readonly IRefObservable<decimal?> _source;

        public AverageDecimalNullable(IRefObservable<decimal?> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<decimal?> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<decimal?>
        {
            private decimal _sum;
            private long _count;

            public _(IRefObserver<decimal?> observer)
                : base(observer)
            {
                _sum = 0M;
                _count = 0L;
            }

            public override void OnNext(ref decimal? value)
            {
                try
                {
                    checked
                    {
                        if (value != null)
                        {
                            _sum += value.Value;
                            _count++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ForwardOnError(ex);
                }
            }

            public override unsafe void OnCompleted()
            {
                if (_count > 0)
                {
                    ForwardOnNext(ref Unsafe.AsRef<decimal?>(_sum / _count));
                }
                else
                {
                    ForwardOnNext(ref Unsafe.AsRef<decimal?>(null));
                }

                ForwardOnCompleted();
            }
        }
    }

    internal sealed class AverageInt32Nullable : Producer<double?, AverageInt32Nullable._>
    {
        private readonly IRefObservable<int?> _source;

        public AverageInt32Nullable(IRefObservable<int?> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<double?> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : Sink<int?, double?>
        {
            private long _sum;
            private long _count;

            public _(IRefObserver<double?> observer)
                : base(observer)
            {
                _sum = 0L;
                _count = 0L;
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
                            _count++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ForwardOnError(ex);
                }
            }

            public override unsafe void OnCompleted()
            {
                if (_count > 0)
                {
                    ForwardOnNext(ref Unsafe.AsRef<double?>((double)_sum / _count));
                }
                else
                {
                    ForwardOnNext(ref Unsafe.AsRef<double?>(null));
                }

                ForwardOnCompleted();
            }
        }
    }

    internal sealed class AverageInt64Nullable : Producer<double?, AverageInt64Nullable._>
    {
        private readonly IRefObservable<long?> _source;

        public AverageInt64Nullable(IRefObservable<long?> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<double?> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : Sink<long?, double?>
        {
            private long _sum;
            private long _count;

            public _(IRefObserver<double?> observer)
                : base(observer)
            {
                _sum = 0L;
                _count = 0L;
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
                            _count++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ForwardOnError(ex);
                }
            }

            public override unsafe void OnCompleted()
            {
                if (_count > 0)
                {
                    ForwardOnNext(ref Unsafe.AsRef<double?>((double)_sum / _count));
                }
                else
                {
                    ForwardOnNext(ref Unsafe.AsRef<double?>(null));
                }

                ForwardOnCompleted();
            }
        }
    }
}
