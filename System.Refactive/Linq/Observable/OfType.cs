// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class OfType<TSource, TResult> : Producer<TResult, OfType<TSource, TResult>._>
    {
        private readonly IRefObservable<TSource> _source;

        public OfType(IRefObservable<TSource> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<TResult> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : Sink<TSource, TResult>
        {
            public _(IRefObserver<TResult> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref TSource value)
            {
                if (value is TResult v)
                {
                    ForwardOnNext(ref v);
                }
            }
        }
    }
}
