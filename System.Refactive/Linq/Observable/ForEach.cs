// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Threading;

namespace System.Refactive.Linq
{
    internal sealed class ForEach<TSource>
    {
        public abstract class ObserverBase : ManualResetEventSlim, IRefObserver<TSource>
        {
            private Exception? _exception;
            private int _stopped;

            public Exception? Error => _exception;

            protected abstract void OnNextCore(ref TSource value);

            public void OnNext(ref TSource value)
            {
                if (Volatile.Read(ref _stopped) == 0)
                {
                    try
                    {
                        OnNextCore(ref value);
                    }
                    catch (Exception ex)
                    {
                        OnError(ex);
                    }
                }
            }

            public void OnError(Exception error)
            {
                if (Interlocked.Exchange(ref _stopped, 1) == 0)
                {
                    _exception = error;
                    Set();
                }
            }

            public void OnCompleted()
            {
                if (Interlocked.Exchange(ref _stopped, 1) == 0)
                {
                    Set();
                }
            }
        }

        public sealed class Observer : ObserverBase
        {
            private readonly Action<TSource> _onNext;

            public Observer(Action<TSource> onNext) => _onNext = onNext;

            protected override void OnNextCore(ref TSource value) => _onNext(value);
        }

        public sealed class ObserverIndexed : ObserverBase
        {
            private readonly Action<TSource, int> _onNext;

            private int _index;

            public ObserverIndexed(Action<TSource, int> onNext) => _onNext = onNext;

            protected override void OnNextCore(ref TSource value) => _onNext(value, checked(_index++));
        }
    }
}
