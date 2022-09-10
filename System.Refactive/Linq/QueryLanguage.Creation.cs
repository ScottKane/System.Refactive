// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Concurrency;
using System.Refactive.Disposables;
using System.Refactive.Internal;
using System.Refactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace System.Refactive.Linq
{
    internal partial class QueryLanguage
    {
        #region - Create -

        public virtual IRefObservable<TSource> Create<TSource>(Func<IRefObserver<TSource>, IDisposable> subscribe)
        {
            return new CreateWithDisposableObservable<TSource>(subscribe);
        }

        private sealed class CreateWithDisposableObservable<TSource> : ObservableBase<TSource>
        {
            private readonly Func<IRefObserver<TSource>, IDisposable> _subscribe;

            public CreateWithDisposableObservable(Func<IRefObserver<TSource>, IDisposable> subscribe)
            {
                _subscribe = subscribe;
            }

            protected override IDisposable SubscribeCore(IRefObserver<TSource> observer)
            {
                return _subscribe(observer) ?? Disposable.Empty;
            }
        }

        public virtual IRefObservable<TSource> Create<TSource>(Func<IRefObserver<TSource>, Action> subscribe)
        {
            return new CreateWithActionDisposable<TSource>(subscribe);
        }

        private sealed class CreateWithActionDisposable<TSource> : ObservableBase<TSource>
        {
            private readonly Func<IRefObserver<TSource>, Action> _subscribe;

            public CreateWithActionDisposable(Func<IRefObserver<TSource>, Action> subscribe)
            {
                _subscribe = subscribe;
            }

            protected override IDisposable SubscribeCore(IRefObserver<TSource> observer)
            {
                var a = _subscribe(observer);
                return a != null ? Disposable.Create(a) : Disposable.Empty;
            }
        }

        #endregion

        #region - CreateAsync -

        public virtual IRefObservable<TResult> Create<TResult>(Func<IRefObserver<TResult>, CancellationToken, Task> subscribeAsync)
        {
            return new CreateWithTaskTokenObservable<TResult>(subscribeAsync);
        }

        private sealed class CreateWithTaskTokenObservable<TResult> : ObservableBase<TResult>
        {
            private sealed class Subscription : IDisposable
            {
                private sealed class TaskCompletionObserver : IRefObserver<Unit>
                {
                    private readonly IRefObserver<TResult> _observer;

                    public TaskCompletionObserver(IRefObserver<TResult> observer)
                    {
                        _observer = observer;
                    }

                    public void OnCompleted()
                    {
                        _observer.OnCompleted();
                    }

                    public void OnError(Exception error)
                    {
                        _observer.OnError(error);
                    }

                    public void OnNext(ref Unit value)
                    {
                        // deliberately ignored
                    }
                }

                private readonly IDisposable _subscription;
                private readonly CancellationTokenSource _cts = new CancellationTokenSource();

                public Subscription(Func<IRefObserver<TResult>, CancellationToken, Task> subscribeAsync, IRefObserver<TResult> observer)
                {
                    _subscription = subscribeAsync(observer, _cts.Token)
                        .Subscribe(new TaskCompletionObserver(observer));
                }

                public void Dispose()
                {
                    _cts.Cancel();
                    _subscription.Dispose();
                }
            }

            private readonly Func<IRefObserver<TResult>, CancellationToken, Task> _subscribeAsync;

            public CreateWithTaskTokenObservable(Func<IRefObserver<TResult>, CancellationToken, Task> subscribeAsync)
            {
                _subscribeAsync = subscribeAsync;
            }

            protected override IDisposable SubscribeCore(IRefObserver<TResult> observer)
            {
                return new Subscription(_subscribeAsync, observer);
            }
        }

        public virtual IRefObservable<TResult> Create<TResult>(Func<IRefObserver<TResult>, Task> subscribeAsync)
        {
            return Create<TResult>((observer, token) => subscribeAsync(observer));
        }

        public virtual IRefObservable<TResult> Create<TResult>(Func<IRefObserver<TResult>, CancellationToken, Task<IDisposable>> subscribeAsync)
        {
            return new CreateWithTaskDisposable<TResult>(subscribeAsync);
        }

        private sealed class CreateWithTaskDisposable<TResult> : ObservableBase<TResult>
        {
            private sealed class Subscription : IDisposable
            {
                private sealed class TaskDisposeCompletionObserver : IRefObserver<IDisposable>, IDisposable
                {
                    private readonly IRefObserver<TResult> _observer;
                    private SingleAssignmentDisposableValue _disposable;

                    public TaskDisposeCompletionObserver(IRefObserver<TResult> observer)
                    {
                        _observer = observer;
                    }

                    public void Dispose()
                    {
                        _disposable.Dispose();
                    }

                    public void OnCompleted()
                    {
                    }

                    public void OnError(Exception error)
                    {
                        _observer.OnError(error);
                    }

                    public void OnNext(ref IDisposable value)
                    {
                        _disposable.Disposable = value;
                    }
                }

                private readonly TaskDisposeCompletionObserver _observer;
                private readonly CancellationTokenSource _cts = new CancellationTokenSource();

                public Subscription(Func<IRefObserver<TResult>, CancellationToken, Task<IDisposable>> subscribeAsync, IRefObserver<TResult> observer)
                {
                    //
                    // We don't cancel the subscription below *ever* and want to make sure the returned resource gets disposed eventually.
                    // Notice because we're using the AnonymousObservable<T> type, we get auto-detach behavior for free.
                    //
                    subscribeAsync(observer, _cts.Token)
                        .Subscribe(_observer = new TaskDisposeCompletionObserver(observer));
                }

                public void Dispose()
                {
                    _cts.Cancel();
                    _observer.Dispose();
                }
            }

            private readonly Func<IRefObserver<TResult>, CancellationToken, Task<IDisposable>> _subscribeAsync;

            public CreateWithTaskDisposable(Func<IRefObserver<TResult>, CancellationToken, Task<IDisposable>> subscribeAsync)
            {
                _subscribeAsync = subscribeAsync;
            }

            protected override IDisposable SubscribeCore(IRefObserver<TResult> observer)
            {
                return new Subscription(_subscribeAsync, observer);
            }
        }

        public virtual IRefObservable<TResult> Create<TResult>(Func<IRefObserver<TResult>, Task<IDisposable>> subscribeAsync)
        {
            return Create<TResult>((observer, token) => subscribeAsync(observer));
        }

        public virtual IRefObservable<TResult> Create<TResult>(Func<IRefObserver<TResult>, CancellationToken, Task<Action>> subscribeAsync)
        {
            return new CreateWithTaskActionObservable<TResult>(subscribeAsync);
        }

        private sealed class CreateWithTaskActionObservable<TResult> : ObservableBase<TResult>
        {
            private sealed class Subscription : IDisposable
            {
                private sealed class TaskDisposeCompletionObserver : IRefObserver<Action>, IDisposable
                {
                    private readonly IRefObserver<TResult> _observer;
                    private Action? _disposable;

                    public TaskDisposeCompletionObserver(IRefObserver<TResult> observer)
                    {
                        _observer = observer;
                    }

                    public void Dispose()
                    {
                        Interlocked.Exchange(ref _disposable, Stubs.Nop)?.Invoke();
                    }

                    public void OnCompleted()
                    {
                    }

                    public void OnError(Exception error)
                    {
                        _observer.OnError(error);
                    }

                    public void OnNext(ref Action value)
                    {
                        if (Interlocked.CompareExchange(ref _disposable, value, null) != null)
                        {
                            value?.Invoke();
                        }
                    }
                }

                private readonly TaskDisposeCompletionObserver _observer;
                private readonly CancellationTokenSource _cts = new CancellationTokenSource();

                public Subscription(Func<IRefObserver<TResult>, CancellationToken, Task<Action>> subscribeAsync, IRefObserver<TResult> observer)
                {
                    //
                    // We don't cancel the subscription below *ever* and want to make sure the returned resource gets disposed eventually.
                    // Notice because we're using the AnonymousObservable<T> type, we get auto-detach behavior for free.
                    //
                    subscribeAsync(observer, _cts.Token)
                        .Subscribe(_observer = new TaskDisposeCompletionObserver(observer));
                }

                public void Dispose()
                {
                    _cts.Cancel();
                    _observer.Dispose();
                }
            }

            private readonly Func<IRefObserver<TResult>, CancellationToken, Task<Action>> _subscribeAsync;

            public CreateWithTaskActionObservable(Func<IRefObserver<TResult>, CancellationToken, Task<Action>> subscribeAsync)
            {
                _subscribeAsync = subscribeAsync;
            }

            protected override IDisposable SubscribeCore(IRefObserver<TResult> observer)
            {
                return new Subscription(_subscribeAsync, observer);
            }
        }

        public virtual IRefObservable<TResult> Create<TResult>(Func<IRefObserver<TResult>, Task<Action>> subscribeAsync)
        {
            return Create<TResult>((observer, token) => subscribeAsync(observer));
        }

        #endregion

        #region + Defer +

        public virtual IRefObservable<TValue> Defer<TValue>(Func<IRefObservable<TValue>> observableFactory)
        {
            return new Defer<TValue>(observableFactory);
        }

        #endregion

        #region + DeferAsync +

        public virtual IRefObservable<TValue> Defer<TValue>(Func<Task<IRefObservable<TValue>>> observableFactoryAsync)
        {
            return Defer(() => StartAsync(observableFactoryAsync).Merge());
        }

        public virtual IRefObservable<TValue> Defer<TValue>(Func<CancellationToken, Task<IRefObservable<TValue>>> observableFactoryAsync)
        {
            return Defer(() => StartAsync(observableFactoryAsync).Merge());
        }

        #endregion

        #region + Empty +

        public virtual IRefObservable<TResult> Empty<TResult>()
        {
            return EmptyDirect<TResult>.Instance;
        }

        public virtual IRefObservable<TResult> Empty<TResult>(IScheduler scheduler)
        {
            return new Empty<TResult>(scheduler);
        }

        #endregion

        #region + Generate +

        public virtual IRefObservable<TResult> Generate<TState, TResult>(TState initialState, Func<TState, bool> condition, Func<TState, TState> iterate, Func<TState, TResult> resultSelector)
        {
            return new Generate<TState, TResult>.NoTime(initialState, condition, iterate, resultSelector, SchedulerDefaults.Iteration);
        }

        public virtual IRefObservable<TResult> Generate<TState, TResult>(TState initialState, Func<TState, bool> condition, Func<TState, TState> iterate, Func<TState, TResult> resultSelector, IScheduler scheduler)
        {
            return new Generate<TState, TResult>.NoTime(initialState, condition, iterate, resultSelector, scheduler);
        }

        #endregion

        #region + Never +

        public virtual IRefObservable<TResult> Never<TResult>()
        {
            return Linq.Never<TResult>.Default;
        }

        #endregion

        #region + Range +

        public virtual IRefObservable<int> Range(int start, int count)
        {
            return Range_(start, count, SchedulerDefaults.Iteration);
        }

        public virtual IRefObservable<int> Range(int start, int count, IScheduler scheduler)
        {
            return Range_(start, count, scheduler);
        }

        private static IRefObservable<int> Range_(int start, int count, IScheduler scheduler)
        {
            var longRunning = scheduler.AsLongRunning();
            if (longRunning != null)
            {
                return new RangeLongRunning(start, count, longRunning);
            }
            return new RangeRecursive(start, count, scheduler);
        }

        #endregion

        #region + Repeat +

        public virtual IRefObservable<TResult> Repeat<TResult>(TResult value)
        {
            return Repeat_(value, SchedulerDefaults.Iteration);
        }

        public virtual IRefObservable<TResult> Repeat<TResult>(TResult value, IScheduler scheduler)
        {
            return Repeat_(value, scheduler);
        }

        private static IRefObservable<TResult> Repeat_<TResult>(TResult value, IScheduler scheduler)
        {
            var longRunning = scheduler.AsLongRunning();
            if (longRunning != null)
            {
                return new Repeat<TResult>.ForeverLongRunning(value, longRunning);
            }
            return new Repeat<TResult>.ForeverRecursive(value, scheduler);
        }

        public virtual IRefObservable<TResult> Repeat<TResult>(TResult value, int repeatCount)
        {
            return Repeat_(value, repeatCount, SchedulerDefaults.Iteration);
        }

        public virtual IRefObservable<TResult> Repeat<TResult>(TResult value, int repeatCount, IScheduler scheduler)
        {
            return Repeat_(value, repeatCount, scheduler);
        }

        private static IRefObservable<TResult> Repeat_<TResult>(TResult value, int repeatCount, IScheduler scheduler)
        {
            var longRunning = scheduler.AsLongRunning();
            if (longRunning != null)
            {
                return new Repeat<TResult>.CountLongRunning(value, repeatCount, longRunning);
            }
            return new Repeat<TResult>.CountRecursive(value, repeatCount, scheduler);
        }

        #endregion

        #region + Return +

        public virtual IRefObservable<TResult> Return<TResult>(TResult value)
        {
            // ConstantTimeOperations is a mutable field so we'd have to
            // check if it points to the immediate scheduler instance
            // which is done in the other Return overload anyway
            return Return(value, SchedulerDefaults.ConstantTimeOperations);
        }

        public virtual IRefObservable<TResult> Return<TResult>(TResult value, IScheduler scheduler)
        {
            if (scheduler == ImmediateScheduler.Instance)
            {
                return new ReturnImmediate<TResult>(value);
            }
            return new Return<TResult>(value, scheduler);
        }

        #endregion

        #region + Throw +

        public virtual IRefObservable<TResult> Throw<TResult>(Exception exception)
        {
            // ConstantTimeOperations is a mutable field so we'd have to
            // check if it points to the immediate scheduler instance
            // which is done in the other Return overload anyway
            return Throw<TResult>(exception, SchedulerDefaults.ConstantTimeOperations);
        }

        public virtual IRefObservable<TResult> Throw<TResult>(Exception exception, IScheduler scheduler)
        {
            if (scheduler == ImmediateScheduler.Instance)
            {
                return new ThrowImmediate<TResult>(exception);
            }
            return new Throw<TResult>(exception, scheduler);
        }

        #endregion

        #region + Using +

        public virtual IRefObservable<TSource> Using<TSource, TResource>(Func<TResource> resourceFactory, Func<TResource, IRefObservable<TSource>> observableFactory) where TResource : IDisposable
        {
            return new Using<TSource, TResource>(resourceFactory, observableFactory);
        }

        #endregion

        #region - UsingAsync -

        public virtual IRefObservable<TSource> Using<TSource, TResource>(Func<CancellationToken, Task<TResource>> resourceFactoryAsync, Func<TResource, CancellationToken, Task<IRefObservable<TSource>>> observableFactoryAsync) where TResource : IDisposable
        {
            return Observable.FromAsync(resourceFactoryAsync)
                .SelectMany(resource =>
                    Observable.Using(
                        () => resource,
                        resource_ => Observable.FromAsync(ct => observableFactoryAsync(resource_, ct)).Merge()
                    )
                );
        }

        #endregion
    }
}
