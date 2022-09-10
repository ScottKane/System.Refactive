// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Disposables;
using System.Refactive.Internal;
using System.Threading;

namespace System.Refactive.Linq
{
    internal sealed class Finally<TSource> : Producer<TSource, Finally<TSource>._>
    {
        private readonly IRefObservable<TSource> _source;
        private readonly Action _finallyAction;

        public Finally(IRefObservable<TSource> source, Action finallyAction)
        {
            _source = source;
            _finallyAction = finallyAction;
        }

        protected override _ CreateSink(IRefObserver<TSource> observer) => new _(_finallyAction, observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<TSource>
        {
            private readonly Action _finallyAction;
            private IDisposable? _sourceDisposable;

            public _(Action finallyAction, IRefObserver<TSource> observer)
                : base(observer)
            {
                _finallyAction = finallyAction;
            }

            public override void Run(IRefObservable<TSource> source)
            {
                var d = source.SubscribeSafe(this);

                if (Interlocked.CompareExchange(ref _sourceDisposable, d, null) == BooleanDisposable.True)
                {
                    // The Dispose(bool) methode was already called before the
                    // subscription could be assign, hence the subscription
                    // needs to be diposed here and the action needs to be invoked.
                    try
                    {
                        d.Dispose();
                    }
                    finally
                    {
                        _finallyAction();
                    }
                }
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    var d = Interlocked.Exchange(ref _sourceDisposable, BooleanDisposable.True);
                    if (d != BooleanDisposable.True && d != null)
                    {
                        try
                        {
                            d.Dispose();
                        }
                        finally
                        {
                            _finallyAction();
                        }
                    }
                }
            }
        }
    }
}
