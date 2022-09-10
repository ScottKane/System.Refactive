// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Internal;
using System.Runtime.CompilerServices;

namespace System.Refactive.Linq
{
    internal sealed class All<TSource> : Producer<bool, All<TSource>._>
    {
        private readonly IRefObservable<TSource> _source;
        private readonly Func<TSource, bool> _predicate;

        public All(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            _source = source;
            _predicate = predicate;
        }

        protected override _ CreateSink(IRefObserver<bool> observer) => new _(_predicate, observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : Sink<TSource, bool>
        {
            private readonly Func<TSource, bool> _predicate;

            public _(Func<TSource, bool> predicate, IRefObserver<bool> observer)
                : base(observer)
            {
                _predicate = predicate;
            }

            public override void OnNext(ref TSource value)
            {
                bool res;
                try
                {
                    res = _predicate(value);
                }
                catch (Exception ex)
                {
                    ForwardOnError(ex);
                    return;
                }

                if (!res)
                {
                    ForwardOnNext(ref Unsafe.AsRef(false));
                    ForwardOnCompleted();
                }
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref Unsafe.AsRef(true));
                ForwardOnCompleted();
            }
        }
    }
}
