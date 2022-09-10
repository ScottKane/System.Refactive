// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Concurrency;
using System.Threading;

namespace System.Refactive.Linq
{
    internal partial class QueryLanguage
    {
        #region + ObserveOn +

        public virtual IRefObservable<TSource> ObserveOn<TSource>(IRefObservable<TSource> source, IScheduler scheduler)
        {
            return Synchronization.ObserveOn(source, scheduler);
        }

        public virtual IRefObservable<TSource> ObserveOn<TSource>(IRefObservable<TSource> source, SynchronizationContext context)
        {
            return Synchronization.ObserveOn(source, context);
        }

        #endregion

        #region + SubscribeOn +

        public virtual IRefObservable<TSource> SubscribeOn<TSource>(IRefObservable<TSource> source, IScheduler scheduler)
        {
            return Synchronization.SubscribeOn(source, scheduler);
        }

        public virtual IRefObservable<TSource> SubscribeOn<TSource>(IRefObservable<TSource> source, SynchronizationContext context)
        {
            return Synchronization.SubscribeOn(source, context);
        }

        #endregion

        #region + Synchronize +

        public virtual IRefObservable<TSource> Synchronize<TSource>(IRefObservable<TSource> source)
        {
            return Synchronization.Synchronize(source);
        }

        public virtual IRefObservable<TSource> Synchronize<TSource>(IRefObservable<TSource> source, object gate)
        {
            return Synchronization.Synchronize(source, gate);
        }

        #endregion
    }
}
