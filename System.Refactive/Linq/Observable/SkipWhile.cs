// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal static class SkipWhile<TSource>
    {
        internal sealed class Predicate : Producer<TSource, Predicate._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly Func<TSource, bool> _predicate;

            public Predicate(IRefObservable<TSource> source, Func<TSource, bool> predicate)
            {
                _source = source;
                _predicate = predicate;
            }

            protected override _ CreateSink(IRefObserver<TSource> observer) => new _(_predicate, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : IdentitySink<TSource>
            {
                private Func<TSource, bool>? _predicate;

                public _(Func<TSource, bool> predicate, IRefObserver<TSource> observer)
                    : base(observer)
                {
                    _predicate = predicate;
                }

                public override void OnNext(ref TSource value)
                {
                    if (_predicate != null)
                    {
                        bool shouldStart;
                        try
                        {
                            shouldStart = !_predicate(value);
                        }
                        catch (Exception exception)
                        {
                            ForwardOnError(exception);
                            return;
                        }

                        if (shouldStart)
                        {
                            _predicate = null;

                            ForwardOnNext(ref value);
                        }
                    }
                    else
                    {
                        ForwardOnNext(ref value);
                    }
                }
            }
        }

        internal sealed class PredicateIndexed : Producer<TSource, PredicateIndexed._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly Func<TSource, int, bool> _predicate;

            public PredicateIndexed(IRefObservable<TSource> source, Func<TSource, int, bool> predicate)
            {
                _source = source;
                _predicate = predicate;
            }

            protected override _ CreateSink(IRefObserver<TSource> observer) => new _(_predicate, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : IdentitySink<TSource>
            {
                private Func<TSource, int, bool>? _predicate;
                private int _index;

                public _(Func<TSource, int, bool> predicate, IRefObserver<TSource> observer)
                    : base(observer)
                {
                    _predicate = predicate;
                }

                public override void OnNext(ref TSource value)
                {
                    if (_predicate != null)
                    {
                        bool shouldStart;
                        try
                        {
                            shouldStart = !_predicate(value, checked(_index++));
                        }
                        catch (Exception exception)
                        {
                            ForwardOnError(exception);
                            return;
                        }

                        if (shouldStart)
                        {
                            _predicate = null;

                            ForwardOnNext(ref value);
                        }
                    }
                    else
                    {
                        ForwardOnNext(ref value);
                    }
                }
            }
        }
    }
}
