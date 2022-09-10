// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Concurrency;
using System.Refactive.Internal;
using System.Runtime.CompilerServices;

namespace System.Refactive.Linq
{
    internal partial class QueryLanguage
    {
        #region + Subscribe +

        public virtual IDisposable Subscribe<TSource>(IEnumerable<TSource> source, IRefObserver<TSource> observer)
        {
            return Subscribe_(source, observer, SchedulerDefaults.Iteration);
        }

        public virtual IDisposable Subscribe<TSource>(IEnumerable<TSource> source, IRefObserver<TSource> observer, IScheduler scheduler)
        {
            return Subscribe_(source, observer, scheduler);
        }

        private static IDisposable Subscribe_<TSource>(IEnumerable<TSource> source, IRefObserver<TSource> observer, IScheduler scheduler)
        {
            var longRunning = scheduler.AsLongRunning();
            if (longRunning != null)
            {
                //
                // [OK] Use of unsafe Subscribe: we're calling into a known producer implementation.
                //
                return new ToObservableLongRunning<TSource>(source, longRunning).Subscribe/*Unsafe*/(observer);
            }
            //
            // [OK] Use of unsafe Subscribe: we're calling into a known producer implementation.
            //
            return new ToObservableRecursive<TSource>(source, scheduler).Subscribe/*Unsafe*/(observer);
        }

        #endregion

        #region + ToEnumerable +

        public virtual IEnumerable<TSource> ToEnumerable<TSource>(IRefObservable<TSource> source)
        {
            return new AnonymousEnumerable<TSource>(() => source.GetEnumerator());
        }

        #endregion

        #region ToEvent

        public virtual IEventSource<Unit> ToEvent(IRefObservable<Unit> source)
        {
            return new EventSource<Unit>(source, (h, _) => h(Unit.Default));
        }

        public virtual IEventSource<TSource> ToEvent<TSource>(IRefObservable<TSource> source)
        {
            return new EventSource<TSource>(source, (h, value) => h(value));
        }

        #endregion

        #region ToEventPattern

        #endregion

        #region + ToObservable +

        public virtual IRefObservable<TSource> ToObservable<TSource>(IEnumerable<TSource> source)
        {
            return ToObservable_(source, SchedulerDefaults.Iteration);
        }

        public virtual IRefObservable<TSource> ToObservable<TSource>(IEnumerable<TSource> source, IScheduler scheduler)
        {
            return ToObservable_(source, scheduler);
        }

        private static IRefObservable<TSource> ToObservable_<TSource>(IEnumerable<TSource> source, IScheduler scheduler)
        {
            var longRunning = scheduler.AsLongRunning();
            if (longRunning != null)
            {
                //
                // [OK] Use of unsafe Subscribe: we're calling into a known producer implementation.
                //
                return new ToObservableLongRunning<TSource>(source, longRunning);
            }
            //
            // [OK] Use of unsafe Subscribe: we're calling into a known producer implementation.
            //
            return new ToObservableRecursive<TSource>(source, scheduler);
        }

        #endregion
    }
}
