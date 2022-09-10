// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Disposables;
using System.Refactive.Internal;
using System.Refactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Refactive.Linq
{
    internal sealed class RepeatWhen<T, U> : IRefObservable<T>
    {
        private readonly IRefObservable<T> _source;
        private readonly Func<IRefObservable<object>, IRefObservable<U>> _handler;

        internal RepeatWhen(IRefObservable<T> source, Func<IRefObservable<object>, IRefObservable<U>> handler)
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

            var completeSignals = new Subject<object>();

            IRefObservable<U> redo;

            try
            {
                redo = _handler(completeSignals);
                
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

            var parent = new MainObserver(observer, _source, new RedoSerializedObserver<object>(completeSignals));

            var d = redo.SubscribeSafe(parent.HandlerConsumer);
            parent._handlerUpstream.Disposable = d;

            parent.HandlerNext();

            return parent;
        }

        private sealed class MainObserver : Sink<T>, IRefObserver<T>
        {
            private readonly IRefObservable<T> _source;
            private readonly IRefObserver<object> _completeSignal;

            internal readonly HandlerObserver HandlerConsumer;

            internal SingleAssignmentDisposableValue _handlerUpstream;

            private IDisposable? _upstream;
            private int _trampoline;
            private int _halfSerializer;
            private Exception? _error;

            internal MainObserver(IRefObserver<T> downstream, IRefObservable<T> source, IRefObserver<object> completeSignal) : base(downstream)
            {
                _source = source;
                _completeSignal = completeSignal;
                HandlerConsumer = new HandlerObserver(this);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    Disposable.Dispose(ref _upstream);
                    _handlerUpstream.Dispose();
                }

                base.Dispose(disposing);
            }

            public unsafe void OnCompleted()
            {
                if (Disposable.TrySetSerial(ref _upstream, null))
                {
                    //
                    // NB: Unfortunately this thing slipped in using `object` rather than `Unit`, which is our type used to represent nothing,
                    //     so we have to stick with it and just let a `null` go in here. Users are supposed to ignore the elements produced,
                    //     which `Unit` is making obvious since there's only one value. However, we're stuck here for compat reasons.
                    //

                    _completeSignal.OnNext(ref Unsafe.AsRef<object?>(null)!);
                }
            }

            public void OnError(Exception error)
            {
                HalfSerializer.ForwardOnError(this, error, ref _halfSerializer, ref _error);
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
                        if (Interlocked.CompareExchange(ref _upstream, sad, null) != null)
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

                public void OnCompleted() => _main.HandlerComplete();

                public void OnError(Exception error) => _main.HandlerError(error);

                public void OnNext(ref U value) => _main.HandlerNext();
            }
        }
    }
}
