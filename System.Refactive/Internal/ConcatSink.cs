// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;

namespace System.Refactive.Internal
{
    internal abstract class ConcatSink<TSource> : TailRecursiveSink<TSource>
    {
        protected ConcatSink(IRefObserver<TSource> observer)
            : base(observer)
        {
        }

        protected override IEnumerable<IRefObservable<TSource>>? Extract(IRefObservable<TSource> source) => (source as IConcatenatable<TSource>)?.GetSources();

        public override void OnCompleted() => Recurse();
    }
}
