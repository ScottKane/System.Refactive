// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class Min<TSource> : Producer<TSource, Min<TSource>._>
    {
        private readonly IRefObservable<TSource> _source;
        private readonly IComparer<TSource> _comparer;

        public Min(IRefObservable<TSource> source, IComparer<TSource> comparer)
        {
            _source = source;
            _comparer = comparer;
        }

        protected override _ CreateSink(IRefObserver<TSource> observer) => default(TSource) == null ? (_)new Null(_comparer, observer) : new NonNull(_comparer, observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal abstract class _ : IdentitySink<TSource>
        {
            protected readonly IComparer<TSource> _comparer;

            protected _(IComparer<TSource> comparer, IRefObserver<TSource> observer)
                : base(observer)
            {
                _comparer = comparer;
            }
        }

        private sealed class NonNull : _
        {
            private bool _hasValue;
            private TSource? _lastValue;

            public NonNull(IComparer<TSource> comparer, IRefObserver<TSource> observer)
                : base(comparer, observer)
            {
            }

            public override void OnNext(ref TSource value)
            {
                if (_hasValue)
                {
                    int comparison;
                    try
                    {
                        comparison = _comparer.Compare(value, _lastValue!);
                    }
                    catch (Exception ex)
                    {
                        ForwardOnError(ex);
                        return;
                    }

                    if (comparison < 0)
                    {
                        _lastValue = value;
                    }
                }
                else
                {
                    _hasValue = true;
                    _lastValue = value;
                }
            }

            public override void OnError(Exception error)
            {
                ForwardOnError(error);
            }

            public override void OnCompleted()
            {
                if (!_hasValue)
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
                else
                {
                    ForwardOnNext(ref _lastValue!);
                    ForwardOnCompleted();
                }
            }
        }

        private sealed class Null : _
        {
            private TSource? _lastValue;

            public Null(IComparer<TSource> comparer, IRefObserver<TSource> observer)
                : base(comparer, observer)
            {
            }

            public override void OnNext(ref TSource value)
            {
                if (value != null)
                {
                    if (_lastValue == null)
                    {
                        _lastValue = value;
                    }
                    else
                    {
                        int comparison;
                        try
                        {
                            comparison = _comparer.Compare(value, _lastValue);
                        }
                        catch (Exception ex)
                        {
                            ForwardOnError(ex);
                            return;
                        }

                        if (comparison < 0)
                        {
                            _lastValue = value;
                        }
                    }
                }
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref _lastValue!);
                ForwardOnCompleted();
            }
        }
    }

    internal sealed class MinDouble : Producer<double, MinDouble._>
    {
        private readonly IRefObservable<double> _source;

        public MinDouble(IRefObservable<double> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<double> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<double>
        {
            private bool _hasValue;
            private double _lastValue;

            public _(IRefObserver<double> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref double value)
            {
                if (_hasValue)
                {
                    if (value < _lastValue || double.IsNaN(value))
                    {
                        _lastValue = value;
                    }
                }
                else
                {
                    _lastValue = value;
                    _hasValue = true;
                }
            }

            public override void OnCompleted()
            {
                if (!_hasValue)
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
                else
                {
                    ForwardOnNext(ref _lastValue);
                    ForwardOnCompleted();
                }
            }
        }
    }

    internal sealed class MinSingle : Producer<float, MinSingle._>
    {
        private readonly IRefObservable<float> _source;

        public MinSingle(IRefObservable<float> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<float> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<float>
        {
            private bool _hasValue;
            private float _lastValue;

            public _(IRefObserver<float> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref float value)
            {
                if (_hasValue)
                {
                    if (value < _lastValue || float.IsNaN(value))
                    {
                        _lastValue = value;
                    }
                }
                else
                {
                    _lastValue = value;
                    _hasValue = true;
                }
            }

            public override void OnCompleted()
            {
                if (!_hasValue)
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
                else
                {
                    ForwardOnNext(ref _lastValue);
                    ForwardOnCompleted();
                }
            }
        }
    }

    internal sealed class MinDecimal : Producer<decimal, MinDecimal._>
    {
        private readonly IRefObservable<decimal> _source;

        public MinDecimal(IRefObservable<decimal> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<decimal> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<decimal>
        {
            private bool _hasValue;
            private decimal _lastValue;

            public _(IRefObserver<decimal> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref decimal value)
            {
                if (_hasValue)
                {
                    if (value < _lastValue)
                    {
                        _lastValue = value;
                    }
                }
                else
                {
                    _lastValue = value;
                    _hasValue = true;
                }
            }

            public override void OnCompleted()
            {
                if (!_hasValue)
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
                else
                {
                    ForwardOnNext(ref _lastValue);
                    ForwardOnCompleted();
                }
            }
        }
    }

    internal sealed class MinInt32 : Producer<int, MinInt32._>
    {
        private readonly IRefObservable<int> _source;

        public MinInt32(IRefObservable<int> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<int> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<int>
        {
            private bool _hasValue;
            private int _lastValue;

            public _(IRefObserver<int> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref int value)
            {
                if (_hasValue)
                {
                    if (value < _lastValue)
                    {
                        _lastValue = value;
                    }
                }
                else
                {
                    _lastValue = value;
                    _hasValue = true;
                }
            }

            public override void OnCompleted()
            {
                if (!_hasValue)
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
                else
                {
                    ForwardOnNext(ref _lastValue);
                    ForwardOnCompleted();
                }
            }
        }
    }

    internal sealed class MinInt64 : Producer<long, MinInt64._>
    {
        private readonly IRefObservable<long> _source;

        public MinInt64(IRefObservable<long> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<long> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<long>
        {
            private bool _hasValue;
            private long _lastValue;

            public _(IRefObserver<long> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref long value)
            {
                if (_hasValue)
                {
                    if (value < _lastValue)
                    {
                        _lastValue = value;
                    }
                }
                else
                {
                    _lastValue = value;
                    _hasValue = true;
                }
            }

            public override void OnCompleted()
            {
                if (!_hasValue)
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
                else
                {
                    ForwardOnNext(ref _lastValue);
                    ForwardOnCompleted();
                }
            }
        }
    }

    internal sealed class MinDoubleNullable : Producer<double?, MinDoubleNullable._>
    {
        private readonly IRefObservable<double?> _source;

        public MinDoubleNullable(IRefObservable<double?> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<double?> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<double?>
        {
            private double? _lastValue;

            public _(IRefObserver<double?> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref double? value)
            {
                if (!value.HasValue)
                {
                    return;
                }

                if (_lastValue.HasValue)
                {
                    if (value < _lastValue || double.IsNaN((double)value))
                    {
                        _lastValue = value;
                    }
                }
                else
                {
                    _lastValue = value;
                }
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref _lastValue);
                ForwardOnCompleted();
            }
        }
    }

    internal sealed class MinSingleNullable : Producer<float?, MinSingleNullable._>
    {
        private readonly IRefObservable<float?> _source;

        public MinSingleNullable(IRefObservable<float?> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<float?> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<float?>
        {
            private float? _lastValue;

            public _(IRefObserver<float?> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref float? value)
            {
                if (!value.HasValue)
                {
                    return;
                }

                if (_lastValue.HasValue)
                {
                    if (value < _lastValue || float.IsNaN((float)value))
                    {
                        _lastValue = value;
                    }
                }
                else
                {
                    _lastValue = value;
                }
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref _lastValue);
                ForwardOnCompleted();
            }
        }
    }

    internal sealed class MinDecimalNullable : Producer<decimal?, MinDecimalNullable._>
    {
        private readonly IRefObservable<decimal?> _source;

        public MinDecimalNullable(IRefObservable<decimal?> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<decimal?> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<decimal?>
        {
            private decimal? _lastValue;

            public _(IRefObserver<decimal?> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref decimal? value)
            {
                if (!value.HasValue)
                {
                    return;
                }

                if (_lastValue.HasValue)
                {
                    if (value < _lastValue)
                    {
                        _lastValue = value;
                    }
                }
                else
                {
                    _lastValue = value;
                }
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref _lastValue);
                ForwardOnCompleted();
            }
        }
    }

    internal sealed class MinInt32Nullable : Producer<int?, MinInt32Nullable._>
    {
        private readonly IRefObservable<int?> _source;

        public MinInt32Nullable(IRefObservable<int?> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<int?> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<int?>
        {
            private int? _lastValue;

            public _(IRefObserver<int?> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref int? value)
            {
                if (!value.HasValue)
                {
                    return;
                }

                if (_lastValue.HasValue)
                {
                    if (value < _lastValue)
                    {
                        _lastValue = value;
                    }
                }
                else
                {
                    _lastValue = value;
                }
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref _lastValue);
                ForwardOnCompleted();
            }
        }
    }

    internal sealed class MinInt64Nullable : Producer<long?, MinInt64Nullable._>
    {
        private readonly IRefObservable<long?> _source;

        public MinInt64Nullable(IRefObservable<long?> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<long?> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<long?>
        {
            private long? _lastValue;

            public _(IRefObserver<long?> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref long? value)
            {
                if (!value.HasValue)
                {
                    return;
                }

                if (_lastValue.HasValue)
                {
                    if (value < _lastValue)
                    {
                        _lastValue = value;
                    }
                }
                else
                {
                    _lastValue = value;
                }
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref _lastValue);
                ForwardOnCompleted();
            }
        }
    }
}
