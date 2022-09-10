// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Internal;
using System.Runtime.CompilerServices;

namespace System.Refactive.Linq
{
    internal sealed class ElementAtOrDefault<TSource> : Producer<TSource?, ElementAtOrDefault<TSource>._>
    {
        private readonly IRefObservable<TSource> _source;
        private readonly int _index;

        public ElementAtOrDefault(IRefObservable<TSource> source, int index)
        {
            _source = source;
            _index = index;
        }

        protected override _ CreateSink(IRefObserver<TSource?> observer) => new _(_index, observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : Sink<TSource, TSource?>
        {
            private int _i;

            public _(int index, IRefObserver<TSource?> observer)
                : base(observer)
            {
                _i = index;
            }

            public override void OnNext(ref TSource value)
            {
                if (_i == 0)
                {
                    ForwardOnNext(ref value);
                    ForwardOnCompleted();
                }

                _i--;
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref Unsafe.AsRef(default(TSource)));
                ForwardOnCompleted();
            }
        }
    }
}
