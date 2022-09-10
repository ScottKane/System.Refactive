// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Internal;
using System.Runtime.CompilerServices;

namespace System.Refactive.Linq
{
    internal sealed class IsEmpty<TSource> : Producer<bool, IsEmpty<TSource>._>
    {
        private readonly IRefObservable<TSource> _source;

        public IsEmpty(IRefObservable<TSource> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<bool> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : Sink<TSource, bool>
        {
            public _(IRefObserver<bool> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref TSource value)
            {
                ForwardOnNext(ref Unsafe.AsRef(false));
                ForwardOnCompleted();
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref Unsafe.AsRef(true));
                ForwardOnCompleted();
            }
        }
    }
}
