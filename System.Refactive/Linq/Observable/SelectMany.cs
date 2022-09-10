// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Concurrency;
using System.Refactive.Disposables;
using System.Refactive.Internal;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Refactive.Linq
{
    internal static class SelectMany<TSource, TCollection, TResult>
    {
        internal sealed class ObservableSelector : Producer<TResult, ObservableSelector._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly Func<TSource, IRefObservable<TCollection>> _collectionSelector;
            private readonly Func<TSource, TCollection, TResult> _resultSelector;

            public ObservableSelector(IRefObservable<TSource> source, Func<TSource, IRefObservable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
            {
                _source = source;
                _collectionSelector = collectionSelector;
                _resultSelector = resultSelector;
            }

            protected override _ CreateSink(IRefObserver<TResult> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<TSource, TResult>
            {
                private readonly object _gate = new object();
                private readonly CompositeDisposable _group = new CompositeDisposable();

                private readonly Func<TSource, IRefObservable<TCollection>> _collectionSelector;
                private readonly Func<TSource, TCollection, TResult> _resultSelector;

                public _(ObservableSelector parent, IRefObserver<TResult> observer)
                    : base(observer)
                {
                    _collectionSelector = parent._collectionSelector;
                    _resultSelector = parent._resultSelector;
                }

                private volatile bool _isStopped;

                public override void OnNext(ref TSource value)
                {
                    IRefObservable<TCollection> collection;

                    try
                    {
                        collection = _collectionSelector(value);
                    }
                    catch (Exception ex)
                    {
                        lock (_gate)
                        {
                            ForwardOnError(ex);
                        }
                        return;
                    }

                    var innerObserver = new InnerObserver(this, value);
                    _group.Add(innerObserver);
                    innerObserver.SetResource(collection.SubscribeSafe(innerObserver));
                }

                public override void OnError(Exception error)
                {
                    lock (_gate)
                    {
                        ForwardOnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    _isStopped = true;
                    if (_group.Count == 0)
                    {
                        //
                        // Notice there can be a race between OnCompleted of the source and any
                        // of the inner sequences, where both see _group.Count == 1, and one is
                        // waiting for the lock. There won't be a double OnCompleted observation
                        // though, because the call to Dispose silences the observer by swapping
                        // in a NopObserver<T>.
                        //
                        lock (_gate)
                        {
                            ForwardOnCompleted();
                        }
                    }
                    else
                    {
                        DisposeUpstream();
                    }
                }

                protected override void Dispose(bool disposing)
                {
                    base.Dispose(disposing);

                    if (disposing)
                    {
                        _group.Dispose();
                    }
                }

                private sealed class InnerObserver : SafeObserver<TCollection>
                {
                    private readonly _ _parent;
                    private readonly TSource _value;

                    public InnerObserver(_ parent, TSource value)
                    {
                        _parent = parent;
                        _value = value;
                    }

                    public override void OnNext(ref TCollection value)
                    {
                        TResult res;

                        try
                        {
                            res = _parent._resultSelector(_value, value);
                        }
                        catch (Exception ex)
                        {
                            lock (_parent._gate)
                            {
                                _parent.ForwardOnError(ex);
                            }
                            return;
                        }

                        lock (_parent._gate)
                        {
                            _parent.ForwardOnNext(ref res);
                        }
                    }

                    public override void OnError(Exception error)
                    {
                        lock (_parent._gate)
                        {
                            _parent.ForwardOnError(error);
                        }
                    }

                    public override void OnCompleted()
                    {
                        _parent._group.Remove(this);
                        if (_parent._isStopped && _parent._group.Count == 0)
                        {
                            //
                            // Notice there can be a race between OnCompleted of the source and any
                            // of the inner sequences, where both see _group.Count == 1, and one is
                            // waiting for the lock. There won't be a double OnCompleted observation
                            // though, because the call to Dispose silences the observer by swapping
                            // in a NopObserver<T>.
                            //
                            lock (_parent._gate)
                            {
                                _parent.ForwardOnCompleted();
                            }
                        }
                    }
                }
            }
        }

        internal sealed class ObservableSelectorIndexed : Producer<TResult, ObservableSelectorIndexed._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly Func<TSource, int, IRefObservable<TCollection>> _collectionSelector;
            private readonly Func<TSource, int, TCollection, int, TResult> _resultSelector;

            public ObservableSelectorIndexed(IRefObservable<TSource> source, Func<TSource, int, IRefObservable<TCollection>> collectionSelector, Func<TSource, int, TCollection, int, TResult> resultSelector)
            {
                _source = source;
                _collectionSelector = collectionSelector;
                _resultSelector = resultSelector;
            }

            protected override _ CreateSink(IRefObserver<TResult> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<TSource, TResult>
            {
                private readonly object _gate = new object();
                private readonly CompositeDisposable _group = new CompositeDisposable();

                private readonly Func<TSource, int, IRefObservable<TCollection>> _collectionSelector;
                private readonly Func<TSource, int, TCollection, int, TResult> _resultSelector;

                public _(ObservableSelectorIndexed parent, IRefObserver<TResult> observer)
                    : base(observer)
                {
                    _collectionSelector = parent._collectionSelector;
                    _resultSelector = parent._resultSelector;
                }

                private volatile bool _isStopped;
                private int _index;

                public override void OnNext(ref TSource value)
                {
                    int index;
                    IRefObservable<TCollection> collection;

                    try
                    {
                        index = checked(_index++);
                        collection = _collectionSelector(value, index);
                    }
                    catch (Exception ex)
                    {
                        lock (_gate)
                        {
                            ForwardOnError(ex);
                        }
                        return;
                    }

                    var innerObserver = new InnerObserver(this, value, index);
                    _group.Add(innerObserver);
                    innerObserver.SetResource(collection.SubscribeSafe(innerObserver));
                }

                public override void OnError(Exception error)
                {
                    lock (_gate)
                    {
                        ForwardOnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    _isStopped = true;
                    if (_group.Count == 0)
                    {
                        //
                        // Notice there can be a race between OnCompleted of the source and any
                        // of the inner sequences, where both see _group.Count == 1, and one is
                        // waiting for the lock. There won't be a double OnCompleted observation
                        // though, because the call to Dispose silences the observer by swapping
                        // in a NopObserver<T>.
                        //
                        lock (_gate)
                        {
                            ForwardOnCompleted();
                        }
                    }
                    else
                    {
                        DisposeUpstream();
                    }
                }

                protected override void Dispose(bool disposing)
                {
                    base.Dispose(disposing);

                    if (disposing)
                    {
                        _group.Dispose();
                    }
                }

                private sealed class InnerObserver : SafeObserver<TCollection>
                {
                    private readonly _ _parent;
                    private readonly TSource _value;
                    private readonly int _valueIndex;

                    public InnerObserver(_ parent, TSource value, int index)
                    {
                        _parent = parent;
                        _value = value;
                        _valueIndex = index;
                    }

                    private int _index;

                    public override void OnNext(ref TCollection value)
                    {
                        TResult res;

                        try
                        {
                            res = _parent._resultSelector(_value, _valueIndex, value, checked(_index++));
                        }
                        catch (Exception ex)
                        {
                            lock (_parent._gate)
                            {
                                _parent.ForwardOnError(ex);
                            }
                            return;
                        }

                        lock (_parent._gate)
                        {
                            _parent.ForwardOnNext(ref res);
                        }
                    }

                    public override void OnError(Exception error)
                    {
                        lock (_parent._gate)
                        {
                            _parent.ForwardOnError(error);
                        }
                    }

                    public override void OnCompleted()
                    {
                        _parent._group.Remove(this);
                        if (_parent._isStopped && _parent._group.Count == 0)
                        {
                            //
                            // Notice there can be a race between OnCompleted of the source and any
                            // of the inner sequences, where both see _group.Count == 1, and one is
                            // waiting for the lock. There won't be a double OnCompleted observation
                            // though, because the call to Dispose silences the observer by swapping
                            // in a NopObserver<T>.
                            //
                            lock (_parent._gate)
                            {
                                _parent.ForwardOnCompleted();
                            }
                        }
                    }
                }
            }
        }

        internal sealed class EnumerableSelector : Producer<TResult, EnumerableSelector._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly Func<TSource, IEnumerable<TCollection>> _collectionSelector;
            private readonly Func<TSource, TCollection, TResult> _resultSelector;

            public EnumerableSelector(IRefObservable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
            {
                _source = source;
                _collectionSelector = collectionSelector;
                _resultSelector = resultSelector;
            }

            protected override _ CreateSink(IRefObserver<TResult> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<TSource, TResult>
            {
                private readonly Func<TSource, IEnumerable<TCollection>> _collectionSelector;
                private readonly Func<TSource, TCollection, TResult> _resultSelector;

                public _(EnumerableSelector parent, IRefObserver<TResult> observer)
                    : base(observer)
                {
                    _collectionSelector = parent._collectionSelector;
                    _resultSelector = parent._resultSelector;
                }

                public override void OnNext(ref TSource value)
                {
                    IEnumerable<TCollection> xs;
                    try
                    {
                        xs = _collectionSelector(value);
                    }
                    catch (Exception exception)
                    {
                        ForwardOnError(exception);
                        return;
                    }

                    IEnumerator<TCollection> e;
                    try
                    {
                        e = xs.GetEnumerator();
                    }
                    catch (Exception exception)
                    {
                        ForwardOnError(exception);
                        return;
                    }

                    using (e)
                    {
                        var hasNext = true;

                        while (hasNext)
                        {
                            var current = default(TResult);

                            try
                            {
                                hasNext = e.MoveNext();
                                if (hasNext)
                                {
                                    current = _resultSelector(value, e.Current);
                                }
                            }
                            catch (Exception exception)
                            {
                                ForwardOnError(exception);
                                return;
                            }

                            if (hasNext)
                            {
                                ForwardOnNext(ref current!);
                            }
                        }
                    }
                }
            }
        }

        internal sealed class EnumerableSelectorIndexed : Producer<TResult, EnumerableSelectorIndexed._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly Func<TSource, int, IEnumerable<TCollection>> _collectionSelector;
            private readonly Func<TSource, int, TCollection, int, TResult> _resultSelector;

            public EnumerableSelectorIndexed(IRefObservable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, int, TCollection, int, TResult> resultSelector)
            {
                _source = source;
                _collectionSelector = collectionSelector;
                _resultSelector = resultSelector;
            }

            protected override _ CreateSink(IRefObserver<TResult> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<TSource, TResult>
            {
                private readonly Func<TSource, int, IEnumerable<TCollection>> _collectionSelector;
                private readonly Func<TSource, int, TCollection, int, TResult> _resultSelector;

                public _(EnumerableSelectorIndexed parent, IRefObserver<TResult> observer)
                    : base(observer)
                {
                    _collectionSelector = parent._collectionSelector;
                    _resultSelector = parent._resultSelector;
                }

                private int _index;

                public override void OnNext(ref TSource value)
                {
                    int index;

                    IEnumerable<TCollection> xs;
                    try
                    {
                        index = checked(_index++);
                        xs = _collectionSelector(value, index);
                    }
                    catch (Exception exception)
                    {
                        ForwardOnError(exception);
                        return;
                    }

                    IEnumerator<TCollection> e;
                    try
                    {
                        e = xs.GetEnumerator();
                    }
                    catch (Exception exception)
                    {
                        ForwardOnError(exception);
                        return;
                    }

                    using (e)
                    {
                        var eIndex = 0;
                        var hasNext = true;

                        while (hasNext)
                        {
                            var current = default(TResult);

                            try
                            {
                                hasNext = e.MoveNext();
                                if (hasNext)
                                {
                                    current = _resultSelector(value, index, e.Current, checked(eIndex++));
                                }
                            }
                            catch (Exception exception)
                            {
                                ForwardOnError(exception);
                                return;
                            }

                            if (hasNext)
                            {
                                ForwardOnNext(ref current!);
                            }
                        }
                    }
                }
            }
        }

        internal sealed class TaskSelector : Producer<TResult, TaskSelector._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly Func<TSource, CancellationToken, Task<TCollection>> _collectionSelector;
            private readonly Func<TSource, TCollection, TResult> _resultSelector;

            public TaskSelector(IRefObservable<TSource> source, Func<TSource, CancellationToken, Task<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
            {
                _source = source;
                _collectionSelector = collectionSelector;
                _resultSelector = resultSelector;
            }

            protected override _ CreateSink(IRefObserver<TResult> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<TSource, TResult>
            {
                private readonly object _gate = new object();
                private readonly CancellationTokenSource _cancel = new CancellationTokenSource();

                private readonly Func<TSource, CancellationToken, Task<TCollection>> _collectionSelector;
                private readonly Func<TSource, TCollection, TResult> _resultSelector;

                public _(TaskSelector parent, IRefObserver<TResult> observer)
                    : base(observer)
                {
                    _collectionSelector = parent._collectionSelector;
                    _resultSelector = parent._resultSelector;
                }

                private volatile int _count;

                public override void Run(IRefObservable<TSource> source)
                {
                    _count = 1;

                    base.Run(source);
                }

                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        _cancel.Cancel();
                    }
                    base.Dispose(disposing);
                }

                public override void OnNext(ref TSource value)
                {
                    Task<TCollection> task;
                    try
                    {
                        Interlocked.Increment(ref _count);
                        task = _collectionSelector(value, _cancel.Token);
                    }
                    catch (Exception ex)
                    {
                        lock (_gate)
                        {
                            ForwardOnError(ex);
                        }

                        return;
                    }

                    if (task.IsCompleted)
                    {
                        OnCompletedTask(value, task);
                    }
                    else
                    {
                        task.ContinueWithState(static (t, tuple) => tuple.@this.OnCompletedTask(tuple.value, t), (@this: this, value), _cancel.Token);
                    }
                }

                private void OnCompletedTask(TSource value, Task<TCollection> task)
                {
                    switch (task.Status)
                    {
                        case TaskStatus.RanToCompletion:
                        {
                            TResult res;
                            try
                            {
                                res = _resultSelector(value, task.Result);
                            }
                            catch (Exception ex)
                            {
                                lock (_gate)
                                {
                                    ForwardOnError(ex);
                                }

                                return;
                            }

                            lock (_gate)
                            {
                                ForwardOnNext(ref res);
                            }

                            OnCompleted();

                            break;
                        }
                        
                        case TaskStatus.Faulted:
                        {
                            lock (_gate)
                            {
                                ForwardOnError(TaskHelpers.GetSingleException(task));
                            }

                            break;
                        }
                        case TaskStatus.Canceled:
                        {
                            if (!_cancel.IsCancellationRequested)
                            {
                                lock (_gate)
                                {
                                    ForwardOnError(new TaskCanceledException(task));
                                }
                            }

                            break;
                        }
                    }
                }

                public override void OnError(Exception error)
                {
                    lock (_gate)
                    {
                        ForwardOnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    if (Interlocked.Decrement(ref _count) == 0)
                    {
                        lock (_gate)
                        {
                            ForwardOnCompleted();
                        }
                    }
                }
            }
        }

        internal sealed class TaskSelectorIndexed : Producer<TResult, TaskSelectorIndexed._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly Func<TSource, int, CancellationToken, Task<TCollection>> _collectionSelector;
            private readonly Func<TSource, int, TCollection, TResult> _resultSelector;

            public TaskSelectorIndexed(IRefObservable<TSource> source, Func<TSource, int, CancellationToken, Task<TCollection>> collectionSelector, Func<TSource, int, TCollection, TResult> resultSelector)
            {
                _source = source;
                _collectionSelector = collectionSelector;
                _resultSelector = resultSelector;
            }

            protected override _ CreateSink(IRefObserver<TResult> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<TSource, TResult>
            {
                private readonly object _gate = new object();
                private readonly CancellationTokenSource _cancel = new CancellationTokenSource();

                private readonly Func<TSource, int, CancellationToken, Task<TCollection>> _collectionSelector;
                private readonly Func<TSource, int, TCollection, TResult> _resultSelector;

                public _(TaskSelectorIndexed parent, IRefObserver<TResult> observer)
                    : base(observer)
                {
                    _collectionSelector = parent._collectionSelector;
                    _resultSelector = parent._resultSelector;
                }

                private volatile int _count;
                private int _index;

                public override void Run(IRefObservable<TSource> source)
                {
                    _count = 1;

                    base.Run(source);
                }

                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        _cancel.Cancel();
                    }
                    base.Dispose(disposing);
                }

                public override void OnNext(ref TSource value)
                {
                    int index;

                    Task<TCollection> task;
                    try
                    {
                        index = checked(_index++);
                        Interlocked.Increment(ref _count);
                        task = _collectionSelector(value, index, _cancel.Token);
                    }
                    catch (Exception ex)
                    {
                        lock (_gate)
                        {
                            ForwardOnError(ex);
                        }

                        return;
                    }

                    if (task.IsCompleted)
                    {
                        OnCompletedTask(value, index, task);
                    }
                    else
                    {
                        task.ContinueWithState(static (t, tuple) => tuple.@this.OnCompletedTask(tuple.value, tuple.index, t), (@this: this, value, index), _cancel.Token);
                    }
                }

                private void OnCompletedTask(TSource value, int index, Task<TCollection> task)
                {
                    switch (task.Status)
                    {
                        case TaskStatus.RanToCompletion:
                        {
                            TResult res;
                            try
                            {
                                res = _resultSelector(value, index, task.Result);
                            }
                            catch (Exception ex)
                            {
                                lock (_gate)
                                {
                                    ForwardOnError(ex);
                                }

                                return;
                            }

                            lock (_gate)
                            {
                                ForwardOnNext(ref res);
                            }

                            OnCompleted();

                            break;
                        }
                        case TaskStatus.Faulted:
                        {
                            lock (_gate)
                            {
                                ForwardOnError(TaskHelpers.GetSingleException(task));
                            }

                            break;
                        }
                        case TaskStatus.Canceled:
                        {
                            if (!_cancel.IsCancellationRequested)
                            {
                                lock (_gate)
                                {
                                    ForwardOnError(new TaskCanceledException(task));
                                }
                            }

                            break;
                        }
                    }
                }

                public override void OnError(Exception error)
                {
                    lock (_gate)
                    {
                        ForwardOnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    if (Interlocked.Decrement(ref _count) == 0)
                    {
                        lock (_gate)
                        {
                            ForwardOnCompleted();
                        }
                    }
                }
            }
        }
    }

    internal static class SelectMany<TSource, TResult>
    {
        internal class ObservableSelector : Producer<TResult, ObservableSelector._>
        {
            protected readonly IRefObservable<TSource> _source;
            protected readonly Func<TSource, IRefObservable<TResult>> _selector;

            public ObservableSelector(IRefObservable<TSource> source, Func<TSource, IRefObservable<TResult>> selector)
            {
                _source = source;
                _selector = selector;
            }

            protected override _ CreateSink(IRefObserver<TResult> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal class _ : Sink<TSource, TResult>
            {
                protected readonly object _gate = new object();
                private readonly Func<TSource, IRefObservable<TResult>> _selector;
                private readonly CompositeDisposable _group = new CompositeDisposable();

                private volatile bool _isStopped;

                public _(ObservableSelector parent, IRefObserver<TResult> observer)
                    : base(observer)
                {
                    _selector = parent._selector;
                }

                public override void OnNext(ref TSource value)
                {
                    IRefObservable<TResult> inner;

                    try
                    {
                        inner = _selector(value);
                    }
                    catch (Exception ex)
                    {
                        lock (_gate)
                        {
                            ForwardOnError(ex);
                        }
                        return;
                    }

                    SubscribeInner(inner);
                }

                public override void OnError(Exception error)
                {
                    lock (_gate)
                    {
                        ForwardOnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    Final();
                }

                protected override void Dispose(bool disposing)
                {
                    base.Dispose(disposing);

                    if (disposing)
                    {
                        _group.Dispose();
                    }
                }

                protected void Final()
                {
                    _isStopped = true;
                    if (_group.Count == 0)
                    {
                        //
                        // Notice there can be a race between OnCompleted of the source and any
                        // of the inner sequences, where both see _group.Count == 0, and one is
                        // waiting for the lock. There won't be a double OnCompleted observation
                        // though, because the call to Dispose silences the observer by swapping
                        // in a NopObserver<T>.
                        //
                        lock (_gate)
                        {
                            ForwardOnCompleted();
                        }
                    }
                    else
                    {
                        DisposeUpstream();
                    }
                }

                protected void SubscribeInner(IRefObservable<TResult> inner)
                {
                    var innerObserver = new InnerObserver(this);

                    _group.Add(innerObserver);
                    innerObserver.SetResource(inner.SubscribeSafe(innerObserver));
                }

                private sealed class InnerObserver : SafeObserver<TResult>
                {
                    private readonly _ _parent;

                    public InnerObserver(_ parent)
                    {
                        _parent = parent;
                    }

                    public override void OnNext(ref TResult value)
                    {
                        lock (_parent._gate)
                        {
                            _parent.ForwardOnNext(ref value);
                        }
                    }

                    public override void OnError(Exception error)
                    {
                        lock (_parent._gate)
                        {
                            _parent.ForwardOnError(error);
                        }
                    }

                    public override void OnCompleted()
                    {
                        _parent._group.Remove(this);
                        if (_parent._isStopped && _parent._group.Count == 0)
                        {
                            //
                            // Notice there can be a race between OnCompleted of the source and any
                            // of the inner sequences, where both see _group.Count == 1, and one is
                            // waiting for the lock. There won't be a double OnCompleted observation
                            // though, because the call to Dispose silences the observer by swapping
                            // in a NopObserver<T>.
                            //
                            lock (_parent._gate)
                            {
                                _parent.ForwardOnCompleted();
                            }
                        }
                    }
                }
            }
        }

        internal sealed class ObservableSelectors : ObservableSelector
        {
            private readonly Func<Exception, IRefObservable<TResult>> _selectorOnError;
            private readonly Func<IRefObservable<TResult>> _selectorOnCompleted;

            public ObservableSelectors(IRefObservable<TSource> source, Func<TSource, IRefObservable<TResult>> selector, Func<Exception, IRefObservable<TResult>> selectorOnError, Func<IRefObservable<TResult>> selectorOnCompleted)
                : base(source, selector)
            {
                _selectorOnError = selectorOnError;
                _selectorOnCompleted = selectorOnCompleted;
            }

            protected override ObservableSelector._ CreateSink(IRefObserver<TResult> observer) => new _(this, observer);

            internal new sealed class _ : ObservableSelector._
            {
                private readonly Func<Exception, IRefObservable<TResult>> _selectorOnError;
                private readonly Func<IRefObservable<TResult>> _selectorOnCompleted;

                public _(ObservableSelectors parent, IRefObserver<TResult> observer)
                    : base(parent, observer)
                {
                    _selectorOnError = parent._selectorOnError;
                    _selectorOnCompleted = parent._selectorOnCompleted;
                }

                public override void OnError(Exception error)
                {
                    if (_selectorOnError != null)
                    {
                        IRefObservable<TResult> inner;

                        try
                        {
                            inner = _selectorOnError(error);
                        }
                        catch (Exception ex)
                        {
                            lock (_gate)
                            {
                                ForwardOnError(ex);
                            }
                            return;
                        }

                        SubscribeInner(inner);

                        Final();
                    }
                    else
                    {
                        base.OnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    if (_selectorOnCompleted != null)
                    {
                        IRefObservable<TResult> inner;

                        try
                        {
                            inner = _selectorOnCompleted();
                        }
                        catch (Exception ex)
                        {
                            lock (_gate)
                            {
                                ForwardOnError(ex);
                            }
                            return;
                        }

                        SubscribeInner(inner);
                    }

                    Final();
                }
            }
        }

        internal class ObservableSelectorIndexed : Producer<TResult, ObservableSelectorIndexed._>
        {
            protected readonly IRefObservable<TSource> _source;
            protected readonly Func<TSource, int, IRefObservable<TResult>> _selector;

            public ObservableSelectorIndexed(IRefObservable<TSource> source, Func<TSource, int, IRefObservable<TResult>> selector)
            {
                _source = source;
                _selector = selector;
            }

            protected override _ CreateSink(IRefObserver<TResult> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal class _ : Sink<TSource, TResult>
            {
                private readonly object _gate = new object();
                private readonly CompositeDisposable _group = new CompositeDisposable();

                protected readonly Func<TSource, int, IRefObservable<TResult>> _selector;

                private int _index;
                private volatile bool _isStopped;

                public _(ObservableSelectorIndexed parent, IRefObserver<TResult> observer)
                    : base(observer)
                {
                    _selector = parent._selector;
                }

                public override void OnNext(ref TSource value)
                {
                    IRefObservable<TResult> inner;

                    try
                    {
                        inner = _selector(value, checked(_index++));
                    }
                    catch (Exception ex)
                    {
                        lock (_gate)
                        {
                            ForwardOnError(ex);
                        }
                        return;
                    }

                    SubscribeInner(inner);
                }

                public override void OnError(Exception error)
                {
                    lock (_gate)
                    {
                        ForwardOnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    Final();
                }

                protected override void Dispose(bool disposing)
                {
                    base.Dispose(disposing);

                    if (disposing)
                    {
                        _group.Dispose();
                    }
                }

                protected void Final()
                {
                    _isStopped = true;
                    if (_group.Count == 0)
                    {
                        //
                        // Notice there can be a race between OnCompleted of the source and any
                        // of the inner sequences, where both see _group.Count == 1, and one is
                        // waiting for the lock. There won't be a double OnCompleted observation
                        // though, because the call to Dispose silences the observer by swapping
                        // in a NopObserver<T>.
                        //
                        lock (_gate)
                        {
                            ForwardOnCompleted();
                        }
                    }
                    else
                    {
                        DisposeUpstream();
                    }
                }

                protected void SubscribeInner(IRefObservable<TResult> inner)
                {
                    var innerObserver = new InnerObserver(this);

                    _group.Add(innerObserver);
                    innerObserver.SetResource(inner.SubscribeSafe(innerObserver));
                }

                private sealed class InnerObserver : SafeObserver<TResult>
                {
                    private readonly _ _parent;

                    public InnerObserver(_ parent)
                    {
                        _parent = parent;
                    }

                    public override void OnNext(ref TResult value)
                    {
                        lock (_parent._gate)
                        {
                            _parent.ForwardOnNext(ref value);
                        }
                    }

                    public override void OnError(Exception error)
                    {
                        lock (_parent._gate)
                        {
                            _parent.ForwardOnError(error);
                        }
                    }

                    public override void OnCompleted()
                    {
                        _parent._group.Remove(this);
                        if (_parent._isStopped && _parent._group.Count == 0)
                        {
                            //
                            // Notice there can be a race between OnCompleted of the source and any
                            // of the inner sequences, where both see _group.Count == 1, and one is
                            // waiting for the lock. There won't be a double OnCompleted observation
                            // though, because the call to Dispose silences the observer by swapping
                            // in a NopObserver<T>.
                            //
                            lock (_parent._gate)
                            {
                                _parent.ForwardOnCompleted();
                            }
                        }
                    }
                }
            }
        }

        internal sealed class ObservableSelectorsIndexed : ObservableSelectorIndexed
        {
            private readonly Func<Exception, IRefObservable<TResult>> _selectorOnError;
            private readonly Func<IRefObservable<TResult>> _selectorOnCompleted;

            public ObservableSelectorsIndexed(IRefObservable<TSource> source, Func<TSource, int, IRefObservable<TResult>> selector, Func<Exception, IRefObservable<TResult>> selectorOnError, Func<IRefObservable<TResult>> selectorOnCompleted)
                : base(source, selector)
            {
                _selectorOnError = selectorOnError;
                _selectorOnCompleted = selectorOnCompleted;
            }

            protected override ObservableSelectorIndexed._ CreateSink(IRefObserver<TResult> observer) => new _(this, observer);

            internal new sealed class _ : ObservableSelectorIndexed._
            {
                private readonly object _gate = new object();

                private readonly Func<Exception, IRefObservable<TResult>> _selectorOnError;
                private readonly Func<IRefObservable<TResult>> _selectorOnCompleted;

                public _(ObservableSelectorsIndexed parent, IRefObserver<TResult> observer)
                    : base(parent, observer)
                {
                    _selectorOnError = parent._selectorOnError;
                    _selectorOnCompleted = parent._selectorOnCompleted;
                }

                public override void OnError(Exception error)
                {
                    if (_selectorOnError != null)
                    {
                        IRefObservable<TResult> inner;

                        try
                        {
                            inner = _selectorOnError(error);
                        }
                        catch (Exception ex)
                        {
                            lock (_gate)
                            {
                                ForwardOnError(ex);
                            }
                            return;
                        }

                        SubscribeInner(inner);

                        Final();
                    }
                    else
                    {
                        base.OnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    if (_selectorOnCompleted != null)
                    {
                        IRefObservable<TResult> inner;

                        try
                        {
                            inner = _selectorOnCompleted();
                        }
                        catch (Exception ex)
                        {
                            lock (_gate)
                            {
                                ForwardOnError(ex);
                            }
                            return;
                        }

                        SubscribeInner(inner);
                    }

                    Final();
                }
            }
        }

        internal sealed class EnumerableSelector : Producer<TResult, EnumerableSelector._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly Func<TSource, IEnumerable<TResult>> _selector;

            public EnumerableSelector(IRefObservable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
            {
                _source = source;
                _selector = selector;
            }

            protected override _ CreateSink(IRefObserver<TResult> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<TSource, TResult>
            {
                private readonly Func<TSource, IEnumerable<TResult>> _selector;

                public _(EnumerableSelector parent, IRefObserver<TResult> observer)
                    : base(observer)
                {
                    _selector = parent._selector;
                }

                public override void OnNext(ref TSource value)
                {
                    IEnumerable<TResult> xs;
                    try
                    {
                        xs = _selector(value);
                    }
                    catch (Exception exception)
                    {
                        ForwardOnError(exception);
                        return;
                    }

                    IEnumerator<TResult> e;
                    try
                    {
                        e = xs.GetEnumerator();
                    }
                    catch (Exception exception)
                    {
                        ForwardOnError(exception);
                        return;
                    }

                    using (e)
                    {
                        var hasNext = true;

                        while (hasNext)
                        {
                            var current = default(TResult);

                            try
                            {
                                hasNext = e.MoveNext();
                                if (hasNext)
                                {
                                    current = e.Current;
                                }
                            }
                            catch (Exception exception)
                            {
                                ForwardOnError(exception);
                                return;
                            }

                            if (hasNext)
                            {
                                ForwardOnNext(ref current!);
                            }
                        }
                    }
                }
            }
        }

        internal sealed class EnumerableSelectorIndexed : Producer<TResult, EnumerableSelectorIndexed._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly Func<TSource, int, IEnumerable<TResult>> _selector;

            public EnumerableSelectorIndexed(IRefObservable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
            {
                _source = source;
                _selector = selector;
            }

            protected override _ CreateSink(IRefObserver<TResult> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<TSource, TResult>
            {
                private readonly Func<TSource, int, IEnumerable<TResult>> _selector;

                public _(EnumerableSelectorIndexed parent, IRefObserver<TResult> observer)
                    : base(observer)
                {
                    _selector = parent._selector;
                }

                private int _index;

                public override void OnNext(ref TSource value)
                {
                    IEnumerable<TResult> xs;
                    try
                    {
                        xs = _selector(value, checked(_index++));
                    }
                    catch (Exception exception)
                    {
                        ForwardOnError(exception);
                        return;
                    }

                    IEnumerator<TResult> e;
                    try
                    {
                        e = xs.GetEnumerator();
                    }
                    catch (Exception exception)
                    {
                        ForwardOnError(exception);
                        return;
                    }

                    using (e)
                    {
                        var hasNext = true;

                        while (hasNext)
                        {
                            var current = default(TResult);

                            try
                            {
                                hasNext = e.MoveNext();
                                if (hasNext)
                                {
                                    current = e.Current;
                                }
                            }
                            catch (Exception exception)
                            {
                                ForwardOnError(exception);
                                return;
                            }

                            if (hasNext)
                            {
                                ForwardOnNext(ref current!);
                            }
                        }
                    }
                }
            }
        }

        internal sealed class TaskSelector : Producer<TResult, TaskSelector._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly Func<TSource, CancellationToken, Task<TResult>> _selector;

            public TaskSelector(IRefObservable<TSource> source, Func<TSource, CancellationToken, Task<TResult>> selector)
            {
                _source = source;
                _selector = selector;
            }

            protected override _ CreateSink(IRefObserver<TResult> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<TSource, TResult>
            {
                private readonly object _gate = new object();
                private readonly CancellationTokenSource _cts = new CancellationTokenSource();

                private readonly Func<TSource, CancellationToken, Task<TResult>> _selector;

                public _(TaskSelector parent, IRefObserver<TResult> observer)
                    : base(observer)
                {
                    _selector = parent._selector;
                }

                private volatile int _count;

                public override void Run(IRefObservable<TSource> source)
                {
                    _count = 1;

                    base.Run(source);
                }

                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        _cts.Cancel();
                    }
                    base.Dispose(disposing);
                }

                public override void OnNext(ref TSource value)
                {
                    Task<TResult> task;
                    try
                    {
                        Interlocked.Increment(ref _count);
                        task = _selector(value, _cts.Token);
                    }
                    catch (Exception ex)
                    {
                        lock (_gate)
                        {
                            ForwardOnError(ex);
                        }

                        return;
                    }

                    if (task.IsCompleted)
                    {
                        OnCompletedTask(task);
                    }
                    else
                    {
                        task.ContinueWith((closureTask, thisObject) => ((_)thisObject!).OnCompletedTask(closureTask), this, _cts.Token);
                    }
                }

                private void OnCompletedTask(Task<TResult> task)
                {
                    switch (task.Status)
                    {
                        case TaskStatus.RanToCompletion:
                        {
                            lock (_gate)
                            {
                                ForwardOnNext(ref Unsafe.AsRef(task.Result));
                            }

                            OnCompleted();

                            break;
                        }
                        case TaskStatus.Faulted:
                        {
                            lock (_gate)
                            {
                                ForwardOnError(TaskHelpers.GetSingleException(task));
                            }

                            break;
                        }
                        case TaskStatus.Canceled:
                        {
                            if (!_cts.IsCancellationRequested)
                            {
                                lock (_gate)
                                {
                                    ForwardOnError(new TaskCanceledException(task));
                                }
                            }

                            break;
                        }
                    }
                }

                public override void OnError(Exception error)
                {
                    lock (_gate)
                    {
                        ForwardOnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    if (Interlocked.Decrement(ref _count) == 0)
                    {
                        lock (_gate)
                        {
                            ForwardOnCompleted();
                        }
                    }
                }
            }
        }

        internal sealed class TaskSelectorIndexed : Producer<TResult, TaskSelectorIndexed._>
        {
            private readonly IRefObservable<TSource> _source;
            private readonly Func<TSource, int, CancellationToken, Task<TResult>> _selector;

            public TaskSelectorIndexed(IRefObservable<TSource> source, Func<TSource, int, CancellationToken, Task<TResult>> selector)
            {
                _source = source;
                _selector = selector;
            }

            protected override _ CreateSink(IRefObserver<TResult> observer) => new _(this, observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<TSource, TResult>
            {
                private readonly object _gate = new object();
                private readonly CancellationTokenSource _cts = new CancellationTokenSource();

                private readonly Func<TSource, int, CancellationToken, Task<TResult>> _selector;

                public _(TaskSelectorIndexed parent, IRefObserver<TResult> observer)
                    : base(observer)
                {
                    _selector = parent._selector;
                }

                private volatile int _count;
                private int _index;

                public override void Run(IRefObservable<TSource> source)
                {
                    _count = 1;

                    base.Run(source);
                }

                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        _cts.Cancel();
                    }
                    base.Dispose(disposing);
                }

                public override void OnNext(ref TSource value)
                {
                    Task<TResult> task;
                    try
                    {
                        Interlocked.Increment(ref _count);
                        task = _selector(value, checked(_index++), _cts.Token);
                    }
                    catch (Exception ex)
                    {
                        lock (_gate)
                        {
                            ForwardOnError(ex);
                        }

                        return;
                    }

                    if (task.IsCompleted)
                    {
                        OnCompletedTask(task);
                    }
                    else
                    {
                        task.ContinueWith((closureTask, thisObject) => ((_)thisObject!).OnCompletedTask(closureTask), this, _cts.Token);
                    }
                }

                private void OnCompletedTask(Task<TResult> task)
                {
                    switch (task.Status)
                    {
                        case TaskStatus.RanToCompletion:
                        {
                            lock (_gate)
                            {
                                ForwardOnNext(ref Unsafe.AsRef(task.Result));
                            }

                            OnCompleted();

                            break;
                        }
                        case TaskStatus.Faulted:
                        {
                            lock (_gate)
                            {
                                ForwardOnError(TaskHelpers.GetSingleException(task));
                            }

                            break;
                        }
                        case TaskStatus.Canceled:
                        {
                            if (!_cts.IsCancellationRequested)
                            {
                                lock (_gate)
                                {
                                    ForwardOnError(new TaskCanceledException(task));
                                }
                            }

                            break;
                        }
                    }
                }

                public override void OnError(Exception error)
                {
                    lock (_gate)
                    {
                        ForwardOnError(error);
                    }
                }

                public override void OnCompleted()
                {
                    if (Interlocked.Decrement(ref _count) == 0)
                    {
                        lock (_gate)
                        {
                            ForwardOnCompleted();
                        }
                    }
                }
            }
        }
    }
}
