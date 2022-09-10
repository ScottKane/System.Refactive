// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Linq;
using System.Refactive.Concurrency;
using System.Refactive.Disposables;
using System.Refactive.Internal;
using System.Refactive.Subjects;
using System.Runtime.CompilerServices;

namespace System.Refactive.Linq
{
    internal partial class QueryLanguageEx : IQueryLanguageEx
    {
        #region Create

        public virtual IRefObservable<TResult> Create<TResult>(Func<IRefObserver<TResult>, IEnumerable<IRefObservable<object>>> iteratorMethod)
        {
            return new CreateWithEnumerableObservable<TResult>(iteratorMethod);
        }

        private sealed class CreateWithEnumerableObservable<TResult> : ObservableBase<TResult>
        {
            private readonly Func<IRefObserver<TResult>, IEnumerable<IRefObservable<object>>> _iteratorMethod;

            public CreateWithEnumerableObservable(Func<IRefObserver<TResult>, IEnumerable<IRefObservable<object>>> iteratorMethod)
            {
                _iteratorMethod = iteratorMethod;
            }

            protected override IDisposable SubscribeCore(IRefObserver<TResult> observer)
            {
                return _iteratorMethod(observer)
                    .Concat()
                    .Subscribe(new TerminalOnlyObserver<TResult>(observer));
            }
        }

        private sealed class TerminalOnlyObserver<TResult> : IRefObserver<object>
        {
            private readonly IRefObserver<TResult> _observer;

            public TerminalOnlyObserver(IRefObserver<TResult> observer)
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

            public void OnNext(ref object value)
            {
                // deliberately ignored
            }
        }

        public virtual IRefObservable<Unit> Create(Func<IEnumerable<IRefObservable<object>>> iteratorMethod)
        {
            return new CreateWithOnlyEnumerableObservable<Unit>(iteratorMethod);
        }

        private sealed class CreateWithOnlyEnumerableObservable<TResult> : ObservableBase<TResult>
        {
            private readonly Func<IEnumerable<IRefObservable<object>>> _iteratorMethod;

            public CreateWithOnlyEnumerableObservable(Func<IEnumerable<IRefObservable<object>>> iteratorMethod)
            {
                _iteratorMethod = iteratorMethod;
            }

            protected override IDisposable SubscribeCore(IRefObserver<TResult> observer)
            {
                return _iteratorMethod()
                    .Concat()
                    .Subscribe(new TerminalOnlyObserver<TResult>(observer));
            }
        }

        #endregion

        #region Expand

        public virtual IRefObservable<TSource> Expand<TSource>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TSource>> selector, IScheduler scheduler)
        {
            return new ExpandObservable<TSource>(source, selector, scheduler);
        }

        private sealed class ExpandObservable<TSource> : ObservableBase<TSource>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly Func<TSource, IRefObservable<TSource>> _selector;
            private readonly IScheduler _scheduler;

            public ExpandObservable(IRefObservable<TSource> source, Func<TSource, IRefObservable<TSource>> selector, IScheduler scheduler)
            {
                _source = source;
                _selector = selector;
                _scheduler = scheduler;
            }

            protected override IDisposable SubscribeCore(IRefObserver<TSource> observer)
            {
                var outGate = new object();
                var q = new Queue<IRefObservable<TSource>>();
                var m = new SerialDisposable();
                var d = new CompositeDisposable { m };
                var activeCount = 0;
                var isAcquired = false;

                void ensureActive()
                {
                    var isOwner = false;

                    lock (q)
                    {
                        if (q.Count > 0)
                        {
                            isOwner = !isAcquired;
                            isAcquired = true;
                        }
                    }

                    if (isOwner)
                    {
                        m.Disposable = _scheduler.Schedule(self =>
                        {
                            IRefObservable<TSource> work;

                            lock (q)
                            {
                                if (q.Count > 0)
                                {
                                    work = q.Dequeue();
                                }
                                else
                                {
                                    isAcquired = false;
                                    return;
                                }
                            }

                            var m1 = new SingleAssignmentDisposable();
                            d.Add(m1);
                            m1.Disposable = work.Subscribe(
                                (ref TSource x) =>
                                {
                                    lock (outGate)
                                    {
                                        observer.OnNext(ref x);
                                    }

                                    IRefObservable<TSource> result;
                                    try
                                    {
                                        result = _selector(x);
                                    }
                                    catch (Exception exception)
                                    {
                                        lock (outGate)
                                        {
                                            observer.OnError(exception);
                                        }

                                        return;
                                    }

                                    lock (q)
                                    {
                                        q.Enqueue(result);
                                        activeCount++;
                                    }

                                    ensureActive();
                                },
                                exception =>
                                {
                                    lock (outGate)
                                    {
                                        observer.OnError(exception);
                                    }
                                },
                                () =>
                                {
                                    d.Remove(m1);

                                    var done = false;
                                    lock (q)
                                    {
                                        activeCount--;
                                        if (activeCount == 0)
                                        {
                                            done = true;
                                        }
                                    }
                                    if (done)
                                    {
                                        lock (outGate)
                                        {
                                            observer.OnCompleted();
                                        }
                                    }
                                });
                            self();
                        });
                    }
                }

                lock (q)
                {
                    q.Enqueue(_source);
                    activeCount++;
                }
                ensureActive();

                return d;
            }
        }

        public virtual IRefObservable<TSource> Expand<TSource>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TSource>> selector)
        {
            return source.Expand(selector, SchedulerDefaults.Iteration);
        }

        #endregion

        #region ForkJoin

        public virtual IRefObservable<TResult> ForkJoin<TFirst, TSecond, TResult>(IRefObservable<TFirst> first, IRefObservable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            return Combine<TFirst, TSecond, TResult>(first, second, (observer, leftSubscription, rightSubscription) =>
            {
                var leftStopped = false;
                var rightStopped = false;
                var hasLeft = false;
                var hasRight = false;
                var lastLeft = default(TFirst);
                var lastRight = default(TSecond);

                return new BinaryObserver<TFirst, TSecond>(
                    left =>
                    {
                        switch (left.Kind)
                        {
                            case NotificationKind.OnNext:
                                hasLeft = true;
                                lastLeft = left.Value;
                                break;
                            case NotificationKind.OnError:
                                rightSubscription.Dispose();
                                observer.OnError(left.Exception!);
                                break;
                            case NotificationKind.OnCompleted:
                                leftStopped = true;
                                if (rightStopped)
                                {
                                    if (!hasLeft)
                                    {
                                        observer.OnCompleted();
                                    }
                                    else if (!hasRight)
                                    {
                                        observer.OnCompleted();
                                    }
                                    else
                                    {
                                        TResult result;
                                        try
                                        {
                                            result = resultSelector(lastLeft!, lastRight!);
                                        }
                                        catch (Exception exception)
                                        {
                                            observer.OnError(exception);
                                            return;
                                        }
                                        observer.OnNext(ref result);
                                        observer.OnCompleted();
                                    }
                                }
                                break;
                        }
                    },
                    right =>
                    {
                        switch (right.Kind)
                        {
                            case NotificationKind.OnNext:
                                hasRight = true;
                                lastRight = right.Value;
                                break;
                            case NotificationKind.OnError:
                                leftSubscription.Dispose();
                                observer.OnError(right.Exception!);
                                break;
                            case NotificationKind.OnCompleted:
                                rightStopped = true;
                                if (leftStopped)
                                {
                                    if (!hasLeft)
                                    {
                                        observer.OnCompleted();
                                    }
                                    else if (!hasRight)
                                    {
                                        observer.OnCompleted();
                                    }
                                    else
                                    {
                                        TResult result;
                                        try
                                        {
                                            result = resultSelector(lastLeft!, lastRight!);
                                        }
                                        catch (Exception exception)
                                        {
                                            observer.OnError(exception);
                                            return;
                                        }
                                        observer.OnNext(ref result);
                                        observer.OnCompleted();
                                    }
                                }
                                break;
                        }
                    });
            });
        }

        public virtual IRefObservable<TSource[]> ForkJoin<TSource>(params IRefObservable<TSource>[] sources)
        {
            return sources.ForkJoin();
        }

        public virtual IRefObservable<TSource[]> ForkJoin<TSource>(IEnumerable<IRefObservable<TSource>> sources)
        {
            return new ForkJoinObservable<TSource>(sources);
        }

        private sealed class ForkJoinObservable<TSource> : ObservableBase<TSource[]>
        {
            private readonly IEnumerable<IRefObservable<TSource>> _sources;

            public ForkJoinObservable(IEnumerable<IRefObservable<TSource>> sources)
            {
                _sources = sources;
            }

            protected override IDisposable SubscribeCore(IRefObserver<TSource[]> observer)
            {
                var allSources = _sources.ToArray();
                var count = allSources.Length;

                if (count == 0)
                {
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                var group = new CompositeDisposable(allSources.Length);
                var gate = new object();

                var finished = false;
                var hasResults = new bool[count];
                var hasCompleted = new bool[count];
                var results = new List<TSource>(count);

                lock (gate)
                {
                    for (var index = 0; index < count; index++)
                    {
                        var currentIndex = index;
                        var source = allSources[index];
                        results.Add(default!); // NB: Reserves a space; the default value gets overwritten below.
                        group.Add(source.Subscribe(
                            (ref TSource value) =>
                            {
                                lock (gate)
                                {
                                    if (!finished)
                                    {
                                        hasResults[currentIndex] = true;
                                        results[currentIndex] = value;
                                    }
                                }
                            },
                            error =>
                            {
                                lock (gate)
                                {
                                    finished = true;
                                    observer.OnError(error);
                                    group.Dispose();
                                }
                            },
                            () =>
                            {
                                lock (gate)
                                {
                                    if (!finished)
                                    {
                                        if (!hasResults[currentIndex])
                                        {
                                            observer.OnCompleted();
                                            return;
                                        }
                                        hasCompleted[currentIndex] = true;
                                        foreach (var completed in hasCompleted)
                                        {
                                            if (!completed)
                                            {
                                                return;
                                            }
                                        }
                                        finished = true;
                                        observer.OnNext(ref Unsafe.AsRef(results.ToArray()));
                                        observer.OnCompleted();
                                    }
                                }
                            }));
                    }
                }
                return group;
            }
        }

        #endregion

        #region Let

        public virtual IRefObservable<TResult> Let<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> function)
        {
            return function(source);
        }

        #endregion

        #region ManySelect

        public virtual IRefObservable<TResult> ManySelect<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, TResult> selector)
        {
            return ManySelect(source, selector, DefaultScheduler.Instance);
        }

        public virtual IRefObservable<TResult> ManySelect<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, TResult> selector, IScheduler scheduler)
        {
            return Observable.Defer(() =>
            {
                ChainObservable<TSource>? chain = null;

                return source
                    .Select(
                        x =>
                        {
                            var curr = new ChainObservable<TSource>(x);

                            chain?.OnNext(ref Unsafe.AsRef<IRefObservable<TSource>>(curr));

                            chain = curr;

                            return (IRefObservable<TSource>)curr;
                        })
                    .Do(
                        _ => { },
                        exception =>
                        {
                            chain?.OnError(exception);
                        },
                        () =>
                        {
                            chain?.OnCompleted();
                        })
                    .ObserveOn(scheduler)
                    .Select(selector);
            });
        }

        private class ChainObservable<T> : ISubject<IRefObservable<T>, T>
        {
            private T _head;
            private readonly AsyncSubject<IRefObservable<T>> _tail = new AsyncSubject<IRefObservable<T>>();

            public ChainObservable(T head)
            {
                _head = head;
            }

            public IDisposable Subscribe(IRefObserver<T> observer)
            {
                var g = new CompositeDisposable();
                g.Add(CurrentThreadScheduler.Instance.ScheduleAction((observer, g, @this: this),
                state =>
                {
                    state.observer.OnNext(ref state.@this._head);
                    state.g.Add(state.@this._tail.Merge().Subscribe(state.observer));
                }));
                return g;
            }

            public void OnCompleted()
            {
                OnNext(ref Unsafe.AsRef(Observable.Empty<T>()));
            }

            public void OnError(Exception error)
            {
                OnNext(ref Unsafe.AsRef(Observable.Throw<T>(error)));
            }

            public void OnNext(ref IRefObservable<T> value)
            {
                _tail.OnNext(ref value);
                _tail.OnCompleted();
            }
        }

        #endregion

        #region ToListObservable

        public virtual ListObservable<TSource> ToListObservable<TSource>(IRefObservable<TSource> source)
        {
            return new ListObservable<TSource>(source);
        }

        #endregion

        #region WithLatestFrom

        public virtual IRefObservable<(TFirst First, TSecond Second)> WithLatestFrom<TFirst, TSecond>(IRefObservable<TFirst> first, IRefObservable<TSecond> second)
        {
            return new WithLatestFrom<TFirst, TSecond, (TFirst, TSecond)>(first, second, (t1, t2) => (t1, t2));
        }

        #endregion

        #region Zip

        public virtual IRefObservable<(TFirst First, TSecond Second)> Zip<TFirst, TSecond>(IRefObservable<TFirst> first, IEnumerable<TSecond> second)
        {
            return new Zip<TFirst, TSecond, (TFirst, TSecond)>.Enumerable(first, second, (t1, t2) => (t1, t2));
        }

        #endregion

        #region |> Helpers <|

        private static IRefObservable<TResult> Combine<TLeft, TRight, TResult>(IRefObservable<TLeft> leftSource, IRefObservable<TRight> rightSource, Func<IRefObserver<TResult>, IDisposable, IDisposable, IRefObserver<Either<Notification<TLeft>, Notification<TRight>>>> combinerSelector)
        {
            return new CombineObservable<TLeft, TRight, TResult>(leftSource, rightSource, combinerSelector);
        }

        private sealed class CombineObservable<TLeft, TRight, TResult> : ObservableBase<TResult>
        {
            private readonly IRefObservable<TLeft> _leftSource;
            private readonly IRefObservable<TRight> _rightSource;
            private readonly Func<IRefObserver<TResult>, IDisposable, IDisposable, IRefObserver<Either<Notification<TLeft>, Notification<TRight>>>> _combinerSelector;

            public CombineObservable(IRefObservable<TLeft> leftSource, IRefObservable<TRight> rightSource, Func<IRefObserver<TResult>, IDisposable, IDisposable, IRefObserver<Either<Notification<TLeft>, Notification<TRight>>>> combinerSelector)
            {
                _leftSource = leftSource;
                _rightSource = rightSource;
                _combinerSelector = combinerSelector;
            }

            protected override IDisposable SubscribeCore(IRefObserver<TResult> observer)
            {
                var leftSubscription = new SingleAssignmentDisposable();
                var rightSubscription = new SingleAssignmentDisposable();

                var combiner = _combinerSelector(observer, leftSubscription, rightSubscription);
                var gate = new object();

                leftSubscription.Disposable = _leftSource.Materialize().Select(x => Either<Notification<TLeft>, Notification<TRight>>.CreateLeft(x)).Synchronize(gate).Subscribe(combiner);
                rightSubscription.Disposable = _rightSource.Materialize().Select(x => Either<Notification<TLeft>, Notification<TRight>>.CreateRight(x)).Synchronize(gate).Subscribe(combiner);

                return StableCompositeDisposable.Create(leftSubscription, rightSubscription);

            }
        }

        #endregion
    }
}
