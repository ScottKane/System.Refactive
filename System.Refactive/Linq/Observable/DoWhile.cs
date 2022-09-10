// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class DoWhile<TSource> : Producer<TSource, DoWhile<TSource>._>, IConcatenatable<TSource>
    {
        private readonly IRefObservable<TSource> _source;
        private readonly Func<bool> _condition;

        public DoWhile(IRefObservable<TSource> source, Func<bool> condition)
        {
            _condition = condition;
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<TSource> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(GetSources());

        public IEnumerable<IRefObservable<TSource>> GetSources()
        {
            yield return _source;
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
