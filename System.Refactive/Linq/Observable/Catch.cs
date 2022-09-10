﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Disposables;
using System.Refactive.Internal;
using System.Threading;

namespace System.Refactive.Linq
{
    internal sealed class Catch<TSource> : Producer<TSource, Catch<TSource>._>
    {
        private readonly IEnumerable<IRefObservable<TSource>> _sources;

        public Catch(IEnumerable<IRefObservable<TSource>> sources)
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
                if (source is Catch<TSource> @catch)
                {
                    return @catch._sources;
                }

                return null;
            }

            private Exception? _lastException;

            public override void OnError(Exception error)
            {
                _lastException = error;
                Recurse();
            }

            protected override void Done()
            {
                if (_lastException != null)
                {
                    ForwardOnError(_lastException);
                }
                else
                {
                    ForwardOnCompleted();
                }
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

    internal sealed class Catch<TSource, TException> : Producer<TSource, Catch<TSource, TException>._> where TException : Exception
    {
        private readonly IRefObservable<TSource> _source;
        private readonly Func<TException, IRefObservable<TSource>> _handler;

        public Catch(IRefObservable<TSource> source, Func<TException, IRefObservable<TSource>> handler)
        {
            _source = source;
            _handler = handler;
        }

        protected override _ CreateSink(IRefObserver<TSource> observer) => new _(_handler, observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<TSource>
        {
            private readonly Func<TException, IRefObservable<TSource>> _handler;

            public _(Func<TException, IRefObservable<TSource>> handler, IRefObserver<TSource> observer)
                : base(observer)
            {
                _handler = handler;
            }

            private bool _once;
            private SerialDisposableValue _subscription;

            public override void Run(IRefObservable<TSource> source)
            {
                _subscription.TrySetFirst(source.SubscribeSafe(this));
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _subscription.Dispose();
                }

                base.Dispose(disposing);
            }

            public override void OnError(Exception error)
            {
                if (!Volatile.Read(ref _once) && error is TException e)
                {
                    IRefObservable<TSource> result;
                    try
                    {
                        result = _handler(e);
                    }
                    catch (Exception ex)
                    {
                        ForwardOnError(ex);
                        return;
                    }

                    Volatile.Write(ref _once, true);
                    _subscription.Disposable = result.SubscribeSafe(this);
                }
                else
                {
                    ForwardOnError(error);
                }
            }
        }
    }
}
