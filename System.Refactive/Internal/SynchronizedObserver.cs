// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

namespace System.Refactive.Internal
{
    internal sealed class SynchronizedObserver<T> : ObserverBase<T>
    {
        private readonly object _gate;
        private readonly IRefObserver<T> _observer;

        public SynchronizedObserver(IRefObserver<T> observer, object gate)
        {
            _gate = gate;
            _observer = observer;
        }

        protected override void OnNextCore(ref T value)
        {
            lock (_gate)
            {
                _observer.OnNext(ref value);
            }
        }

        protected override void OnErrorCore(Exception exception)
        {
            lock (_gate)
            {
                _observer.OnError(exception);
            }
        }

        protected override void OnCompletedCore()
        {
            lock (_gate)
            {
                _observer.OnCompleted();
            }
        }
    }
}
