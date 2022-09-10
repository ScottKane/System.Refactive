// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class For<TSource, TResult> : Producer<TResult, For<TSource, TResult>._>, IConcatenatable<TResult>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, IRefObservable<TResult>> _resultSelector;

        public For(IEnumerable<TSource> source, Func<TSource, IRefObservable<TResult>> resultSelector)
        {
            _source = source;
            _resultSelector = resultSelector;
        }

        protected override _ CreateSink(IRefObserver<TResult> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(GetSources());

        public IEnumerable<IRefObservable<TResult>> GetSources()
        {
            foreach (var item in _source)
            {
                yield return _resultSelector(item);
            }
        }

        internal sealed class _ : ConcatSink<TResult>
        {
            public _(IRefObserver<TResult> observer)
                : base(observer)
            {
            }
        }
    }
}
