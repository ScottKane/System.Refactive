// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;

namespace System.Refactive.Linq
{
    internal partial class QueryLanguage
    {
        #region + Cast +

        public virtual IRefObservable<TResult> Cast<TResult>(IRefObservable<object> source)
        {
            return new Cast<object, TResult>(source);
        }

        #endregion

        #region + DefaultIfEmpty +

        public virtual IRefObservable<TSource?> DefaultIfEmpty<TSource>(IRefObservable<TSource> source)
        {
            return new DefaultIfEmpty<TSource>(source, default!);
        }

        public virtual IRefObservable<TSource> DefaultIfEmpty<TSource>(IRefObservable<TSource> source, TSource defaultValue)
        {
            return new DefaultIfEmpty<TSource>(source, defaultValue);
        }

        #endregion

        #region + Distinct +

        public virtual IRefObservable<TSource> Distinct<TSource>(IRefObservable<TSource> source)
        {
            return new Distinct<TSource, TSource>(source, x => x, EqualityComparer<TSource>.Default);
        }

        public virtual IRefObservable<TSource> Distinct<TSource>(IRefObservable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            return new Distinct<TSource, TSource>(source, x => x, comparer);
        }

        public virtual IRefObservable<TSource> Distinct<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return new Distinct<TSource, TKey>(source, keySelector, EqualityComparer<TKey>.Default);
        }

        public virtual IRefObservable<TSource> Distinct<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return new Distinct<TSource, TKey>(source, keySelector, comparer);
        }

        #endregion

        #region + GroupBy +

        public virtual IRefObservable<IGroupedObservable<TKey, TElement>> GroupBy<TSource, TKey, TElement>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return GroupBy_(source, keySelector, elementSelector, null, EqualityComparer<TKey>.Default);
        }

        public virtual IRefObservable<IGroupedObservable<TKey, TSource>> GroupBy<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return GroupBy_(source, keySelector, x => x, null, comparer);
        }

        public virtual IRefObservable<IGroupedObservable<TKey, TSource>> GroupBy<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return GroupBy_(source, keySelector, x => x, null, EqualityComparer<TKey>.Default);
        }

        public virtual IRefObservable<IGroupedObservable<TKey, TElement>> GroupBy<TSource, TKey, TElement>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            return GroupBy_(source, keySelector, elementSelector, null, comparer);
        }

        public virtual IRefObservable<IGroupedObservable<TKey, TElement>> GroupBy<TSource, TKey, TElement>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, int capacity)
        {
            return GroupBy_(source, keySelector, elementSelector, capacity, EqualityComparer<TKey>.Default);
        }

        public virtual IRefObservable<IGroupedObservable<TKey, TSource>> GroupBy<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, int capacity, IEqualityComparer<TKey> comparer)
        {
            return GroupBy_(source, keySelector, x => x, capacity, comparer);
        }

        public virtual IRefObservable<IGroupedObservable<TKey, TSource>> GroupBy<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, int capacity)
        {
            return GroupBy_(source, keySelector, x => x, capacity, EqualityComparer<TKey>.Default);
        }

        public virtual IRefObservable<IGroupedObservable<TKey, TElement>> GroupBy<TSource, TKey, TElement>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, int capacity, IEqualityComparer<TKey> comparer)
        {
            return GroupBy_(source, keySelector, elementSelector, capacity, comparer);
        }

        private static IRefObservable<IGroupedObservable<TKey, TElement>> GroupBy_<TSource, TKey, TElement>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, int? capacity, IEqualityComparer<TKey> comparer)
        {
            return new GroupBy<TSource, TKey, TElement>(source, keySelector, elementSelector, capacity, comparer);
        }

        #endregion

        #region + GroupByUntil +

        public virtual IRefObservable<IGroupedObservable<TKey, TElement>> GroupByUntil<TSource, TKey, TElement, TDuration>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<IGroupedObservable<TKey, TElement>, IRefObservable<TDuration>> durationSelector, IEqualityComparer<TKey> comparer)
        {
            return GroupByUntil_(source, keySelector, elementSelector, durationSelector, null, comparer);
        }

        public virtual IRefObservable<IGroupedObservable<TKey, TElement>> GroupByUntil<TSource, TKey, TElement, TDuration>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<IGroupedObservable<TKey, TElement>, IRefObservable<TDuration>> durationSelector)
        {
            return GroupByUntil_(source, keySelector, elementSelector, durationSelector, null, EqualityComparer<TKey>.Default);
        }

        public virtual IRefObservable<IGroupedObservable<TKey, TSource>> GroupByUntil<TSource, TKey, TDuration>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<IGroupedObservable<TKey, TSource>, IRefObservable<TDuration>> durationSelector, IEqualityComparer<TKey> comparer)
        {
            return GroupByUntil_(source, keySelector, x => x, durationSelector, null, comparer);
        }

        public virtual IRefObservable<IGroupedObservable<TKey, TSource>> GroupByUntil<TSource, TKey, TDuration>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<IGroupedObservable<TKey, TSource>, IRefObservable<TDuration>> durationSelector)
        {
            return GroupByUntil_(source, keySelector, x => x, durationSelector, null, EqualityComparer<TKey>.Default);
        }

        public virtual IRefObservable<IGroupedObservable<TKey, TElement>> GroupByUntil<TSource, TKey, TElement, TDuration>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<IGroupedObservable<TKey, TElement>, IRefObservable<TDuration>> durationSelector, int capacity, IEqualityComparer<TKey> comparer)
        {
            return GroupByUntil_(source, keySelector, elementSelector, durationSelector, capacity, comparer);
        }

        public virtual IRefObservable<IGroupedObservable<TKey, TElement>> GroupByUntil<TSource, TKey, TElement, TDuration>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<IGroupedObservable<TKey, TElement>, IRefObservable<TDuration>> durationSelector, int capacity)
        {
            return GroupByUntil_(source, keySelector, elementSelector, durationSelector, capacity, EqualityComparer<TKey>.Default);
        }

        public virtual IRefObservable<IGroupedObservable<TKey, TSource>> GroupByUntil<TSource, TKey, TDuration>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<IGroupedObservable<TKey, TSource>, IRefObservable<TDuration>> durationSelector, int capacity, IEqualityComparer<TKey> comparer)
        {
            return GroupByUntil_(source, keySelector, x => x, durationSelector, capacity, comparer);
        }

        public virtual IRefObservable<IGroupedObservable<TKey, TSource>> GroupByUntil<TSource, TKey, TDuration>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<IGroupedObservable<TKey, TSource>, IRefObservable<TDuration>> durationSelector, int capacity)
        {
            return GroupByUntil_(source, keySelector, x => x, durationSelector, capacity, EqualityComparer<TKey>.Default);
        }

        private static IRefObservable<IGroupedObservable<TKey, TElement>> GroupByUntil_<TSource, TKey, TElement, TDuration>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<IGroupedObservable<TKey, TElement>, IRefObservable<TDuration>> durationSelector, int? capacity, IEqualityComparer<TKey> comparer)
        {
            return new GroupByUntil<TSource, TKey, TElement, TDuration>(source, keySelector, elementSelector, durationSelector, capacity, comparer);
        }

        #endregion

        #region + GroupJoin +

        public virtual IRefObservable<TResult> GroupJoin<TLeft, TRight, TLeftDuration, TRightDuration, TResult>(IRefObservable<TLeft> left, IRefObservable<TRight> right, Func<TLeft, IRefObservable<TLeftDuration>> leftDurationSelector, Func<TRight, IRefObservable<TRightDuration>> rightDurationSelector, Func<TLeft, IRefObservable<TRight>, TResult> resultSelector)
        {
            return GroupJoin_(left, right, leftDurationSelector, rightDurationSelector, resultSelector);
        }

        private static IRefObservable<TResult> GroupJoin_<TLeft, TRight, TLeftDuration, TRightDuration, TResult>(IRefObservable<TLeft> left, IRefObservable<TRight> right, Func<TLeft, IRefObservable<TLeftDuration>> leftDurationSelector, Func<TRight, IRefObservable<TRightDuration>> rightDurationSelector, Func<TLeft, IRefObservable<TRight>, TResult> resultSelector)
        {
            return new GroupJoin<TLeft, TRight, TLeftDuration, TRightDuration, TResult>(left, right, leftDurationSelector, rightDurationSelector, resultSelector);
        }

        #endregion

        #region + Join +

        public virtual IRefObservable<TResult> Join<TLeft, TRight, TLeftDuration, TRightDuration, TResult>(IRefObservable<TLeft> left, IRefObservable<TRight> right, Func<TLeft, IRefObservable<TLeftDuration>> leftDurationSelector, Func<TRight, IRefObservable<TRightDuration>> rightDurationSelector, Func<TLeft, TRight, TResult> resultSelector)
        {
            return Join_(left, right, leftDurationSelector, rightDurationSelector, resultSelector);
        }

        private static IRefObservable<TResult> Join_<TLeft, TRight, TLeftDuration, TRightDuration, TResult>(IRefObservable<TLeft> left, IRefObservable<TRight> right, Func<TLeft, IRefObservable<TLeftDuration>> leftDurationSelector, Func<TRight, IRefObservable<TRightDuration>> rightDurationSelector, Func<TLeft, TRight, TResult> resultSelector)
        {
            return new Join<TLeft, TRight, TLeftDuration, TRightDuration, TResult>(left, right, leftDurationSelector, rightDurationSelector, resultSelector);
        }

        #endregion

        #region + OfType +

        public virtual IRefObservable<TResult> OfType<TResult>(IRefObservable<object> source)
        {
            return new OfType<object, TResult>(source);
        }

        #endregion

        #region + Select +

        public virtual IRefObservable<TResult> Select<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, TResult> selector)
        {
            // CONSIDER: Add fusion for Select/Select pairs.

            return new Select<TSource, TResult>.Selector(source, selector);
        }

        public virtual IRefObservable<TResult> Select<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, int, TResult> selector)
        {
            return new Select<TSource, TResult>.SelectorIndexed(source, selector);
        }

        #endregion

        #region + SelectMany +

        public virtual IRefObservable<TOther> SelectMany<TSource, TOther>(IRefObservable<TSource> source, IRefObservable<TOther> other)
        {
            return SelectMany_(source, _ => other);
        }

        public virtual IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TResult>> selector)
        {
            return SelectMany_(source, selector);
        }

        public virtual IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, int, IRefObservable<TResult>> selector)
        {
            return SelectMany_(source, selector);
        }

        public virtual IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, Task<TResult>> selector)
        {
            return new SelectMany<TSource, TResult>.TaskSelector(source, (x, token) => selector(x));
        }

        public virtual IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, int, Task<TResult>> selector)
        {
            return new SelectMany<TSource, TResult>.TaskSelectorIndexed(source, (x, i, token) => selector(x, i));
        }

        public virtual IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, CancellationToken, Task<TResult>> selector)
        {
            return new SelectMany<TSource, TResult>.TaskSelector(source, selector);
        }

        public virtual IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, int, CancellationToken, Task<TResult>> selector)
        {
            return new SelectMany<TSource, TResult>.TaskSelectorIndexed(source, selector);
        }

        public virtual IRefObservable<TResult> SelectMany<TSource, TCollection, TResult>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            return SelectMany_(source, collectionSelector, resultSelector);
        }

        public virtual IRefObservable<TResult> SelectMany<TSource, TCollection, TResult>(IRefObservable<TSource> source, Func<TSource, int, IRefObservable<TCollection>> collectionSelector, Func<TSource, int, TCollection, int, TResult> resultSelector)
        {
            return SelectMany_(source, collectionSelector, resultSelector);
        }

        public virtual IRefObservable<TResult> SelectMany<TSource, TTaskResult, TResult>(IRefObservable<TSource> source, Func<TSource, Task<TTaskResult>> taskSelector, Func<TSource, TTaskResult, TResult> resultSelector)
        {
            return new SelectMany<TSource, TTaskResult, TResult>.TaskSelector(source, (x, token) => taskSelector(x), resultSelector);
        }

        public virtual IRefObservable<TResult> SelectMany<TSource, TTaskResult, TResult>(IRefObservable<TSource> source, Func<TSource, int, Task<TTaskResult>> taskSelector, Func<TSource, int, TTaskResult, TResult> resultSelector)
        {
            return new SelectMany<TSource, TTaskResult, TResult>.TaskSelectorIndexed(source, (x, i, token) => taskSelector(x, i), resultSelector);
        }

        public virtual IRefObservable<TResult> SelectMany<TSource, TTaskResult, TResult>(IRefObservable<TSource> source, Func<TSource, CancellationToken, Task<TTaskResult>> taskSelector, Func<TSource, TTaskResult, TResult> resultSelector)
        {
            return new SelectMany<TSource, TTaskResult, TResult>.TaskSelector(source, taskSelector, resultSelector);
        }

        public virtual IRefObservable<TResult> SelectMany<TSource, TTaskResult, TResult>(IRefObservable<TSource> source, Func<TSource, int, CancellationToken, Task<TTaskResult>> taskSelector, Func<TSource, int, TTaskResult, TResult> resultSelector)
        {
            return new SelectMany<TSource, TTaskResult, TResult>.TaskSelectorIndexed(source, taskSelector, resultSelector);
        }

        private static IRefObservable<TResult> SelectMany_<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TResult>> selector)
        {
            return new SelectMany<TSource, TResult>.ObservableSelector(source, selector);
        }

        private static IRefObservable<TResult> SelectMany_<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, int, IRefObservable<TResult>> selector)
        {
            return new SelectMany<TSource, TResult>.ObservableSelectorIndexed(source, selector);
        }

        private static IRefObservable<TResult> SelectMany_<TSource, TCollection, TResult>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            return new SelectMany<TSource, TCollection, TResult>.ObservableSelector(source, collectionSelector, resultSelector);
        }

        private static IRefObservable<TResult> SelectMany_<TSource, TCollection, TResult>(IRefObservable<TSource> source, Func<TSource, int, IRefObservable<TCollection>> collectionSelector, Func<TSource, int, TCollection, int, TResult> resultSelector)
        {
            return new SelectMany<TSource, TCollection, TResult>.ObservableSelectorIndexed(source, collectionSelector, resultSelector);
        }

        public virtual IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TResult>> onNext, Func<Exception, IRefObservable<TResult>> onError, Func<IRefObservable<TResult>> onCompleted)
        {
            return new SelectMany<TSource, TResult>.ObservableSelectors(source, onNext, onError, onCompleted);
        }

        public virtual IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, int, IRefObservable<TResult>> onNext, Func<Exception, IRefObservable<TResult>> onError, Func<IRefObservable<TResult>> onCompleted)
        {
            return new SelectMany<TSource, TResult>.ObservableSelectorsIndexed(source, onNext, onError, onCompleted);
        }

        public virtual IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            return new SelectMany<TSource, TResult>.EnumerableSelector(source, selector);
        }

        public virtual IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
        {
            return new SelectMany<TSource, TResult>.EnumerableSelectorIndexed(source, selector);
        }

        public virtual IRefObservable<TResult> SelectMany<TSource, TCollection, TResult>(IRefObservable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            return SelectMany_(source, collectionSelector, resultSelector);
        }

        public virtual IRefObservable<TResult> SelectMany<TSource, TCollection, TResult>(IRefObservable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, int, TCollection, int, TResult> resultSelector)
        {
            return SelectMany_(source, collectionSelector, resultSelector);
        }

        private static IRefObservable<TResult> SelectMany_<TSource, TCollection, TResult>(IRefObservable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            return new SelectMany<TSource, TCollection, TResult>.EnumerableSelector(source, collectionSelector, resultSelector);
        }

        private static IRefObservable<TResult> SelectMany_<TSource, TCollection, TResult>(IRefObservable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, int, TCollection, int, TResult> resultSelector)
        {
            return new SelectMany<TSource, TCollection, TResult>.EnumerableSelectorIndexed(source, collectionSelector, resultSelector);
        }

        #endregion

        #region + Skip +

        public virtual IRefObservable<TSource> Skip<TSource>(IRefObservable<TSource> source, int count)
        {
            if (source is Skip<TSource>.Count skip)
            {
                return skip.Combine(count);
            }

            return new Skip<TSource>.Count(source, count);
        }

        #endregion

        #region + SkipWhile +

        public virtual IRefObservable<TSource> SkipWhile<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return new SkipWhile<TSource>.Predicate(source, predicate);
        }

        public virtual IRefObservable<TSource> SkipWhile<TSource>(IRefObservable<TSource> source, Func<TSource, int, bool> predicate)
        {
            return new SkipWhile<TSource>.PredicateIndexed(source, predicate);
        }

        #endregion

        #region + Take +

        public virtual IRefObservable<TSource> Take<TSource>(IRefObservable<TSource> source, int count)
        {
            if (count == 0)
            {
                return Empty<TSource>();
            }

            return Take_(source, count);
        }

        public virtual IRefObservable<TSource> Take<TSource>(IRefObservable<TSource> source, int count, IScheduler scheduler)
        {
            if (count == 0)
            {
                return Empty<TSource>(scheduler);
            }

            return Take_(source, count);
        }

        private static IRefObservable<TSource> Take_<TSource>(IRefObservable<TSource> source, int count)
        {
            if (source is Take<TSource>.Count take)
            {
                return take.Combine(count);
            }

            return new Take<TSource>.Count(source, count);
        }

        #endregion

        #region + TakeWhile +

        public virtual IRefObservable<TSource> TakeWhile<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return new TakeWhile<TSource>.Predicate(source, predicate);
        }

        public virtual IRefObservable<TSource> TakeWhile<TSource>(IRefObservable<TSource> source, Func<TSource, int, bool> predicate)
        {
            return new TakeWhile<TSource>.PredicateIndexed(source, predicate);
        }

        #endregion

        #region + Where +

        public virtual IRefObservable<TSource> Where<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source is Where<TSource>.Predicate where)
            {
                return where.Combine(predicate);
            }

            return new Where<TSource>.Predicate(source, predicate);
        }

        public virtual IRefObservable<TSource> Where<TSource>(IRefObservable<TSource> source, Func<TSource, int, bool> predicate)
        {
            return new Where<TSource>.PredicateIndexed(source, predicate);
        }

        #endregion
    }
}
