// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class AsObservable<TSource> : Producer<TSource, AsObservable<TSource>._>, IEvaluatableObservable<TSource>
    {
        private readonly IRefObservable<TSource> _source;

        public AsObservable(IRefObservable<TSource> source)
        {
            _source = source;
        }

        public IRefObservable<TSource> Eval() => _source;

        protected override _ CreateSink(IRefObserver<TSource> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<TSource>
        {
            public _(IRefObserver<TSource> observer)
                : base(observer)
            {
            }
        }
    }
}
