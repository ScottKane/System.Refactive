﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Disposables;
using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class Switch<TSource> : Producer<TSource, Switch<TSource>._>
    {
        private readonly IRefObservable<IRefObservable<TSource>> _sources;

        public Switch(IRefObservable<IRefObservable<TSource>> sources)
        {
            _sources = sources;
        }

        protected override _ CreateSink(IRefObserver<TSource> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_sources);

        internal sealed class _ : Sink<IRefObservable<TSource>, TSource>
        {
            private readonly object _gate = new object();

            public _(IRefObserver<TSource> observer)
                : base(observer)
            {
            }

            private SerialDisposableValue _innerSerialDisposable;
            private bool _isStopped;
            private ulong _latest;
            private bool _hasLatest;

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _innerSerialDisposable.Dispose();
                }

                base.Dispose(disposing);
            }

            public override void OnNext(ref IRefObservable<TSource> value)
            {
                ulong id;
                
                lock (_gate)
                {
                    id = unchecked(++_latest);
                    _hasLatest = true;
                }

                var innerObserver = new InnerObserver(this, id);

                _innerSerialDisposable.Disposable = innerObserver;
                innerObserver.SetResource(value.SubscribeSafe(innerObserver));
            }

            public override void OnError(Exception error)
            {
                lock (_gate)
                {
                    ForwardOnError(error);
                }
            }

            public override void OnCompleted()
            {
                lock (_gate)
                {
                    DisposeUpstream();

                    _isStopped = true;
                    if (!_hasLatest)
                    {
                        ForwardOnCompleted();
                    }
                }
            }

            private sealed class InnerObserver : SafeObserver<TSource>
            {
                private readonly _ _parent;
                private readonly ulong _id;

                public InnerObserver(_ parent, ulong id)
                {
                    _parent = parent;
                    _id = id;
                }

                public override void OnNext(ref TSource value)
                {
                    lock (_parent._gate)
                    {
                        if (_parent._latest == _id)
                        {
                            _parent.ForwardOnNext(ref value);
                        }
                    }
                }

                public override void OnError(Exception error)
                {
                    lock (_parent._gate)
                    {
                        Dispose();

                        if (_parent._latest == _id)
                        {
                            _parent.ForwardOnError(error);
                        }
                    }
                }

                public override void OnCompleted()
                {
                    lock (_parent._gate)
                    {
                        Dispose();

                        if (_parent._latest == _id)
                        {
                            _parent._hasLatest = false;

                            if (_parent._isStopped)
                            {
                                _parent.ForwardOnCompleted();
                            }
                        }
                    }
                }
            }
        }
    }
}
