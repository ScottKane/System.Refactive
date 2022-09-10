// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Concurrency;
using System.Refactive.Subjects;

namespace System.Refactive.Linq
{
    internal partial class QueryLanguage
    {
        #region + Multicast +

        public virtual IConnectableObservable<TResult> Multicast<TSource, TResult>(IRefObservable<TSource> source, ISubject<TSource, TResult> subject)
        {
            return new ConnectableObservable<TSource, TResult>(source, subject);
        }

        public virtual IRefObservable<TResult> Multicast<TSource, TIntermediate, TResult>(IRefObservable<TSource> source, Func<ISubject<TSource, TIntermediate>> subjectSelector, Func<IRefObservable<TIntermediate>, IRefObservable<TResult>> selector)
        {
            return new Multicast<TSource, TIntermediate, TResult>(source, subjectSelector, selector);
        }

        #endregion

        #region + Publish +

        public virtual IConnectableObservable<TSource> Publish<TSource>(IRefObservable<TSource> source)
        {
            return source.Multicast(new Subject<TSource>());
        }

        public virtual IRefObservable<TResult> Publish<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector)
        {
            return source.Multicast(() => new Subject<TSource>(), selector);
        }

        public virtual IConnectableObservable<TSource> Publish<TSource>(IRefObservable<TSource> source, TSource initialValue)
        {
            return source.Multicast(new BehaviorSubject<TSource>(initialValue));
        }

        public virtual IRefObservable<TResult> Publish<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector, TSource initialValue)
        {
            return source.Multicast(() => new BehaviorSubject<TSource>(initialValue), selector);
        }

        #endregion

        #region + PublishLast +

        public virtual IConnectableObservable<TSource> PublishLast<TSource>(IRefObservable<TSource> source)
        {
            return source.Multicast(new AsyncSubject<TSource>());
        }

        public virtual IRefObservable<TResult> PublishLast<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector)
        {
            return source.Multicast(() => new AsyncSubject<TSource>(), selector);
        }

        #endregion

        #region + RefCount +

        public virtual IRefObservable<TSource> RefCount<TSource>(IConnectableObservable<TSource> source)
        {
            return RefCount(source, 1);
        }

        public virtual IRefObservable<TSource> RefCount<TSource>(IConnectableObservable<TSource> source, TimeSpan disconnectTime)
        {
            return RefCount(source, disconnectTime, Scheduler.Default);
        }

        public virtual IRefObservable<TSource> RefCount<TSource>(IConnectableObservable<TSource> source, TimeSpan disconnectTime, IScheduler scheduler)
        {
            return RefCount(source, 1, disconnectTime, scheduler);
        }

        public virtual IRefObservable<TSource> RefCount<TSource>(IConnectableObservable<TSource> source, int minObservers)
        {
            return new RefCount<TSource>.Eager(source, minObservers);
        }

        public virtual IRefObservable<TSource> RefCount<TSource>(IConnectableObservable<TSource> source, int minObservers, TimeSpan disconnectTime)
        {
            return RefCount(source, minObservers, disconnectTime, Scheduler.Default);
        }

        public virtual IRefObservable<TSource> RefCount<TSource>(IConnectableObservable<TSource> source, int minObservers, TimeSpan disconnectTime, IScheduler scheduler)
        {
            return new RefCount<TSource>.Lazy(source, disconnectTime, scheduler, minObservers);
        }

        #endregion

        #region + AutoConnect +

        public virtual IRefObservable<TSource> AutoConnect<TSource>(IConnectableObservable<TSource> source, int minObservers = 1, Action<IDisposable>? onConnect = null)
        {
            if (minObservers <= 0)
            {
                var d = source.Connect();
                onConnect?.Invoke(d);
                return source;
            }

            return new AutoConnect<TSource>(source, minObservers, onConnect);
        }


        #endregion

        #region + Replay +

        public virtual IConnectableObservable<TSource> Replay<TSource>(IRefObservable<TSource> source)
        {
            return source.Multicast(new ReplaySubject<TSource>());
        }

        public virtual IConnectableObservable<TSource> Replay<TSource>(IRefObservable<TSource> source, IScheduler scheduler)
        {
            return source.Multicast(new ReplaySubject<TSource>(scheduler));
        }

        public virtual IRefObservable<TResult> Replay<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector)
        {
            return source.Multicast(() => new ReplaySubject<TSource>(), selector);
        }

        public virtual IRefObservable<TResult> Replay<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector, IScheduler scheduler)
        {
            return source.Multicast(() => new ReplaySubject<TSource>(scheduler), selector);
        }

        public virtual IConnectableObservable<TSource> Replay<TSource>(IRefObservable<TSource> source, TimeSpan window)
        {
            return source.Multicast(new ReplaySubject<TSource>(window));
        }

        public virtual IRefObservable<TResult> Replay<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector, TimeSpan window)
        {
            return source.Multicast(() => new ReplaySubject<TSource>(window), selector);
        }

        public virtual IConnectableObservable<TSource> Replay<TSource>(IRefObservable<TSource> source, TimeSpan window, IScheduler scheduler)
        {
            return source.Multicast(new ReplaySubject<TSource>(window, scheduler));
        }

        public virtual IRefObservable<TResult> Replay<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector, TimeSpan window, IScheduler scheduler)
        {
            return source.Multicast(() => new ReplaySubject<TSource>(window, scheduler), selector);
        }

        public virtual IConnectableObservable<TSource> Replay<TSource>(IRefObservable<TSource> source, int bufferSize, IScheduler scheduler)
        {
            return source.Multicast(new ReplaySubject<TSource>(bufferSize, scheduler));
        }

        public virtual IRefObservable<TResult> Replay<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector, int bufferSize, IScheduler scheduler)
        {
            return source.Multicast(() => new ReplaySubject<TSource>(bufferSize, scheduler), selector);
        }

        public virtual IConnectableObservable<TSource> Replay<TSource>(IRefObservable<TSource> source, int bufferSize)
        {
            return source.Multicast(new ReplaySubject<TSource>(bufferSize));
        }

        public virtual IRefObservable<TResult> Replay<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector, int bufferSize)
        {
            return source.Multicast(() => new ReplaySubject<TSource>(bufferSize), selector);
        }

        public virtual IConnectableObservable<TSource> Replay<TSource>(IRefObservable<TSource> source, int bufferSize, TimeSpan window)
        {
            return source.Multicast(new ReplaySubject<TSource>(bufferSize, window));
        }

        public virtual IRefObservable<TResult> Replay<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector, int bufferSize, TimeSpan window)
        {
            return source.Multicast(() => new ReplaySubject<TSource>(bufferSize, window), selector);
        }

        public virtual IConnectableObservable<TSource> Replay<TSource>(IRefObservable<TSource> source, int bufferSize, TimeSpan window, IScheduler scheduler)
        {
            return source.Multicast(new ReplaySubject<TSource>(bufferSize, window, scheduler));
        }

        public virtual IRefObservable<TResult> Replay<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector, int bufferSize, TimeSpan window, IScheduler scheduler)
        {
            return source.Multicast(() => new ReplaySubject<TSource>(bufferSize, window, scheduler), selector);
        }

        #endregion
    }
}
