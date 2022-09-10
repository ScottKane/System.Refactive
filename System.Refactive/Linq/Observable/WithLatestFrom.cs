﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Disposables;
using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class WithLatestFrom<TFirst, TSecond, TResult> : Producer<TResult, WithLatestFrom<TFirst, TSecond, TResult>._>
    {
        private readonly IRefObservable<TFirst> _first;
        private readonly IRefObservable<TSecond> _second;
        private readonly Func<TFirst, TSecond, TResult> _resultSelector;

        public WithLatestFrom(IRefObservable<TFirst> first, IRefObservable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            _first = first;
            _second = second;
            _resultSelector = resultSelector;
        }

        protected override _ CreateSink(IRefObserver<TResult> observer) => new _(_resultSelector, observer);

        protected override void Run(_ sink) => sink.Run(_first, _second);

        internal sealed class _ : IdentitySink<TResult>
        {
            private readonly object _gate = new object();
            private readonly object _latestGate = new object();

            private readonly Func<TFirst, TSecond, TResult> _resultSelector;

            public _(Func<TFirst, TSecond, TResult> resultSelector, IRefObserver<TResult> observer)
                : base(observer)
            {
                _resultSelector = resultSelector;
            }

            private volatile bool _hasLatest;
            private TSecond? _latest;

            private SingleAssignmentDisposableValue _secondDisposable;

            public void Run(IRefObservable<TFirst> first, IRefObservable<TSecond> second)
            {
                var fstO = new FirstObserver(this);
                var sndO = new SecondObserver(this);

                _secondDisposable.Disposable = second.SubscribeSafe(sndO);
                SetUpstream(first.SubscribeSafe(fstO));
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _secondDisposable.Dispose();
                }

                base.Dispose(disposing);
            }

            private sealed class FirstObserver : IRefObserver<TFirst>
            {
                private readonly _ _parent;

                public FirstObserver(_ parent)
                {
                    _parent = parent;
                }

                public void OnCompleted()
                {
                    lock (_parent._gate)
                    {
                        _parent.ForwardOnCompleted();
                    }
                }

                public void OnError(Exception error)
                {
                    lock (_parent._gate)
                    {
                        _parent.ForwardOnError(error);
                    }
                }

                public void OnNext(ref TFirst value)
                {
                    if (_parent._hasLatest) // Volatile read
                    {
                        TSecond latest;

                        lock (_parent._latestGate)
                        {
                            latest = _parent._latest!; // NB: Not null when hasLatest is true.
                        }

                        TResult res;

                        try
                        {
                            res = _parent._resultSelector(value, latest);
                        }
                        catch (Exception ex)
                        {
                            lock (_parent._gate)
                            {
                                _parent.ForwardOnError(ex);
                            }

                            return;
                        }

                        lock (_parent._gate)
                        {
                            _parent.ForwardOnNext(ref res);
                        }
                    }
                }
            }

            private sealed class SecondObserver : IRefObserver<TSecond>
            {
                private readonly _ _parent;

                public SecondObserver(_ parent)
                {
                    _parent = parent;
                }

                public void OnCompleted()
                {
                    _parent._secondDisposable.Dispose();
                }

                public void OnError(Exception error)
                {
                    lock (_parent._gate)
                    {
                        _parent.ForwardOnError(error);
                    }
                }

                public void OnNext(ref TSecond value)
                {
                    lock (_parent._latestGate)
                    {
                        _parent._latest = value;
                    }

                    if (!_parent._hasLatest)
                    {
                        _parent._hasLatest = true;
                    }
                }
            }
        }
    }
}