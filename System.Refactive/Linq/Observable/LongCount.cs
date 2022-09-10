// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal static class LongCount<TSource>
    {
        internal sealed class All : Producer<long, All._>
        {
            private readonly IRefObservable<TSource> _source;

            public All(IRefObservable<TSource> source)
            {
                _source = source;
            }

            protected override _ CreateSink(IRefObserver<long> observer) => new _(observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<TSource, long>
            {
                private long _count;

                public _(IRefObserver<long> observer)
                    : base(observer)
                {
                }

                public override void OnNext(ref TSource value)
                {
                    try
                    {
                        checked
                        {
                            _count++;
                        }
                    }
                    catch (Exception ex)
                    {
                        ForwardOnError(ex);
                    }
                }

                public override void OnCompleted()
                {
                    ForwardOnNext(ref _count);
                    ForwardOnCompleted();
                }
            }
        }

        internal sealed class Predicate : Producer<long, Predicate._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly Func<TSource, bool> _predicate;

            public Predicate(IRefObservable<TSource> source, Func<TSource, bool> predicate)
            {
                _source = source;
                _predicate = predicate;
            }

            protected override _ CreateSink(IRefObserver<long> observer) => new _(_predicate, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<TSource, long>
            {
                private readonly Func<TSource, bool> _predicate;
                private long _count;

                public _(Func<TSource, bool> predicate, IRefObserver<long> observer)
                    : base(observer)
                {
                    _predicate = predicate;
                }

                public override void OnNext(ref TSource value)
                {
                    try
                    {
                        checked
                        {
                            if (_predicate(value))
                            {
                                _count++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ForwardOnError(ex);
                    }
                }

                public override void OnCompleted()
                {
                    ForwardOnNext(ref _count);
                    ForwardOnCompleted();
                }
            }
        }
    }
}
