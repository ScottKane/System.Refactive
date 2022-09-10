// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Linq;

namespace System.Refactive.Linq
{
    internal partial class QueryLanguage
    {
        #region + Aggregate +

        public virtual IRefObservable<TAccumulate> Aggregate<TSource, TAccumulate>(IRefObservable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
        {
            return new Aggregate<TSource, TAccumulate>(source, seed, accumulator);
        }

        public virtual IRefObservable<TResult> Aggregate<TSource, TAccumulate, TResult>(IRefObservable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator, Func<TAccumulate, TResult> resultSelector)
        {
            return new Aggregate<TSource, TAccumulate, TResult>(source, seed, accumulator, resultSelector);
        }

        public virtual IRefObservable<TSource> Aggregate<TSource>(IRefObservable<TSource> source, Func<TSource, TSource, TSource> accumulator)
        {
            return new Aggregate<TSource>(source, accumulator);
        }

        public virtual IRefObservable<double> Average<TSource>(IRefObservable<TSource> source, Func<TSource, double> selector)
        {
            return Average(Select(source, selector));
        }

        public virtual IRefObservable<float> Average<TSource>(IRefObservable<TSource> source, Func<TSource, float> selector)
        {
            return Average(Select(source, selector));
        }

        public virtual IRefObservable<decimal> Average<TSource>(IRefObservable<TSource> source, Func<TSource, decimal> selector)
        {
            return Average(Select(source, selector));
        }

        public virtual IRefObservable<double> Average<TSource>(IRefObservable<TSource> source, Func<TSource, int> selector)
        {
            return Average(Select(source, selector));
        }

        public virtual IRefObservable<double> Average<TSource>(IRefObservable<TSource> source, Func<TSource, long> selector)
        {
            return Average(Select(source, selector));
        }

        public virtual IRefObservable<double?> Average<TSource>(IRefObservable<TSource> source, Func<TSource, double?> selector)
        {
            return Average(Select(source, selector));
        }

        public virtual IRefObservable<float?> Average<TSource>(IRefObservable<TSource> source, Func<TSource, float?> selector)
        {
            return Average(Select(source, selector));
        }

        public virtual IRefObservable<decimal?> Average<TSource>(IRefObservable<TSource> source, Func<TSource, decimal?> selector)
        {
            return Average(Select(source, selector));
        }

        public virtual IRefObservable<double?> Average<TSource>(IRefObservable<TSource> source, Func<TSource, int?> selector)
        {
            return Average(Select(source, selector));
        }

        public virtual IRefObservable<double?> Average<TSource>(IRefObservable<TSource> source, Func<TSource, long?> selector)
        {
            return Average(Select(source, selector));
        }

        #endregion

        #region + All +

        public virtual IRefObservable<bool> All<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return new All<TSource>(source, predicate);
        }

        #endregion

        #region + Any +

        public virtual IRefObservable<bool> Any<TSource>(IRefObservable<TSource> source)
        {
            return new Any<TSource>.Count(source);
        }

        public virtual IRefObservable<bool> Any<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return new Any<TSource>.Predicate(source, predicate);
        }

        #endregion

        #region + Average +

        public virtual IRefObservable<double> Average(IRefObservable<double> source)
        {
            return new AverageDouble(source);
        }

        public virtual IRefObservable<float> Average(IRefObservable<float> source)
        {
            return new AverageSingle(source);
        }

        public virtual IRefObservable<decimal> Average(IRefObservable<decimal> source)
        {
            return new AverageDecimal(source);
        }

        public virtual IRefObservable<double> Average(IRefObservable<int> source)
        {
            return new AverageInt32(source);
        }

        public virtual IRefObservable<double> Average(IRefObservable<long> source)
        {
            return new AverageInt64(source);
        }

        public virtual IRefObservable<double?> Average(IRefObservable<double?> source)
        {
            return new AverageDoubleNullable(source);
        }

        public virtual IRefObservable<float?> Average(IRefObservable<float?> source)
        {
            return new AverageSingleNullable(source);
        }

        public virtual IRefObservable<decimal?> Average(IRefObservable<decimal?> source)
        {
            return new AverageDecimalNullable(source);
        }

        public virtual IRefObservable<double?> Average(IRefObservable<int?> source)
        {
            return new AverageInt32Nullable(source);
        }

        public virtual IRefObservable<double?> Average(IRefObservable<long?> source)
        {
            return new AverageInt64Nullable(source);
        }

        #endregion

        #region + Contains +

        public virtual IRefObservable<bool> Contains<TSource>(IRefObservable<TSource> source, TSource value)
        {
            return new Contains<TSource>(source, value, EqualityComparer<TSource>.Default);
        }

        public virtual IRefObservable<bool> Contains<TSource>(IRefObservable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
        {
            return new Contains<TSource>(source, value, comparer);
        }

        #endregion

        #region + Count +

        public virtual IRefObservable<int> Count<TSource>(IRefObservable<TSource> source)
        {
            return new Count<TSource>.All(source);
        }

        public virtual IRefObservable<int> Count<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return new Count<TSource>.Predicate(source, predicate);
        }

        #endregion

        #region + ElementAt +

        public virtual IRefObservable<TSource> ElementAt<TSource>(IRefObservable<TSource> source, int index)
        {
            return new ElementAt<TSource>(source, index);
        }

        #endregion

        #region + ElementAtOrDefault +

        public virtual IRefObservable<TSource?> ElementAtOrDefault<TSource>(IRefObservable<TSource> source, int index)
        {
            return new ElementAtOrDefault<TSource>(source, index);
        }

        #endregion

        #region + FirstAsync +

        public virtual IRefObservable<TSource> FirstAsync<TSource>(IRefObservable<TSource> source)
        {
            return new FirstAsync<TSource>.Sequence(source);
        }

        public virtual IRefObservable<TSource> FirstAsync<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return new FirstAsync<TSource>.Predicate(source, predicate);
        }

        #endregion

        #region + FirstAsyncOrDefaultAsync +

        public virtual IRefObservable<TSource?> FirstOrDefaultAsync<TSource>(IRefObservable<TSource> source)
        {
            return new FirstOrDefaultAsync<TSource>.Sequence(source);
        }

        public virtual IRefObservable<TSource?> FirstOrDefaultAsync<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return new FirstOrDefaultAsync<TSource>.Predicate(source, predicate);
        }

        #endregion

        #region + IsEmpty +

        public virtual IRefObservable<bool> IsEmpty<TSource>(IRefObservable<TSource> source)
        {
            return new IsEmpty<TSource>(source);
        }

        #endregion

        #region + LastAsync +

        public virtual IRefObservable<TSource> LastAsync<TSource>(IRefObservable<TSource> source)
        {
            return new LastAsync<TSource>.Sequence(source);
        }

        public virtual IRefObservable<TSource> LastAsync<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return new LastAsync<TSource>.Predicate(source, predicate);
        }

        #endregion

        #region + LastOrDefaultAsync +

        public virtual IRefObservable<TSource?> LastOrDefaultAsync<TSource>(IRefObservable<TSource> source)
        {
            return new LastOrDefaultAsync<TSource>.Sequence(source);
        }

        public virtual IRefObservable<TSource?> LastOrDefaultAsync<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return new LastOrDefaultAsync<TSource>.Predicate(source, predicate);
        }

        #endregion

        #region + LongCount +

        public virtual IRefObservable<long> LongCount<TSource>(IRefObservable<TSource> source)
        {
            return new LongCount<TSource>.All(source);
        }

        public virtual IRefObservable<long> LongCount<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return new LongCount<TSource>.Predicate(source, predicate);
        }

        #endregion

        #region + Max +

        public virtual IRefObservable<TSource> Max<TSource>(IRefObservable<TSource> source)
        {
            // BREAKING CHANGE v2 > v1.x - Behavior for reference types
            return new Max<TSource>(source, Comparer<TSource>.Default);
        }

        public virtual IRefObservable<TSource> Max<TSource>(IRefObservable<TSource> source, IComparer<TSource> comparer)
        {
            // BREAKING CHANGE v2 > v1.x - Behavior for reference types
            return new Max<TSource>(source, comparer);
        }

        public virtual IRefObservable<double> Max(IRefObservable<double> source)
        {
            return new MaxDouble(source);
        }

        public virtual IRefObservable<float> Max(IRefObservable<float> source)
        {
            return new MaxSingle(source);
        }

        public virtual IRefObservable<decimal> Max(IRefObservable<decimal> source)
        {
            return new MaxDecimal(source);
        }

        public virtual IRefObservable<int> Max(IRefObservable<int> source)
        {
            return new MaxInt32(source);
        }

        public virtual IRefObservable<long> Max(IRefObservable<long> source)
        {
            return new MaxInt64(source);
        }

        public virtual IRefObservable<double?> Max(IRefObservable<double?> source)
        {
            return new MaxDoubleNullable(source);
        }

        public virtual IRefObservable<float?> Max(IRefObservable<float?> source)
        {
            return new MaxSingleNullable(source);
        }

        public virtual IRefObservable<decimal?> Max(IRefObservable<decimal?> source)
        {
            return new MaxDecimalNullable(source);
        }

        public virtual IRefObservable<int?> Max(IRefObservable<int?> source)
        {
            return new MaxInt32Nullable(source);
        }

        public virtual IRefObservable<long?> Max(IRefObservable<long?> source)
        {
            return new MaxInt64Nullable(source);
        }

        public virtual IRefObservable<TResult> Max<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, TResult> selector)
        {
            return Max(Select(source, selector));
        }

        public virtual IRefObservable<TResult> Max<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, TResult> selector, IComparer<TResult> comparer)
        {
            return Max(Select(source, selector), comparer);
        }

        public virtual IRefObservable<double> Max<TSource>(IRefObservable<TSource> source, Func<TSource, double> selector)
        {
            return Max(Select(source, selector));
        }

        public virtual IRefObservable<float> Max<TSource>(IRefObservable<TSource> source, Func<TSource, float> selector)
        {
            return Max(Select(source, selector));
        }

        public virtual IRefObservable<decimal> Max<TSource>(IRefObservable<TSource> source, Func<TSource, decimal> selector)
        {
            return Max(Select(source, selector));
        }

        public virtual IRefObservable<int> Max<TSource>(IRefObservable<TSource> source, Func<TSource, int> selector)
        {
            return Max(Select(source, selector));
        }

        public virtual IRefObservable<long> Max<TSource>(IRefObservable<TSource> source, Func<TSource, long> selector)
        {
            return Max(Select(source, selector));
        }

        public virtual IRefObservable<double?> Max<TSource>(IRefObservable<TSource> source, Func<TSource, double?> selector)
        {
            return Max(Select(source, selector));
        }

        public virtual IRefObservable<float?> Max<TSource>(IRefObservable<TSource> source, Func<TSource, float?> selector)
        {
            return Max(Select(source, selector));
        }

        public virtual IRefObservable<decimal?> Max<TSource>(IRefObservable<TSource> source, Func<TSource, decimal?> selector)
        {
            return Max(Select(source, selector));
        }

        public virtual IRefObservable<int?> Max<TSource>(IRefObservable<TSource> source, Func<TSource, int?> selector)
        {
            return Max(Select(source, selector));
        }

        public virtual IRefObservable<long?> Max<TSource>(IRefObservable<TSource> source, Func<TSource, long?> selector)
        {
            return Max(Select(source, selector));
        }

        #endregion

        #region + MaxBy +

        public virtual IRefObservable<IList<TSource>> MaxBy<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return new MaxBy<TSource, TKey>(source, keySelector, Comparer<TKey>.Default);
        }

        public virtual IRefObservable<IList<TSource>> MaxBy<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return new MaxBy<TSource, TKey>(source, keySelector, comparer);
        }

        #endregion

        #region + Min +

        public virtual IRefObservable<TSource> Min<TSource>(IRefObservable<TSource> source)
        {
            // BREAKING CHANGE v2 > v1.x - Behavior for reference types
            return new Min<TSource>(source, Comparer<TSource>.Default);
        }

        public virtual IRefObservable<TSource> Min<TSource>(IRefObservable<TSource> source, IComparer<TSource> comparer)
        {
            // BREAKING CHANGE v2 > v1.x - Behavior for reference types
            return new Min<TSource>(source, comparer);
        }

        public virtual IRefObservable<double> Min(IRefObservable<double> source)
        {
            return new MinDouble(source);
        }

        public virtual IRefObservable<float> Min(IRefObservable<float> source)
        {
            return new MinSingle(source);
        }

        public virtual IRefObservable<decimal> Min(IRefObservable<decimal> source)
        {
            return new MinDecimal(source);
        }

        public virtual IRefObservable<int> Min(IRefObservable<int> source)
        {
            return new MinInt32(source);
        }

        public virtual IRefObservable<long> Min(IRefObservable<long> source)
        {
            return new MinInt64(source);
        }

        public virtual IRefObservable<double?> Min(IRefObservable<double?> source)
        {
            return new MinDoubleNullable(source);
        }

        public virtual IRefObservable<float?> Min(IRefObservable<float?> source)
        {
            return new MinSingleNullable(source);
        }

        public virtual IRefObservable<decimal?> Min(IRefObservable<decimal?> source)
        {
            return new MinDecimalNullable(source);
        }

        public virtual IRefObservable<int?> Min(IRefObservable<int?> source)
        {
            return new MinInt32Nullable(source);
        }

        public virtual IRefObservable<long?> Min(IRefObservable<long?> source)
        {
            return new MinInt64Nullable(source);
        }

        public virtual IRefObservable<TResult> Min<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, TResult> selector)
        {
            return Min(Select(source, selector));
        }

        public virtual IRefObservable<TResult> Min<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, TResult> selector, IComparer<TResult> comparer)
        {
            return Min(Select(source, selector), comparer);
        }

        public virtual IRefObservable<double> Min<TSource>(IRefObservable<TSource> source, Func<TSource, double> selector)
        {
            return Min(Select(source, selector));
        }

        public virtual IRefObservable<float> Min<TSource>(IRefObservable<TSource> source, Func<TSource, float> selector)
        {
            return Min(Select(source, selector));
        }

        public virtual IRefObservable<decimal> Min<TSource>(IRefObservable<TSource> source, Func<TSource, decimal> selector)
        {
            return Min(Select(source, selector));
        }

        public virtual IRefObservable<int> Min<TSource>(IRefObservable<TSource> source, Func<TSource, int> selector)
        {
            return Min(Select(source, selector));
        }

        public virtual IRefObservable<long> Min<TSource>(IRefObservable<TSource> source, Func<TSource, long> selector)
        {
            return Min(Select(source, selector));
        }

        public virtual IRefObservable<double?> Min<TSource>(IRefObservable<TSource> source, Func<TSource, double?> selector)
        {
            return Min(Select(source, selector));
        }

        public virtual IRefObservable<float?> Min<TSource>(IRefObservable<TSource> source, Func<TSource, float?> selector)
        {
            return Min(Select(source, selector));
        }

        public virtual IRefObservable<decimal?> Min<TSource>(IRefObservable<TSource> source, Func<TSource, decimal?> selector)
        {
            return Min(Select(source, selector));
        }

        public virtual IRefObservable<int?> Min<TSource>(IRefObservable<TSource> source, Func<TSource, int?> selector)
        {
            return Min(Select(source, selector));
        }

        public virtual IRefObservable<long?> Min<TSource>(IRefObservable<TSource> source, Func<TSource, long?> selector)
        {
            return Min(Select(source, selector));
        }

        #endregion

        #region + MinBy +

        public virtual IRefObservable<IList<TSource>> MinBy<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return new MinBy<TSource, TKey>(source, keySelector, Comparer<TKey>.Default);
        }

        public virtual IRefObservable<IList<TSource>> MinBy<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return new MinBy<TSource, TKey>(source, keySelector, comparer);
        }

        #endregion

        #region + SequenceEqual +

        public virtual IRefObservable<bool> SequenceEqual<TSource>(IRefObservable<TSource> first, IRefObservable<TSource> second)
        {
            return new SequenceEqual<TSource>.Observable(first, second, EqualityComparer<TSource>.Default);
        }

        public virtual IRefObservable<bool> SequenceEqual<TSource>(IRefObservable<TSource> first, IRefObservable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            return new SequenceEqual<TSource>.Observable(first, second, comparer);
        }

        public virtual IRefObservable<bool> SequenceEqual<TSource>(IRefObservable<TSource> first, IEnumerable<TSource> second)
        {
            return new SequenceEqual<TSource>.Enumerable(first, second, EqualityComparer<TSource>.Default);
        }

        public virtual IRefObservable<bool> SequenceEqual<TSource>(IRefObservable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            return new SequenceEqual<TSource>.Enumerable(first, second, comparer);
        }

        #endregion

        #region + SingleAsync +

        public virtual IRefObservable<TSource> SingleAsync<TSource>(IRefObservable<TSource> source)
        {
            return new SingleAsync<TSource>.Sequence(source);
        }

        public virtual IRefObservable<TSource> SingleAsync<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return new SingleAsync<TSource>.Predicate(source, predicate);
        }

        #endregion

        #region + SingleOrDefaultAsync +

        public virtual IRefObservable<TSource?> SingleOrDefaultAsync<TSource>(IRefObservable<TSource> source)
        {
            return new SingleOrDefaultAsync<TSource>.Sequence(source);
        }

        public virtual IRefObservable<TSource?> SingleOrDefaultAsync<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return new SingleOrDefaultAsync<TSource>.Predicate(source, predicate);
        }

        #endregion

        #region + Sum +

        public virtual IRefObservable<double> Sum(IRefObservable<double> source)
        {
            return new SumDouble(source);
        }

        public virtual IRefObservable<float> Sum(IRefObservable<float> source)
        {
            return new SumSingle(source);
        }

        public virtual IRefObservable<decimal> Sum(IRefObservable<decimal> source)
        {
            return new SumDecimal(source);
        }

        public virtual IRefObservable<int> Sum(IRefObservable<int> source)
        {
            return new SumInt32(source);
        }

        public virtual IRefObservable<long> Sum(IRefObservable<long> source)
        {
            return new SumInt64(source);
        }

        public virtual IRefObservable<double?> Sum(IRefObservable<double?> source)
        {
            return new SumDoubleNullable(source);
        }

        public virtual IRefObservable<float?> Sum(IRefObservable<float?> source)
        {
            return new SumSingleNullable(source);
        }

        public virtual IRefObservable<decimal?> Sum(IRefObservable<decimal?> source)
        {
            return new SumDecimalNullable(source);
        }

        public virtual IRefObservable<int?> Sum(IRefObservable<int?> source)
        {
            return new SumInt32Nullable(source);
        }

        public virtual IRefObservable<long?> Sum(IRefObservable<long?> source)
        {
            return new SumInt64Nullable(source);
        }

        public virtual IRefObservable<double> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, double> selector)
        {
            return Sum(Select(source, selector));
        }

        public virtual IRefObservable<float> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, float> selector)
        {
            return Sum(Select(source, selector));
        }

        public virtual IRefObservable<decimal> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, decimal> selector)
        {
            return Sum(Select(source, selector));
        }

        public virtual IRefObservable<int> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, int> selector)
        {
            return Sum(Select(source, selector));
        }

        public virtual IRefObservable<long> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, long> selector)
        {
            return Sum(Select(source, selector));
        }

        public virtual IRefObservable<double?> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, double?> selector)
        {
            return Sum(Select(source, selector));
        }

        public virtual IRefObservable<float?> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, float?> selector)
        {
            return Sum(Select(source, selector));
        }

        public virtual IRefObservable<decimal?> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, decimal?> selector)
        {
            return Sum(Select(source, selector));
        }

        public virtual IRefObservable<int?> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, int?> selector)
        {
            return Sum(Select(source, selector));
        }

        public virtual IRefObservable<long?> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, long?> selector)
        {
            return Sum(Select(source, selector));
        }

        #endregion

        #region + ToArray +

        public virtual IRefObservable<TSource[]> ToArray<TSource>(IRefObservable<TSource> source)
        {
            return new ToArray<TSource>(source);
        }

        #endregion

        #region + ToDictionary +

        public virtual IRefObservable<IDictionary<TKey, TElement>> ToDictionary<TSource, TKey, TElement>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
            where TKey : notnull
        {
            return new ToDictionary<TSource, TKey, TElement>(source, keySelector, elementSelector, comparer);
        }

        public virtual IRefObservable<IDictionary<TKey, TElement>> ToDictionary<TSource, TKey, TElement>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            where TKey : notnull
        {
            return new ToDictionary<TSource, TKey, TElement>(source, keySelector, elementSelector, EqualityComparer<TKey>.Default);
        }

        public virtual IRefObservable<IDictionary<TKey, TSource>> ToDictionary<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
            where TKey : notnull
        {
            return new ToDictionary<TSource, TKey, TSource>(source, keySelector, x => x, comparer);
        }

        public virtual IRefObservable<IDictionary<TKey, TSource>> ToDictionary<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector)
            where TKey : notnull
        {
            return new ToDictionary<TSource, TKey, TSource>(source, keySelector, x => x, EqualityComparer<TKey>.Default);
        }

        #endregion

        #region + ToList +

        public virtual IRefObservable<IList<TSource>> ToList<TSource>(IRefObservable<TSource> source)
        {
            return new ToList<TSource>(source);
        }

        #endregion

        #region + ToLookup +

        public virtual IRefObservable<ILookup<TKey, TElement>> ToLookup<TSource, TKey, TElement>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            return new ToLookup<TSource, TKey, TElement>(source, keySelector, elementSelector, comparer);
        }

        public virtual IRefObservable<ILookup<TKey, TSource>> ToLookup<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return new ToLookup<TSource, TKey, TSource>(source, keySelector, x => x, comparer);
        }

        public virtual IRefObservable<ILookup<TKey, TElement>> ToLookup<TSource, TKey, TElement>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return new ToLookup<TSource, TKey, TElement>(source, keySelector, elementSelector, EqualityComparer<TKey>.Default);
        }

        public virtual IRefObservable<ILookup<TKey, TSource>> ToLookup<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return new ToLookup<TSource, TKey, TSource>(source, keySelector, x => x, EqualityComparer<TKey>.Default);
        }

        #endregion
    }
}
