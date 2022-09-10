// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class Cast<TSource, TResult> : Producer<TResult, Cast<TSource, TResult>._> /* Could optimize further by deriving from Select<TResult> and providing Combine<TResult2>. We're not doing this (yet) for debuggability. */
    {
        private readonly IRefObservable<TSource> _source;

        public Cast(IRefObservable<TSource> source)
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
                TResult result;
                try
                {
                    result = (TResult)((object?)value)!;
                }
                catch (Exception exception)
                {
                    ForwardOnError(exception);
                    return;
                }

                ForwardOnNext(ref result!);
            }
        }
    }
}
