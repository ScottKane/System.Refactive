// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Concurrent;
using System.Refactive.Disposables;
using System.Refactive.Internal;
using System.Refactive.Subjects;
using System.Threading;

namespace System.Refactive.Linq
{
    internal sealed class RetryWhen<T, U> : IRefObservable<T>
    {
        private readonly IRefObservable<T> _source;
        private readonly Func<IRefObservable<Exception>, IRefObservable<U>> _handler;

        internal RetryWhen(IRefObservable<T> source, Func<IRefObservable<Exception>, IRefObservable<U>> handler)
        {
            _source = source;
            _handler = handler;
        }

        public IDisposable Subscribe(IRefObserver<T> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            var errorSignals = new Subject<Exception>();
            
            IRefObservable<U> redo;

            try
            {
                redo = _handler(errorSignals);

                if (redo == null)
                {
                    throw new NullReferenceException("The handler returned a null IRefObservable");
                }
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
                return Disposable.Empty;
            }

            var parent = new MainObserver(observer, _source, new RedoSerializedObserver<Exception>(errorSignals));

            var d = redo.SubscribeSafe(parent.HandlerConsumer);
            parent.HandlerUpstream.Disposable = d;

            parent.HandlerNext();

            return parent;
        }

        private sealed class MainObserver : Sink<T>, IRefObserver<T>
        {
            private readonly IRefObservable<T> _source;
            private readonly IRefObserver<Exception> _errorSignal;

            internal readonly HandlerObserver HandlerConsumer;
            private IDisposable? _upstream;
            internal SingleAssignmentDisposableValue HandlerUpstream;
            private int _trampoline;
            private int _halfSerializer;
            private Exception? _error;

            internal MainObserver(IRefObserver<T> downstream, IRefObservable<T> source, IRefObserver<Exception> errorSignal) : base(downstream)
            {
                _source = source;
                _errorSignal = errorSignal;
                HandlerConsumer = new HandlerObserver(this);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    Disposable.Dispose(ref _upstream);
                    HandlerUpstream.Dispose();
                }

                base.Dispose(disposing);
            }

            public void OnCompleted()
            {
                HalfSerializer.ForwardOnCompleted(this, ref _halfSerializer, ref _error);
            }

            public void OnError(Exception error)
            {
                if (Disposable.TrySetSerial(ref _upstream, null))
                {
                    _errorSignal.OnNext(ref error);
                }
            }

            public void OnNext(ref T value)
            {
                HalfSerializer.ForwardOnNext(this, value, ref _halfSerializer, ref _error);
            }

            private void HandlerError(Exception error)
            {
                HalfSerializer.ForwardOnError(this, error, ref _halfSerializer, ref _error);
            }

            private void HandlerComplete()
            {
                HalfSerializer.ForwardOnCompleted(this, ref _halfSerializer, ref _error);
            }

            internal void HandlerNext()
            {
                if (Interlocked.Increment(ref _trampoline) == 1)
                {
                    do
                    {
                        var sad = new SingleAssignmentDisposable();
                        if (Disposable.TrySetSingle(ref _upstream, sad) != TrySetSingleResult.Success)
                        {
                            return;
                        }

                        sad.Disposable = _source.SubscribeSafe(this);
                    }
                    while (Interlocked.Decrement(ref _trampoline) != 0);
                }
            }

            internal sealed class HandlerObserver : IRefObserver<U>
            {
                private readonly MainObserver _main;

                internal HandlerObserver(MainObserver main)
                {
                    _main = main;
                }

                public void OnCompleted()
                {
                    _main.HandlerComplete();
                }

                public void OnError(Exception error)
                {
                    _main.HandlerError(error);
                }

                public void OnNext(ref U value)
                {
                    _main.HandlerNext();
                }
            }
        }
    }

    internal sealed class RedoSerializedObserver<X> : IRefObserver<X>
    {
        private static readonly Exception SignaledIndicator = new Exception();

        private readonly IRefObserver<X> _downstream;
        private readonly ConcurrentQueue<X> _queue;

        private int _wip;
        private Exception? _terminalException;

        internal RedoSerializedObserver(IRefObserver<X> downstream)
        {
            _downstream = downstream;
            _queue = new ConcurrentQueue<X>();
        }

        public void OnCompleted()
        {
            if (Interlocked.CompareExchange(ref _terminalException, ExceptionHelper.Terminated, null) == null)
            {
                Drain();
            }
        }

        public void OnError(Exception error)
        {
            if (Interlocked.CompareExchange(ref _terminalException, error, null) == null)
            {
                Drain();
            }
        }

        public void OnNext(ref X value)
        {
            _queue.Enqueue(value);
            Drain();
        }

        private void Clear()
        {
            while (_queue.TryDequeue(out _))
            {
            }
        }

        private void Drain()
        {
            if (Interlocked.Increment(ref _wip) != 1)
            {
                return;
            }

            var missed = 1;

            for (; ; )
            {
                var ex = Volatile.Read(ref _terminalException);
                if (ex != null)
                {
                    if (ex != SignaledIndicator)
                    {
                        Interlocked.Exchange(ref _terminalException, SignaledIndicator);
                        if (ex != ExceptionHelper.Terminated)
                        {
                            _downstream.OnError(ex);
                        }
                        else
                        {
                            _downstream.OnCompleted();
                        }
                    }
                    Clear();
                }
                else
                {
                    while (_queue.TryDequeue(out var item))
                    {
                        _downstream.OnNext(ref item);
                    }
                }


                missed = Interlocked.Add(ref _wip, -missed);
                if (missed == 0)
                {
                    break;
                }
            }
        }
    }
}
