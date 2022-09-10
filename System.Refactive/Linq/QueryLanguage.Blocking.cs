// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal partial class QueryLanguage
    {
        #region - Chunkify -

        public virtual IEnumerable<IList<TSource>> Chunkify<TSource>(IRefObservable<TSource> source)
        {
            return source.Collect<TSource, IList<TSource>>(() => new List<TSource>(), (lst, x) => { lst.Add(x); return lst; }, _ => new List<TSource>());
        }

        #endregion

        #region + Collect +

        public virtual IEnumerable<TResult> Collect<TSource, TResult>(IRefObservable<TSource> source, Func<TResult> newCollector, Func<TResult, TSource, TResult> merge)
        {
            return Collect_(source, newCollector, merge, _ => newCollector());
        }

        public virtual IEnumerable<TResult> Collect<TSource, TResult>(IRefObservable<TSource> source, Func<TResult> getInitialCollector, Func<TResult, TSource, TResult> merge, Func<TResult, TResult> getNewCollector)
        {
            return Collect_(source, getInitialCollector, merge, getNewCollector);
        }

        private static IEnumerable<TResult> Collect_<TSource, TResult>(IRefObservable<TSource> source, Func<TResult> getInitialCollector, Func<TResult, TSource, TResult> merge, Func<TResult, TResult> getNewCollector)
        {
            return new Collect<TSource, TResult>(source, getInitialCollector, merge, getNewCollector);
        }

        #endregion

        #region First

        public virtual TSource First<TSource>(IRefObservable<TSource> source)
        {
            return FirstOrDefaultInternal(source, throwOnEmpty: true)!;
        }

        public virtual TSource First<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return First(Where(source, predicate));
        }

        #endregion

        #region FirstOrDefault

        [return: MaybeNull]
        public virtual TSource FirstOrDefault<TSource>(IRefObservable<TSource> source)
        {
            return FirstOrDefaultInternal(source, throwOnEmpty: false);
        }

        [return: MaybeNull]
        public virtual TSource FirstOrDefault<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return FirstOrDefault(Where(source, predicate));
        }

        [return: MaybeNull]
        private static TSource FirstOrDefaultInternal<TSource>(IRefObservable<TSource> source, bool throwOnEmpty)
        {
            using var consumer = new FirstBlocking<TSource>();

            using (source.Subscribe((IRefObserver<TSource>) consumer))
            {
                consumer.Wait();
            }

            consumer._error?.Throw();

            if (throwOnEmpty && !consumer._hasValue)
            {
                throw new InvalidOperationException(Strings_Linq.NO_ELEMENTS);
            }

            return consumer._value;
        }

        #endregion

        #region + ForEach +

        public virtual void ForEach<TSource>(IRefObservable<TSource> source, Action<TSource> onNext)
        {
            using var sink = new ForEach<TSource>.Observer(onNext);

            using (source.SubscribeSafe<TSource>(sink))
            {
                sink.Wait();
            }

            sink.Error?.Throw();
        }

        public virtual void ForEach<TSource>(IRefObservable<TSource> source, Action<TSource, int> onNext)
        {
            using var sink = new ForEach<TSource>.ObserverIndexed(onNext);

            using (source.SubscribeSafe<TSource>(sink))
            {
                sink.Wait();
            }

            sink.Error?.Throw();
        }

        #endregion

        #region + GetEnumerator +

        public virtual IEnumerator<TSource> GetEnumerator<TSource>(IRefObservable<TSource> source)
        {
            var e = new GetEnumerator<TSource>();
            return e.Run(source);
        }

        #endregion

        #region Last

        public virtual TSource Last<TSource>(IRefObservable<TSource> source)
        {
            return LastOrDefaultInternal(source, throwOnEmpty: true)!;
        }

        public virtual TSource Last<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return Last(Where(source, predicate));
        }

        #endregion

        #region LastOrDefault

        [return: MaybeNull]
        public virtual TSource LastOrDefault<TSource>(IRefObservable<TSource> source)
        {
            return LastOrDefaultInternal(source, throwOnEmpty: false);
        }

        [return: MaybeNull]
        public virtual TSource LastOrDefault<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return LastOrDefault(Where(source, predicate));
        }

        [return: MaybeNull]
        private static TSource LastOrDefaultInternal<TSource>(IRefObservable<TSource> source, bool throwOnEmpty)
        {
            using var consumer = new LastBlocking<TSource>();

            using (source.Subscribe((IRefObserver<TSource>) consumer))
            {
                consumer.Wait();
            }

            consumer._error?.Throw();

            if (throwOnEmpty && !consumer._hasValue)
            {
                throw new InvalidOperationException(Strings_Linq.NO_ELEMENTS);
            }

            return consumer._value;
        }

        #endregion

        #region + Latest +

        public virtual IEnumerable<TSource> Latest<TSource>(IRefObservable<TSource> source)
        {
            return new Latest<TSource>(source);
        }

        #endregion

        #region + MostRecent +

        public virtual IEnumerable<TSource> MostRecent<TSource>(IRefObservable<TSource> source, TSource initialValue)
        {
            return new MostRecent<TSource>(source, initialValue);
        }

        #endregion

        #region + Next +

        public virtual IEnumerable<TSource> Next<TSource>(IRefObservable<TSource> source)
        {
            return new Next<TSource>(source);
        }

        #endregion

        #region Single

        public virtual TSource Single<TSource>(IRefObservable<TSource> source)
        {
            return SingleOrDefaultInternal(source, throwOnEmpty: true)!;
        }

        public virtual TSource Single<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return Single(Where(source, predicate));
        }

        #endregion

        #region SingleOrDefault

        [return: MaybeNull]
        public virtual TSource SingleOrDefault<TSource>(IRefObservable<TSource> source)
        {
            return SingleOrDefaultInternal(source, throwOnEmpty: false);
        }

        [return: MaybeNull]
        public virtual TSource SingleOrDefault<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return SingleOrDefault(Where(source, predicate));
        }

        [return: MaybeNull]
        private static TSource SingleOrDefaultInternal<TSource>(IRefObservable<TSource> source, bool throwOnEmpty)
        {
            using var consumer = new SingleBlocking<TSource>();

            using (source.Subscribe((IRefObserver<TSource>) consumer))
            {
                consumer.Wait();
            }

            consumer._error?.Throw();

            if (consumer._hasMoreThanOneElement)
            {
                throw new InvalidOperationException(Strings_Linq.MORE_THAN_ONE_ELEMENT);
            }

            if (throwOnEmpty && !consumer._hasValue)
            {
                throw new InvalidOperationException(Strings_Linq.NO_ELEMENTS);
            }

            return consumer._value;
        }

        #endregion

        #region Wait

        public virtual TSource Wait<TSource>(IRefObservable<TSource> source)
        {
            return LastOrDefaultInternal(source, true)!;
        }

        #endregion
    }
}
