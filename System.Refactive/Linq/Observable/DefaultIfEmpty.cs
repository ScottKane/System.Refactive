// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class DefaultIfEmpty<TSource> : Producer<TSource, DefaultIfEmpty<TSource>._>
    {
        private readonly IRefObservable<TSource> _source;
        private readonly TSource _defaultValue;

        public DefaultIfEmpty(IRefObservable<TSource> source, TSource defaultValue)
        {
            _source = source;
            _defaultValue = defaultValue;
        }

        protected override _ CreateSink(IRefObserver<TSource> observer) => new _(_defaultValue, observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<TSource>
        {
            private TSource _defaultValue;
            private bool _found;

            public _(TSource defaultValue, IRefObserver<TSource> observer)
                : base(observer)
            {
                _defaultValue = defaultValue;
                _found = false;
            }

            public override void OnNext(ref TSource value)
            {
                _found = true;
                ForwardOnNext(ref value);
            }

            public override void OnCompleted()
            {
                if (!_found)
                {
                    ForwardOnNext(ref _defaultValue);
                }

                ForwardOnCompleted();
            }
        }
    }
}
