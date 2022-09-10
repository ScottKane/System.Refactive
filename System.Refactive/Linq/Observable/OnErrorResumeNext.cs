// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class OnErrorResumeNext<TSource> : Producer<TSource, OnErrorResumeNext<TSource>._>
    {
        private readonly IEnumerable<IRefObservable<TSource>> _sources;

        public OnErrorResumeNext(IEnumerable<IRefObservable<TSource>> sources)
        {
            _sources = sources;
        }

        protected override _ CreateSink(IRefObserver<TSource> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_sources);

        internal sealed class _ : TailRecursiveSink<TSource>
        {
            public _(IRefObserver<TSource> observer)
                : base(observer)
            {
            }

            protected override IEnumerable<IRefObservable<TSource>>? Extract(IRefObservable<TSource> source)
            {
                if (source is OnErrorResumeNext<TSource> oern)
                {
                    return oern._sources;
                }

                return null;
            }

            public override void OnError(Exception error)
            {
                Recurse();
            }

            public override void OnCompleted()
            {
                Recurse();
            }

            protected override bool Fail(Exception error)
            {
                //
                // Note that the invocation of _recurse in OnError will
                // cause the next MoveNext operation to be enqueued, so
                // we will still return to the caller immediately.
                //
                OnError(error);
                return true;
            }
        }
    }
}
