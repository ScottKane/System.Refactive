// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class Max<TSource> : Producer<TSource, Max<TSource>._>
    {
        private readonly IRefObservable<TSource> _source;
        private readonly IComparer<TSource> _comparer;

        public Max(IRefObservable<TSource> source, IComparer<TSource> comparer)
        {
            _source = source;
            _comparer = comparer;
        }

        protected override _ CreateSink(IRefObserver<TSource> observer) => default(TSource) == null ? new Null(_comparer, observer) : new NonNull(_comparer, observer);

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

                    if (comparison > 0)
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

                        if (comparison > 0)
                        {
                            _lastValue = value;
                        }
                    }
                }
            }

            public override void OnError(Exception error)
            {
                ForwardOnError(error);
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref _lastValue!);
                ForwardOnCompleted();
            }
        }
    }

    internal sealed class MaxDouble : Producer<double, MaxDouble._>
    {
        private readonly IRefObservable<double> _source;

        public MaxDouble(IRefObservable<double> source)
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
                    if (value > _lastValue || double.IsNaN(value))
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

    internal sealed class MaxSingle : Producer<float, MaxSingle._>
    {
        private readonly IRefObservable<float> _source;

        public MaxSingle(IRefObservable<float> source)
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
                    if (value > _lastValue || float.IsNaN(value))
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

    internal sealed class MaxDecimal : Producer<decimal, MaxDecimal._>
    {
        private readonly IRefObservable<decimal> _source;

        public MaxDecimal(IRefObservable<decimal> source)
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
                    if (value > _lastValue)
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

    internal sealed class MaxInt32 : Producer<int, MaxInt32._>
    {
        private readonly IRefObservable<int> _source;

        public MaxInt32(IRefObservable<int> source)
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
                    if (value > _lastValue)
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

    internal sealed class MaxInt64 : Producer<long, MaxInt64._>
    {
        private readonly IRefObservable<long> _source;

        public MaxInt64(IRefObservable<long> source)
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
                    if (value > _lastValue)
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

    internal sealed class MaxDoubleNullable : Producer<double?, MaxDoubleNullable._>
    {
        private readonly IRefObservable<double?> _source;

        public MaxDoubleNullable(IRefObservable<double?> source)
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
                    if (value > _lastValue || double.IsNaN((double)value))
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

    internal sealed class MaxSingleNullable : Producer<float?, MaxSingleNullable._>
    {
        private readonly IRefObservable<float?> _source;

        public MaxSingleNullable(IRefObservable<float?> source)
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
                    if (value > _lastValue || float.IsNaN((float)value))
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

    internal sealed class MaxDecimalNullable : Producer<decimal?, MaxDecimalNullable._>
    {
        private readonly IRefObservable<decimal?> _source;

        public MaxDecimalNullable(IRefObservable<decimal?> source)
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
                    if (value > _lastValue)
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

    internal sealed class MaxInt32Nullable : Producer<int?, MaxInt32Nullable._>
    {
        private readonly IRefObservable<int?> _source;

        public MaxInt32Nullable(IRefObservable<int?> source)
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
                    if (value > _lastValue)
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

    internal sealed class MaxInt64Nullable : Producer<long?, MaxInt64Nullable._>
    {
        private readonly IRefObservable<long?> _source;

        public MaxInt64Nullable(IRefObservable<long?> source)
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
                    if (value > _lastValue)
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

