﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal static class LastOrDefaultAsync<TSource>
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
                private TSource? _value;

                public _(IRefObserver<TSource?> observer)
                    : base(observer)
                {

                }

                public override void OnNext(ref TSource value)
                {
                    _value = value;
                }

                public override void OnError(Exception error)
                {
                    _value = default;
                    ForwardOnError(error);
                }

                public override void OnCompleted()
                {
                    var value = _value;
                    _value = default;
                    ForwardOnNext(ref value);
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
                private TSource? _value;

                public _(Func<TSource, bool> predicate, IRefObserver<TSource?> observer)
                    : base(observer)
                {
                    _predicate = predicate;
                }

                public override void OnNext(ref TSource value)
                {
                    var b = false;

                    try
                    {
                        b = _predicate(value);
                    }
                    catch (Exception ex)
                    {
                        _value = default;
                        ForwardOnError(ex);
                        return;
                    }

                    if (b)
                    {
                        _value = value;
                    }
                }

                public override void OnError(Exception error)
                {
                    _value = default;
                    ForwardOnError(error);
                }

                public override void OnCompleted()
                {
                    var value = _value;
                    _value = default;
                    ForwardOnNext(ref value);
                    ForwardOnCompleted();
                }
            }
        }
    }
}