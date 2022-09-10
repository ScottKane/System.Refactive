// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Internal;
using System.Runtime.CompilerServices;

namespace System.Refactive.Linq
{
    internal sealed class Contains<TSource> : Producer<bool, Contains<TSource>._>
    {
        private readonly IRefObservable<TSource> _source;
        private readonly TSource _value;
        private readonly IEqualityComparer<TSource> _comparer;

        public Contains(IRefObservable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
        {
            _source = source;
            _value = value;
            _comparer = comparer;
        }

        protected override _ CreateSink(IRefObserver<bool> observer) => new _(this, observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : Sink<TSource, bool>
        {
            private readonly TSource _value;
            private readonly IEqualityComparer<TSource> _comparer;

            public _(Contains<TSource> parent, IRefObserver<bool> observer)
                : base(observer)
            {
                _value = parent._value;
                _comparer = parent._comparer;
            }

            public override void OnNext(ref TSource value)
            {
                bool res;
                try
                {
                    res = _comparer.Equals(value, _value);
                }
                catch (Exception ex)
                {
                    ForwardOnError(ex);
                    return;
                }

                if (res)
                {
                    ForwardOnNext(ref Unsafe.AsRef(true));
                    ForwardOnCompleted();
                }
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref Unsafe.AsRef(false));
                ForwardOnCompleted();
            }
        }
    }
}
