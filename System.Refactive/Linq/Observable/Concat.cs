// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class Concat<TSource> : Producer<TSource, Concat<TSource>._>, IConcatenatable<TSource>
    {
        private readonly IEnumerable<IRefObservable<TSource>> _sources;

        public Concat(IEnumerable<IRefObservable<TSource>> sources)
        {
            _sources = sources;
        }

        protected override _ CreateSink(IRefObserver<TSource> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_sources);

        public IEnumerable<IRefObservable<TSource>> GetSources() => _sources;

        internal sealed class _ : ConcatSink<TSource>
        {
            public _(IRefObserver<TSource> observer)
                : base(observer)
            {
            }
        }
    }
}
