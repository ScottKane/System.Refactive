﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal sealed class DistinctUntilChanged<TSource, TKey> : Producer<TSource, DistinctUntilChanged<TSource, TKey>._>
    {
        private readonly IRefObservable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly IEqualityComparer<TKey> _comparer;

        public DistinctUntilChanged(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            _source = source;
            _keySelector = keySelector;
            _comparer = comparer;
        }

        protected override _ CreateSink(IRefObserver<TSource> observer) => new _(this, observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<TSource>
        {
            private readonly Func<TSource, TKey> _keySelector;
            private readonly IEqualityComparer<TKey> _comparer;

            private TKey? _currentKey;
            private bool _hasCurrentKey;

            public _(DistinctUntilChanged<TSource, TKey> parent, IRefObserver<TSource> observer)
                : base(observer)
            {
                _keySelector = parent._keySelector;
                _comparer = parent._comparer;
            }

            public override void OnNext(ref TSource value)
            {
                TKey key;
                try
                {
                    key = _keySelector(value);
                }
                catch (Exception exception)
                {
                    ForwardOnError(exception);
                    return;
                }

                var comparerEquals = false;
                if (_hasCurrentKey)
                {
                    try
                    {
                        comparerEquals = _comparer.Equals(_currentKey!, key);
                    }
                    catch (Exception exception)
                    {
                        ForwardOnError(exception);
                        return;
                    }
                }

                if (!_hasCurrentKey || !comparerEquals)
                {
                    _hasCurrentKey = true;
                    _currentKey = key;
                    ForwardOnNext(ref value);
                }
            }
        }
    }
}
