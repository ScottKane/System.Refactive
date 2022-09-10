// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class While<TSource> : Producer<TSource, While<TSource>._>, IConcatenatable<TSource>
    {
        private readonly Func<bool> _condition;
        private readonly IRefObservable<TSource> _source;

        public While(Func<bool> condition, IRefObservable<TSource> source)
        {
            _condition = condition;
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<TSource> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(GetSources());

        public IEnumerable<IRefObservable<TSource>> GetSources()
        {
            while (_condition())
            {
                yield return _source;
            }
        }

        internal sealed class _ : ConcatSink<TSource>
        {
            public _(IRefObserver<TSource> observer)
                : base(observer)
            {
            }
        }
    }
}
