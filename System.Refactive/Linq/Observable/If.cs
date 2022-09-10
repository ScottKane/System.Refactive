// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class If<TResult> : Producer<TResult, If<TResult>._>, IEvaluatableObservable<TResult>
    {
        private readonly Func<bool> _condition;
        private readonly IRefObservable<TResult> _thenSource;
        private readonly IRefObservable<TResult> _elseSource;

        public If(Func<bool> condition, IRefObservable<TResult> thenSource, IRefObservable<TResult> elseSource)
        {
            _condition = condition;
            _thenSource = thenSource;
            _elseSource = elseSource;
        }

        public IRefObservable<TResult> Eval() => _condition() ? _thenSource : _elseSource;

        protected override _ CreateSink(IRefObserver<TResult> observer) => new _(this, observer);

        protected override void Run(_ sink) => sink.Run();

        internal sealed class _ : IdentitySink<TResult>
        {
            private readonly If<TResult> _parent;

            public _(If<TResult> parent, IRefObserver<TResult> observer)
                : base(observer)
            {
                _parent = parent;
            }

            public void Run()
            {
                IRefObservable<TResult> result;
                try
                {
                    result = _parent.Eval();
                }
                catch (Exception exception)
                {
                    ForwardOnError(exception);

                    return;
                }

                SetUpstream(result.SubscribeSafe(this));
            }
        }
    }
}
