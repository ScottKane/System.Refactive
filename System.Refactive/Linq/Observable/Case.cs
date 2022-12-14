// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class Case<TValue, TResult> : Producer<TResult, Case<TValue, TResult>._>, IEvaluatableObservable<TResult>
        where TValue : notnull
    {
        private readonly Func<TValue> _selector;
        private readonly IDictionary<TValue, IRefObservable<TResult>> _sources;
        private readonly IRefObservable<TResult> _defaultSource;

        public Case(Func<TValue> selector, IDictionary<TValue, IRefObservable<TResult>> sources, IRefObservable<TResult> defaultSource)
        {
            _selector = selector;
            _sources = sources;
            _defaultSource = defaultSource;
        }

        public IRefObservable<TResult> Eval()
        {
            if (_sources.TryGetValue(_selector(), out var res))
            {
                return res;
            }

            return _defaultSource;
        }

        protected override _ CreateSink(IRefObserver<TResult> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(this);

        internal sealed class _ : IdentitySink<TResult>
        {
            public _(IRefObserver<TResult> observer)
                : base(observer)
            {
            }

            public void Run(Case<TValue, TResult> parent)
            {
                IRefObservable<TResult> result;
                try
                {
                    result = parent.Eval();
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
