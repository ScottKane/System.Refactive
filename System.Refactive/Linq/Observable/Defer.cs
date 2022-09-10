// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class Defer<TValue> : Producer<TValue, Defer<TValue>._>, IEvaluatableObservable<TValue>
    {
        private readonly Func<IRefObservable<TValue>> _observableFactory;

        public Defer(Func<IRefObservable<TValue>> observableFactory)
        {
            _observableFactory = observableFactory;
        }

        protected override _ CreateSink(IRefObserver<TValue> observer) => new _(_observableFactory, observer);

        protected override void Run(_ sink) => sink.Run();

        public IRefObservable<TValue> Eval() => _observableFactory();

        internal sealed class _ : IdentitySink<TValue>
        {
            private readonly Func<IRefObservable<TValue>> _observableFactory;

            public _(Func<IRefObservable<TValue>> observableFactory, IRefObserver<TValue> observer)
                : base(observer)
            {
                _observableFactory = observableFactory;
            }

            public void Run()
            {
                IRefObservable<TValue> result;
                try
                {
                    result = _observableFactory();
                }
                catch (Exception exception)
                {
                    ForwardOnError(exception);

                    return;
                }

                Run(result);
            }
        }
    }
}
