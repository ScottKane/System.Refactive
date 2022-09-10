// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Internal;
using System.Runtime.CompilerServices;

namespace System.Refactive.Linq
{
    internal static class FirstOrDefaultAsync<TSource>
    {
        internal sealed class Sequence : Producer<TSource?, Sequence._>
        {
            private readonly IRefObservable<TSource> _source;

            public Sequence(IRefObservable<TSource> source)
            {
                _source = source;
            }

            protected override _ CreateSink(IRefObserver<TSource?> observer) => new _(observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<TSource, TSource?>
            {
                public _(IRefObserver<TSource?> observer)
                    : base(observer)
                {
                }

                public override void OnNext(ref TSource value)
                {
                    ForwardOnNext(ref value);
                    ForwardOnCompleted();
                }

                public override void OnCompleted()
                {
                    ForwardOnNext(ref Unsafe.AsRef(default(TSource)));
                    ForwardOnCompleted();
                }
            }
        }

        internal sealed class Predicate : Producer<TSource?, Predicate._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly Func<TSource, bool> _predicate;

            public Predicate(IRefObservable<TSource> source, Func<TSource, bool> predicate)
            {
                _source = source;
                _predicate = predicate;
            }

            protected override _ CreateSink(IRefObserver<TSource?> observer) => new _(_predicate, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<TSource, TSource?>
            {
                private readonly Func<TSource, bool> _predicate;

                public _(Func<TSource, bool> predicate, IRefObserver<TSource?> observer)
                    : base(observer)
                {
                    _predicate = predicate;
                }

                public override void OnNext(ref TSource value)
                {
                    bool b;

                    try
                    {
                        b = _predicate(value);
                    }
                    catch (Exception ex)
                    {
                        ForwardOnError(ex);
                        return;
                    }

                    if (b)
                    {
                        ForwardOnNext(ref value);
                        ForwardOnCompleted();
                    }
                }

                public override void OnCompleted()
                {
                    ForwardOnNext(ref Unsafe.AsRef(default(TSource)));
                    ForwardOnCompleted();
                }
            }
        }
    }
}
