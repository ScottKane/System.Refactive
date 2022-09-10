// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Refactive.Concurrency;
using System.Refactive.Joins;
using System.Refactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace System.Refactive.Linq
{
    /// <summary>
    /// Internal interface describing the LINQ to Events query language.
    /// </summary>
    internal partial interface IQueryLanguage
    {
        #region * Aggregates *

        IRefObservable<TAccumulate> Aggregate<TSource, TAccumulate>(IRefObservable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator);
        IRefObservable<TResult> Aggregate<TSource, TAccumulate, TResult>(IRefObservable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator, Func<TAccumulate, TResult> resultSelector);
        IRefObservable<TSource> Aggregate<TSource>(IRefObservable<TSource> source, Func<TSource, TSource, TSource> accumulator);
        IRefObservable<bool> All<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate);
        IRefObservable<bool> Any<TSource>(IRefObservable<TSource> source);
        IRefObservable<bool> Any<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate);
        IRefObservable<double> Average(IRefObservable<double> source);
        IRefObservable<float> Average(IRefObservable<float> source);
        IRefObservable<decimal> Average(IRefObservable<decimal> source);
        IRefObservable<double> Average(IRefObservable<int> source);
        IRefObservable<double> Average(IRefObservable<long> source);
        IRefObservable<double?> Average(IRefObservable<double?> source);
        IRefObservable<float?> Average(IRefObservable<float?> source);
        IRefObservable<decimal?> Average(IRefObservable<decimal?> source);
        IRefObservable<double?> Average(IRefObservable<int?> source);
        IRefObservable<double?> Average(IRefObservable<long?> source);
        IRefObservable<double> Average<TSource>(IRefObservable<TSource> source, Func<TSource, double> selector);
        IRefObservable<float> Average<TSource>(IRefObservable<TSource> source, Func<TSource, float> selector);
        IRefObservable<decimal> Average<TSource>(IRefObservable<TSource> source, Func<TSource, decimal> selector);
        IRefObservable<double> Average<TSource>(IRefObservable<TSource> source, Func<TSource, int> selector);
        IRefObservable<double> Average<TSource>(IRefObservable<TSource> source, Func<TSource, long> selector);
        IRefObservable<double?> Average<TSource>(IRefObservable<TSource> source, Func<TSource, double?> selector);
        IRefObservable<float?> Average<TSource>(IRefObservable<TSource> source, Func<TSource, float?> selector);
        IRefObservable<decimal?> Average<TSource>(IRefObservable<TSource> source, Func<TSource, decimal?> selector);
        IRefObservable<double?> Average<TSource>(IRefObservable<TSource> source, Func<TSource, int?> selector);
        IRefObservable<double?> Average<TSource>(IRefObservable<TSource> source, Func<TSource, long?> selector);
        IRefObservable<bool> Contains<TSource>(IRefObservable<TSource> source, TSource value);
        IRefObservable<bool> Contains<TSource>(IRefObservable<TSource> source, TSource value, IEqualityComparer<TSource> comparer);
        IRefObservable<int> Count<TSource>(IRefObservable<TSource> source);
        IRefObservable<int> Count<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate);
        IRefObservable<TSource> ElementAt<TSource>(IRefObservable<TSource> source, int index);
        IRefObservable<TSource?> ElementAtOrDefault<TSource>(IRefObservable<TSource> source, int index);
        IRefObservable<TSource> FirstAsync<TSource>(IRefObservable<TSource> source);
        IRefObservable<TSource> FirstAsync<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate);
        IRefObservable<TSource?> FirstOrDefaultAsync<TSource>(IRefObservable<TSource> source);
        IRefObservable<TSource?> FirstOrDefaultAsync<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate);
        IRefObservable<bool> IsEmpty<TSource>(IRefObservable<TSource> source);
        IRefObservable<TSource> LastAsync<TSource>(IRefObservable<TSource> source);
        IRefObservable<TSource> LastAsync<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate);
        IRefObservable<TSource?> LastOrDefaultAsync<TSource>(IRefObservable<TSource> source);
        IRefObservable<TSource?> LastOrDefaultAsync<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate);
        IRefObservable<long> LongCount<TSource>(IRefObservable<TSource> source);
        IRefObservable<long> LongCount<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate);
        IRefObservable<TSource> Max<TSource>(IRefObservable<TSource> source);
        IRefObservable<TSource> Max<TSource>(IRefObservable<TSource> source, IComparer<TSource> comparer);
        IRefObservable<double> Max(IRefObservable<double> source);
        IRefObservable<float> Max(IRefObservable<float> source);
        IRefObservable<decimal> Max(IRefObservable<decimal> source);
        IRefObservable<int> Max(IRefObservable<int> source);
        IRefObservable<long> Max(IRefObservable<long> source);
        IRefObservable<double?> Max(IRefObservable<double?> source);
        IRefObservable<float?> Max(IRefObservable<float?> source);
        IRefObservable<decimal?> Max(IRefObservable<decimal?> source);
        IRefObservable<int?> Max(IRefObservable<int?> source);
        IRefObservable<long?> Max(IRefObservable<long?> source);
        IRefObservable<TResult> Max<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, TResult> selector);
        IRefObservable<TResult> Max<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, TResult> selector, IComparer<TResult> comparer);
        IRefObservable<double> Max<TSource>(IRefObservable<TSource> source, Func<TSource, double> selector);
        IRefObservable<float> Max<TSource>(IRefObservable<TSource> source, Func<TSource, float> selector);
        IRefObservable<decimal> Max<TSource>(IRefObservable<TSource> source, Func<TSource, decimal> selector);
        IRefObservable<int> Max<TSource>(IRefObservable<TSource> source, Func<TSource, int> selector);
        IRefObservable<long> Max<TSource>(IRefObservable<TSource> source, Func<TSource, long> selector);
        IRefObservable<double?> Max<TSource>(IRefObservable<TSource> source, Func<TSource, double?> selector);
        IRefObservable<float?> Max<TSource>(IRefObservable<TSource> source, Func<TSource, float?> selector);
        IRefObservable<decimal?> Max<TSource>(IRefObservable<TSource> source, Func<TSource, decimal?> selector);
        IRefObservable<int?> Max<TSource>(IRefObservable<TSource> source, Func<TSource, int?> selector);
        IRefObservable<long?> Max<TSource>(IRefObservable<TSource> source, Func<TSource, long?> selector);
        IRefObservable<IList<TSource>> MaxBy<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector);
        IRefObservable<IList<TSource>> MaxBy<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer);
        IRefObservable<TSource> Min<TSource>(IRefObservable<TSource> source);
        IRefObservable<TSource> Min<TSource>(IRefObservable<TSource> source, IComparer<TSource> comparer);
        IRefObservable<double> Min(IRefObservable<double> source);
        IRefObservable<float> Min(IRefObservable<float> source);
        IRefObservable<decimal> Min(IRefObservable<decimal> source);
        IRefObservable<int> Min(IRefObservable<int> source);
        IRefObservable<long> Min(IRefObservable<long> source);
        IRefObservable<double?> Min(IRefObservable<double?> source);
        IRefObservable<float?> Min(IRefObservable<float?> source);
        IRefObservable<decimal?> Min(IRefObservable<decimal?> source);
        IRefObservable<int?> Min(IRefObservable<int?> source);
        IRefObservable<long?> Min(IRefObservable<long?> source);
        IRefObservable<TResult> Min<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, TResult> selector);
        IRefObservable<TResult> Min<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, TResult> selector, IComparer<TResult> comparer);
        IRefObservable<double> Min<TSource>(IRefObservable<TSource> source, Func<TSource, double> selector);
        IRefObservable<float> Min<TSource>(IRefObservable<TSource> source, Func<TSource, float> selector);
        IRefObservable<decimal> Min<TSource>(IRefObservable<TSource> source, Func<TSource, decimal> selector);
        IRefObservable<int> Min<TSource>(IRefObservable<TSource> source, Func<TSource, int> selector);
        IRefObservable<long> Min<TSource>(IRefObservable<TSource> source, Func<TSource, long> selector);
        IRefObservable<double?> Min<TSource>(IRefObservable<TSource> source, Func<TSource, double?> selector);
        IRefObservable<float?> Min<TSource>(IRefObservable<TSource> source, Func<TSource, float?> selector);
        IRefObservable<decimal?> Min<TSource>(IRefObservable<TSource> source, Func<TSource, decimal?> selector);
        IRefObservable<int?> Min<TSource>(IRefObservable<TSource> source, Func<TSource, int?> selector);
        IRefObservable<long?> Min<TSource>(IRefObservable<TSource> source, Func<TSource, long?> selector);
        IRefObservable<IList<TSource>> MinBy<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector);
        IRefObservable<IList<TSource>> MinBy<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer);
        IRefObservable<bool> SequenceEqual<TSource>(IRefObservable<TSource> first, IRefObservable<TSource> second);
        IRefObservable<bool> SequenceEqual<TSource>(IRefObservable<TSource> first, IRefObservable<TSource> second, IEqualityComparer<TSource> comparer);
        IRefObservable<bool> SequenceEqual<TSource>(IRefObservable<TSource> first, IEnumerable<TSource> second);
        IRefObservable<bool> SequenceEqual<TSource>(IRefObservable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer);
        IRefObservable<TSource> SingleAsync<TSource>(IRefObservable<TSource> source);
        IRefObservable<TSource> SingleAsync<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate);
        IRefObservable<TSource?> SingleOrDefaultAsync<TSource>(IRefObservable<TSource> source);
        IRefObservable<TSource?> SingleOrDefaultAsync<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate);
        IRefObservable<double> Sum(IRefObservable<double> source);
        IRefObservable<float> Sum(IRefObservable<float> source);
        IRefObservable<decimal> Sum(IRefObservable<decimal> source);
        IRefObservable<int> Sum(IRefObservable<int> source);
        IRefObservable<long> Sum(IRefObservable<long> source);
        IRefObservable<double?> Sum(IRefObservable<double?> source);
        IRefObservable<float?> Sum(IRefObservable<float?> source);
        IRefObservable<decimal?> Sum(IRefObservable<decimal?> source);
        IRefObservable<int?> Sum(IRefObservable<int?> source);
        IRefObservable<long?> Sum(IRefObservable<long?> source);
        IRefObservable<double> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, double> selector);
        IRefObservable<float> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, float> selector);
        IRefObservable<decimal> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, decimal> selector);
        IRefObservable<int> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, int> selector);
        IRefObservable<long> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, long> selector);
        IRefObservable<double?> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, double?> selector);
        IRefObservable<float?> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, float?> selector);
        IRefObservable<decimal?> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, decimal?> selector);
        IRefObservable<int?> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, int?> selector);
        IRefObservable<long?> Sum<TSource>(IRefObservable<TSource> source, Func<TSource, long?> selector);
        IRefObservable<TSource[]> ToArray<TSource>(IRefObservable<TSource> source);
        IRefObservable<IDictionary<TKey, TElement>> ToDictionary<TSource, TKey, TElement>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer) where TKey : notnull;
        IRefObservable<IDictionary<TKey, TElement>> ToDictionary<TSource, TKey, TElement>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) where TKey : notnull;
        IRefObservable<IDictionary<TKey, TSource>> ToDictionary<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) where TKey : notnull;
        IRefObservable<IDictionary<TKey, TSource>> ToDictionary<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector) where TKey : notnull;
        IRefObservable<IList<TSource>> ToList<TSource>(IRefObservable<TSource> source);
        IRefObservable<ILookup<TKey, TElement>> ToLookup<TSource, TKey, TElement>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer);
        IRefObservable<ILookup<TKey, TSource>> ToLookup<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer);
        IRefObservable<ILookup<TKey, TElement>> ToLookup<TSource, TKey, TElement>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector);
        IRefObservable<ILookup<TKey, TSource>> ToLookup<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector);

        #endregion

        #region * Async *

        Func<IRefObservable<TResult>> FromAsyncPattern<TResult>(Func<AsyncCallback, object?, IAsyncResult> begin, Func<IAsyncResult, TResult> end);
        Func<T1, IRefObservable<TResult>> FromAsyncPattern<T1, TResult>(Func<T1, AsyncCallback, object?, IAsyncResult> begin, Func<IAsyncResult, TResult> end);
        Func<T1, T2, IRefObservable<TResult>> FromAsyncPattern<T1, T2, TResult>(Func<T1, T2, AsyncCallback, object?, IAsyncResult> begin, Func<IAsyncResult, TResult> end);
        Func<T1, T2, T3, IRefObservable<TResult>> FromAsyncPattern<T1, T2, T3, TResult>(Func<T1, T2, T3, AsyncCallback, object?, IAsyncResult> begin, Func<IAsyncResult, TResult> end);
        Func<T1, T2, T3, T4, IRefObservable<TResult>> FromAsyncPattern<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, AsyncCallback, object?, IAsyncResult> begin, Func<IAsyncResult, TResult> end);
        Func<T1, T2, T3, T4, T5, IRefObservable<TResult>> FromAsyncPattern<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, AsyncCallback, object?, IAsyncResult> begin, Func<IAsyncResult, TResult> end);
        Func<T1, T2, T3, T4, T5, T6, IRefObservable<TResult>> FromAsyncPattern<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, AsyncCallback, object?, IAsyncResult> begin, Func<IAsyncResult, TResult> end);
        Func<T1, T2, T3, T4, T5, T6, T7, IRefObservable<TResult>> FromAsyncPattern<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, AsyncCallback, object?, IAsyncResult> begin, Func<IAsyncResult, TResult> end);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, IRefObservable<TResult>> FromAsyncPattern<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, AsyncCallback, object?, IAsyncResult> begin, Func<IAsyncResult, TResult> end);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, IRefObservable<TResult>> FromAsyncPattern<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, AsyncCallback, object?, IAsyncResult> begin, Func<IAsyncResult, TResult> end);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, IRefObservable<TResult>> FromAsyncPattern<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, AsyncCallback, object?, IAsyncResult> begin, Func<IAsyncResult, TResult> end);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, IRefObservable<TResult>> FromAsyncPattern<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, AsyncCallback, object?, IAsyncResult> begin, Func<IAsyncResult, TResult> end);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, IRefObservable<TResult>> FromAsyncPattern<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, AsyncCallback, object?, IAsyncResult> begin, Func<IAsyncResult, TResult> end);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, IRefObservable<TResult>> FromAsyncPattern<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, AsyncCallback, object?, IAsyncResult> begin, Func<IAsyncResult, TResult> end);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, IRefObservable<TResult>> FromAsyncPattern<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, AsyncCallback, object?, IAsyncResult> begin, Func<IAsyncResult, TResult> end);

        Func<IRefObservable<Unit>> FromAsyncPattern(Func<AsyncCallback, object?, IAsyncResult> begin, Action<IAsyncResult> end);
        Func<T1, IRefObservable<Unit>> FromAsyncPattern<T1>(Func<T1, AsyncCallback, object?, IAsyncResult> begin, Action<IAsyncResult> end);
        Func<T1, T2, IRefObservable<Unit>> FromAsyncPattern<T1, T2>(Func<T1, T2, AsyncCallback, object?, IAsyncResult> begin, Action<IAsyncResult> end);
        Func<T1, T2, T3, IRefObservable<Unit>> FromAsyncPattern<T1, T2, T3>(Func<T1, T2, T3, AsyncCallback, object?, IAsyncResult> begin, Action<IAsyncResult> end);
        Func<T1, T2, T3, T4, IRefObservable<Unit>> FromAsyncPattern<T1, T2, T3, T4>(Func<T1, T2, T3, T4, AsyncCallback, object?, IAsyncResult> begin, Action<IAsyncResult> end);
        Func<T1, T2, T3, T4, T5, IRefObservable<Unit>> FromAsyncPattern<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, AsyncCallback, object?, IAsyncResult> begin, Action<IAsyncResult> end);
        Func<T1, T2, T3, T4, T5, T6, IRefObservable<Unit>> FromAsyncPattern<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, AsyncCallback, object?, IAsyncResult> begin, Action<IAsyncResult> end);
        Func<T1, T2, T3, T4, T5, T6, T7, IRefObservable<Unit>> FromAsyncPattern<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, AsyncCallback, object?, IAsyncResult> begin, Action<IAsyncResult> end);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, IRefObservable<Unit>> FromAsyncPattern<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, AsyncCallback, object?, IAsyncResult> begin, Action<IAsyncResult> end);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, IRefObservable<Unit>> FromAsyncPattern<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, AsyncCallback, object?, IAsyncResult> begin, Action<IAsyncResult> end);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, IRefObservable<Unit>> FromAsyncPattern<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, AsyncCallback, object?, IAsyncResult> begin, Action<IAsyncResult> end);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, IRefObservable<Unit>> FromAsyncPattern<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, AsyncCallback, object?, IAsyncResult> begin, Action<IAsyncResult> end);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, IRefObservable<Unit>> FromAsyncPattern<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, AsyncCallback, object?, IAsyncResult> begin, Action<IAsyncResult> end);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, IRefObservable<Unit>> FromAsyncPattern<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, AsyncCallback, object?, IAsyncResult> begin, Action<IAsyncResult> end);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, IRefObservable<Unit>> FromAsyncPattern<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, AsyncCallback, object?, IAsyncResult> begin, Action<IAsyncResult> end);

        IRefObservable<TSource> Start<TSource>(Func<TSource> function);
        IRefObservable<TSource> Start<TSource>(Func<TSource> function, IScheduler scheduler);

        IRefObservable<TSource> StartAsync<TSource>(Func<Task<TSource>> functionAsync);
        IRefObservable<TSource> StartAsync<TSource>(Func<CancellationToken, Task<TSource>> functionAsync);
        IRefObservable<TSource> StartAsync<TSource>(Func<Task<TSource>> functionAsync, IScheduler scheduler);
        IRefObservable<TSource> StartAsync<TSource>(Func<CancellationToken, Task<TSource>> functionAsync, IScheduler scheduler);

        IRefObservable<Unit> Start(Action action);
        IRefObservable<Unit> Start(Action action, IScheduler scheduler);

        IRefObservable<Unit> StartAsync(Func<Task> actionAsync);
        IRefObservable<Unit> StartAsync(Func<CancellationToken, Task> actionAsync);
        IRefObservable<Unit> StartAsync(Func<Task> actionAsync, IScheduler scheduler);
        IRefObservable<Unit> StartAsync(Func<CancellationToken, Task> actionAsync, IScheduler scheduler);

        IRefObservable<TResult> FromAsync<TResult>(Func<Task<TResult>> functionAsync);
        IRefObservable<TResult> FromAsync<TResult>(Func<CancellationToken, Task<TResult>> functionAsync);
        IRefObservable<Unit> FromAsync(Func<Task> actionAsync);
        IRefObservable<Unit> FromAsync(Func<CancellationToken, Task> actionAsync);
        IRefObservable<TResult> FromAsync<TResult>(Func<Task<TResult>> functionAsync, IScheduler scheduler);
        IRefObservable<TResult> FromAsync<TResult>(Func<CancellationToken, Task<TResult>> functionAsync, IScheduler scheduler);
        IRefObservable<Unit> FromAsync(Func<Task> actionAsync, IScheduler scheduler);
        IRefObservable<Unit> FromAsync(Func<CancellationToken, Task> actionAsync, IScheduler scheduler);

        Func<IRefObservable<TResult>> ToAsync<TResult>(Func<TResult> function);
        Func<IRefObservable<TResult>> ToAsync<TResult>(Func<TResult> function, IScheduler scheduler);
        Func<T, IRefObservable<TResult>> ToAsync<T, TResult>(Func<T, TResult> function);
        Func<T, IRefObservable<TResult>> ToAsync<T, TResult>(Func<T, TResult> function, IScheduler scheduler);
        Func<T1, T2, IRefObservable<TResult>> ToAsync<T1, T2, TResult>(Func<T1, T2, TResult> function);
        Func<T1, T2, IRefObservable<TResult>> ToAsync<T1, T2, TResult>(Func<T1, T2, TResult> function, IScheduler scheduler);
        Func<T1, T2, T3, IRefObservable<TResult>> ToAsync<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function);
        Func<T1, T2, T3, IRefObservable<TResult>> ToAsync<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function, IScheduler scheduler);
        Func<T1, T2, T3, T4, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> function);
        Func<T1, T2, T3, T4, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> function, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> function);
        Func<T1, T2, T3, T4, T5, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> function, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> function);
        Func<T1, T2, T3, T4, T5, T6, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> function, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> function);
        Func<T1, T2, T3, T4, T5, T6, T7, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> function, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> function);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> function, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> function);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> function, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> function);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> function, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> function);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> function, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> function);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> function, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> function);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> function, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> function);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> function, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> function);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> function, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> function);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, IRefObservable<TResult>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> function, IScheduler scheduler);

        Func<IRefObservable<Unit>> ToAsync(Action action);
        Func<IRefObservable<Unit>> ToAsync(Action action, IScheduler scheduler);
        Func<TSource, IRefObservable<Unit>> ToAsync<TSource>(Action<TSource> action);
        Func<TSource, IRefObservable<Unit>> ToAsync<TSource>(Action<TSource> action, IScheduler scheduler);
        Func<T1, T2, IRefObservable<Unit>> ToAsync<T1, T2>(Action<T1, T2> action);
        Func<T1, T2, IRefObservable<Unit>> ToAsync<T1, T2>(Action<T1, T2> action, IScheduler scheduler);
        Func<T1, T2, T3, IRefObservable<Unit>> ToAsync<T1, T2, T3>(Action<T1, T2, T3> action);
        Func<T1, T2, T3, IRefObservable<Unit>> ToAsync<T1, T2, T3>(Action<T1, T2, T3> action, IScheduler scheduler);
        Func<T1, T2, T3, T4, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action);
        Func<T1, T2, T3, T4, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action);
        Func<T1, T2, T3, T4, T5, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action);
        Func<T1, T2, T3, T4, T5, T6, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action);
        Func<T1, T2, T3, T4, T5, T6, T7, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, IScheduler scheduler);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action);
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, IRefObservable<Unit>> ToAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, IScheduler scheduler);

        #endregion

        #region * Awaiter *

        AsyncSubject<TSource> GetAwaiter<TSource>(IRefObservable<TSource> source);
        AsyncSubject<TSource> GetAwaiter<TSource>(IConnectableObservable<TSource> source);
        AsyncSubject<TSource> RunAsync<TSource>(IRefObservable<TSource> source, CancellationToken cancellationToken);
        AsyncSubject<TSource> RunAsync<TSource>(IConnectableObservable<TSource> source, CancellationToken cancellationToken);

        #endregion

        #region * Binding *

        IConnectableObservable<TResult> Multicast<TSource, TResult>(IRefObservable<TSource> source, ISubject<TSource, TResult> subject);
        IRefObservable<TResult> Multicast<TSource, TIntermediate, TResult>(IRefObservable<TSource> source, Func<ISubject<TSource, TIntermediate>> subjectSelector, Func<IRefObservable<TIntermediate>, IRefObservable<TResult>> selector);
        IConnectableObservable<TSource> Publish<TSource>(IRefObservable<TSource> source);
        IRefObservable<TResult> Publish<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector);
        IConnectableObservable<TSource> Publish<TSource>(IRefObservable<TSource> source, TSource initialValue);
        IRefObservable<TResult> Publish<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector, TSource initialValue);
        IConnectableObservable<TSource> PublishLast<TSource>(IRefObservable<TSource> source);
        IRefObservable<TResult> PublishLast<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector);
        IRefObservable<TSource> RefCount<TSource>(IConnectableObservable<TSource> source);
        IRefObservable<TSource> RefCount<TSource>(IConnectableObservable<TSource> source, TimeSpan disconnectDelay);
        IRefObservable<TSource> RefCount<TSource>(IConnectableObservable<TSource> source, TimeSpan disconnectDelay, IScheduler schedulder);
        IRefObservable<TSource> RefCount<TSource>(IConnectableObservable<TSource> source, int minObservers);
        IRefObservable<TSource> RefCount<TSource>(IConnectableObservable<TSource> source, int minObservers, TimeSpan disconnectDelay);
        IRefObservable<TSource> RefCount<TSource>(IConnectableObservable<TSource> source, int minObservers, TimeSpan disconnectDelay, IScheduler schedulder);
        IConnectableObservable<TSource> Replay<TSource>(IRefObservable<TSource> source);
        IConnectableObservable<TSource> Replay<TSource>(IRefObservable<TSource> source, IScheduler scheduler);
        IRefObservable<TResult> Replay<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector);
        IRefObservable<TResult> Replay<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector, IScheduler scheduler);
        IConnectableObservable<TSource> Replay<TSource>(IRefObservable<TSource> source, TimeSpan window);
        IRefObservable<TResult> Replay<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector, TimeSpan window);
        IConnectableObservable<TSource> Replay<TSource>(IRefObservable<TSource> source, TimeSpan window, IScheduler scheduler);
        IRefObservable<TResult> Replay<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector, TimeSpan window, IScheduler scheduler);
        IConnectableObservable<TSource> Replay<TSource>(IRefObservable<TSource> source, int bufferSize, IScheduler scheduler);
        IRefObservable<TResult> Replay<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector, int bufferSize, IScheduler scheduler);
        IConnectableObservable<TSource> Replay<TSource>(IRefObservable<TSource> source, int bufferSize);
        IRefObservable<TResult> Replay<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector, int bufferSize);
        IConnectableObservable<TSource> Replay<TSource>(IRefObservable<TSource> source, int bufferSize, TimeSpan window);
        IRefObservable<TResult> Replay<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector, int bufferSize, TimeSpan window);
        IConnectableObservable<TSource> Replay<TSource>(IRefObservable<TSource> source, int bufferSize, TimeSpan window, IScheduler scheduler);
        IRefObservable<TResult> Replay<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> selector, int bufferSize, TimeSpan window, IScheduler scheduler);
        IRefObservable<TSource> AutoConnect<TSource>(IConnectableObservable<TSource> source, int minObservers, Action<IDisposable>? onConnect);

        #endregion

        #region * Blocking *

        IEnumerable<IList<TSource>> Chunkify<TSource>(IRefObservable<TSource> source);
        IEnumerable<TResult> Collect<TSource, TResult>(IRefObservable<TSource> source, Func<TResult> newCollector, Func<TResult, TSource, TResult> merge);
        IEnumerable<TResult> Collect<TSource, TResult>(IRefObservable<TSource> source, Func<TResult> getInitialCollector, Func<TResult, TSource, TResult> merge, Func<TResult, TResult> getNewCollector);
        TSource First<TSource>(IRefObservable<TSource> source);
        TSource First<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate);
        [return: MaybeNull] TSource FirstOrDefault<TSource>(IRefObservable<TSource> source);
        [return: MaybeNull] TSource FirstOrDefault<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate);
        void ForEach<TSource>(IRefObservable<TSource> source, Action<TSource> onNext);
        void ForEach<TSource>(IRefObservable<TSource> source, Action<TSource, int> onNext);
        IEnumerator<TSource> GetEnumerator<TSource>(IRefObservable<TSource> source);
        TSource Last<TSource>(IRefObservable<TSource> source);
        TSource Last<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate);
        [return: MaybeNull] TSource LastOrDefault<TSource>(IRefObservable<TSource> source);
        [return: MaybeNull] TSource LastOrDefault<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate);
        IEnumerable<TSource> Latest<TSource>(IRefObservable<TSource> source);
        IEnumerable<TSource> MostRecent<TSource>(IRefObservable<TSource> source, TSource initialValue);
        IEnumerable<TSource> Next<TSource>(IRefObservable<TSource> source);
        TSource Single<TSource>(IRefObservable<TSource> source);
        [return: MaybeNull] TSource SingleOrDefault<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate);
        [return: MaybeNull] TSource SingleOrDefault<TSource>(IRefObservable<TSource> source);
        TSource Single<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate);
        TSource Wait<TSource>(IRefObservable<TSource> source);

        #endregion

        #region * Concurrency *

        IRefObservable<TSource> ObserveOn<TSource>(IRefObservable<TSource> source, IScheduler scheduler);
        IRefObservable<TSource> ObserveOn<TSource>(IRefObservable<TSource> source, SynchronizationContext context);

        IRefObservable<TSource> SubscribeOn<TSource>(IRefObservable<TSource> source, IScheduler scheduler);
        IRefObservable<TSource> SubscribeOn<TSource>(IRefObservable<TSource> source, SynchronizationContext context);

        IRefObservable<TSource> Synchronize<TSource>(IRefObservable<TSource> source);
        IRefObservable<TSource> Synchronize<TSource>(IRefObservable<TSource> source, object gate);

        #endregion

        #region * Conversions *

        IDisposable Subscribe<TSource>(IEnumerable<TSource> source, IRefObserver<TSource> observer);
        IDisposable Subscribe<TSource>(IEnumerable<TSource> source, IRefObserver<TSource> observer, IScheduler scheduler);
        IEnumerable<TSource> ToEnumerable<TSource>(IRefObservable<TSource> source);
        IEventSource<Unit> ToEvent(IRefObservable<Unit> source);
        IEventSource<TSource> ToEvent<TSource>(IRefObservable<TSource> source);
        IRefObservable<TSource> ToObservable<TSource>(IEnumerable<TSource> source);
        IRefObservable<TSource> ToObservable<TSource>(IEnumerable<TSource> source, IScheduler scheduler);

        #endregion

        #region * Creation *

        IRefObservable<TSource> Create<TSource>(Func<IRefObserver<TSource>, IDisposable> subscribe);
        IRefObservable<TSource> Create<TSource>(Func<IRefObserver<TSource>, Action> subscribe);
        IRefObservable<TResult> Create<TResult>(Func<IRefObserver<TResult>, CancellationToken, Task> subscribeAsync);
        IRefObservable<TResult> Create<TResult>(Func<IRefObserver<TResult>, Task> subscribeAsync);
        IRefObservable<TResult> Create<TResult>(Func<IRefObserver<TResult>, CancellationToken, Task<IDisposable>> subscribeAsync);
        IRefObservable<TResult> Create<TResult>(Func<IRefObserver<TResult>, Task<IDisposable>> subscribeAsync);
        IRefObservable<TResult> Create<TResult>(Func<IRefObserver<TResult>, CancellationToken, Task<Action>> subscribeAsync);
        IRefObservable<TResult> Create<TResult>(Func<IRefObserver<TResult>, Task<Action>> subscribeAsync);

        IRefObservable<TValue> Defer<TValue>(Func<IRefObservable<TValue>> observableFactory);

        IRefObservable<TValue> Defer<TValue>(Func<Task<IRefObservable<TValue>>> observableFactoryAsync);
        IRefObservable<TValue> Defer<TValue>(Func<CancellationToken, Task<IRefObservable<TValue>>> observableFactoryAsync);

        IRefObservable<TResult> Empty<TResult>();
        IRefObservable<TResult> Empty<TResult>(IScheduler scheduler);
        IRefObservable<TResult> Generate<TState, TResult>(TState initialState, Func<TState, bool> condition, Func<TState, TState> iterate, Func<TState, TResult> resultSelector);
        IRefObservable<TResult> Generate<TState, TResult>(TState initialState, Func<TState, bool> condition, Func<TState, TState> iterate, Func<TState, TResult> resultSelector, IScheduler scheduler);
        IRefObservable<TResult> Never<TResult>();
        IRefObservable<int> Range(int start, int count);
        IRefObservable<int> Range(int start, int count, IScheduler scheduler);
        IRefObservable<TResult> Repeat<TResult>(TResult value);
        IRefObservable<TResult> Repeat<TResult>(TResult value, IScheduler scheduler);
        IRefObservable<TResult> Repeat<TResult>(TResult value, int repeatCount);
        IRefObservable<TResult> Repeat<TResult>(TResult value, int repeatCount, IScheduler scheduler);
        IRefObservable<TResult> Return<TResult>(TResult value);
        IRefObservable<TResult> Return<TResult>(TResult value, IScheduler scheduler);
        IRefObservable<TResult> Throw<TResult>(Exception exception);
        IRefObservable<TResult> Throw<TResult>(Exception exception, IScheduler scheduler);
        IRefObservable<TSource> Using<TSource, TResource>(Func<TResource> resourceFactory, Func<TResource, IRefObservable<TSource>> observableFactory) where TResource : IDisposable;
        IRefObservable<TSource> Using<TSource, TResource>(Func<CancellationToken, Task<TResource>> resourceFactoryAsync, Func<TResource, CancellationToken, Task<IRefObservable<TSource>>> observableFactoryAsync) where TResource : IDisposable;

        #endregion

        #region * Events *

        IRefObservable<EventPattern<object>> FromEventPattern(Action<EventHandler> addHandler, Action<EventHandler> removeHandler);
        IRefObservable<EventPattern<object>> FromEventPattern(Action<EventHandler> addHandler, Action<EventHandler> removeHandler, IScheduler scheduler);
        IRefObservable<EventPattern<TEventArgs>> FromEventPattern<TDelegate, TEventArgs>(Action<TDelegate> addHandler, Action<TDelegate> removeHandler);
        IRefObservable<EventPattern<TEventArgs>> FromEventPattern<TDelegate, TEventArgs>(Action<TDelegate> addHandler, Action<TDelegate> removeHandler, IScheduler scheduler);
        IRefObservable<EventPattern<TEventArgs>> FromEventPattern<TDelegate, TEventArgs>(Func<EventHandler<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler);
        IRefObservable<EventPattern<TEventArgs>> FromEventPattern<TDelegate, TEventArgs>(Func<EventHandler<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler, IScheduler scheduler);
        IRefObservable<EventPattern<TEventArgs>> FromEventPattern<TEventArgs>(Action<EventHandler<TEventArgs>> addHandler, Action<EventHandler<TEventArgs>> removeHandler);
        IRefObservable<EventPattern<TEventArgs>> FromEventPattern<TEventArgs>(Action<EventHandler<TEventArgs>> addHandler, Action<EventHandler<TEventArgs>> removeHandler, IScheduler scheduler);
        IRefObservable<EventPattern<TSender, TEventArgs>> FromEventPattern<TDelegate, TSender, TEventArgs>(Action<TDelegate> addHandler, Action<TDelegate> removeHandler);
        IRefObservable<EventPattern<TSender, TEventArgs>> FromEventPattern<TDelegate, TSender, TEventArgs>(Action<TDelegate> addHandler, Action<TDelegate> removeHandler, IScheduler scheduler);
        IRefObservable<EventPattern<object>> FromEventPattern(object target, string eventName);
        IRefObservable<EventPattern<object>> FromEventPattern(object target, string eventName, IScheduler scheduler);
        IRefObservable<EventPattern<TEventArgs>> FromEventPattern<TEventArgs>(object target, string eventName);
        IRefObservable<EventPattern<TEventArgs>> FromEventPattern<TEventArgs>(object target, string eventName, IScheduler scheduler);
        IRefObservable<EventPattern<TSender, TEventArgs>> FromEventPattern<TSender, TEventArgs>(object target, string eventName);
        IRefObservable<EventPattern<TSender, TEventArgs>> FromEventPattern<TSender, TEventArgs>(object target, string eventName, IScheduler scheduler);
        IRefObservable<EventPattern<object>> FromEventPattern(Type type, string eventName);
        IRefObservable<EventPattern<object>> FromEventPattern(Type type, string eventName, IScheduler scheduler);
        IRefObservable<EventPattern<TEventArgs>> FromEventPattern<TEventArgs>(Type type, string eventName);
        IRefObservable<EventPattern<TEventArgs>> FromEventPattern<TEventArgs>(Type type, string eventName, IScheduler scheduler);
        IRefObservable<EventPattern<TSender, TEventArgs>> FromEventPattern<TSender, TEventArgs>(Type type, string eventName);
        IRefObservable<EventPattern<TSender, TEventArgs>> FromEventPattern<TSender, TEventArgs>(Type type, string eventName, IScheduler scheduler);

        IRefObservable<TEventArgs> FromEvent<TDelegate, TEventArgs>(Func<RefAction<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler);
        IRefObservable<TEventArgs> FromEvent<TDelegate, TEventArgs>(Func<RefAction<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler, Action<TDelegate> removeHandler, IScheduler scheduler);
        IRefObservable<TEventArgs> FromEvent<TDelegate, TEventArgs>(Action<TDelegate> addHandler, Action<TDelegate> removeHandler);
        IRefObservable<TEventArgs> FromEvent<TDelegate, TEventArgs>(Action<TDelegate> addHandler, Action<TDelegate> removeHandler, IScheduler scheduler);
        IRefObservable<TEventArgs> FromEvent<TEventArgs>(Action<RefAction<TEventArgs>> addHandler, Action<RefAction<TEventArgs>> removeHandler);
        IRefObservable<TEventArgs> FromEvent<TEventArgs>(Action<RefAction<TEventArgs>> addHandler, Action<RefAction<TEventArgs>> removeHandler, IScheduler scheduler);
        IRefObservable<Unit> FromEvent(Action<Action> addHandler, Action<Action> removeHandler);
        IRefObservable<Unit> FromEvent(Action<Action> addHandler, Action<Action> removeHandler, IScheduler scheduler);

        #endregion

        #region * Imperative *

        Task ForEachAsync<TSource>(IRefObservable<TSource> source, Action<TSource> onNext);
        Task ForEachAsync<TSource>(IRefObservable<TSource> source, Action<TSource> onNext, CancellationToken cancellationToken);
        Task ForEachAsync<TSource>(IRefObservable<TSource> source, Action<TSource, int> onNext);
        Task ForEachAsync<TSource>(IRefObservable<TSource> source, Action<TSource, int> onNext, CancellationToken cancellationToken);

        IRefObservable<TResult> Case<TValue, TResult>(Func<TValue> selector, IDictionary<TValue, IRefObservable<TResult>> sources, IRefObservable<TResult> defaultSource) where TValue : notnull;
        IRefObservable<TResult> Case<TValue, TResult>(Func<TValue> selector, IDictionary<TValue, IRefObservable<TResult>> sources, IScheduler scheduler) where TValue : notnull;
        IRefObservable<TResult> Case<TValue, TResult>(Func<TValue> selector, IDictionary<TValue, IRefObservable<TResult>> sources) where TValue : notnull;
        IRefObservable<TSource> DoWhile<TSource>(IRefObservable<TSource> source, Func<bool> condition);
        IRefObservable<TResult> For<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, IRefObservable<TResult>> resultSelector);
        IRefObservable<TResult> If<TResult>(Func<bool> condition, IRefObservable<TResult> thenSource, IRefObservable<TResult> elseSource);
        IRefObservable<TResult> If<TResult>(Func<bool> condition, IRefObservable<TResult> thenSource);
        IRefObservable<TResult> If<TResult>(Func<bool> condition, IRefObservable<TResult> thenSource, IScheduler scheduler);
        IRefObservable<TSource> While<TSource>(Func<bool> condition, IRefObservable<TSource> source);

        #endregion

        #region * Joins *

        Pattern<TLeft, TRight> And<TLeft, TRight>(IRefObservable<TLeft> left, IRefObservable<TRight> right);
        Plan<TResult> Then<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, TResult> selector);
        IRefObservable<TResult> When<TResult>(params Plan<TResult>[] plans);
        IRefObservable<TResult> When<TResult>(IEnumerable<Plan<TResult>> plans);

        #endregion

        #region * Multiple *

        IRefObservable<TSource> Amb<TSource>(IRefObservable<TSource> first, IRefObservable<TSource> second);
        IRefObservable<TSource> Amb<TSource>(params IRefObservable<TSource>[] sources);
        IRefObservable<TSource> Amb<TSource>(IEnumerable<IRefObservable<TSource>> sources);
        IRefObservable<IList<TSource>> Buffer<TSource, TBufferClosing>(IRefObservable<TSource> source, Func<IRefObservable<TBufferClosing>> bufferClosingSelector);
        IRefObservable<IList<TSource>> Buffer<TSource, TBufferOpening, TBufferClosing>(IRefObservable<TSource> source, IRefObservable<TBufferOpening> bufferOpenings, Func<TBufferOpening, IRefObservable<TBufferClosing>> bufferClosingSelector);
        IRefObservable<IList<TSource>> Buffer<TSource, TBufferBoundary>(IRefObservable<TSource> source, IRefObservable<TBufferBoundary> bufferBoundaries);
        IRefObservable<TSource> Catch<TSource, TException>(IRefObservable<TSource> source, Func<TException, IRefObservable<TSource>> handler) where TException : Exception;
        IRefObservable<TSource> Catch<TSource>(IRefObservable<TSource> first, IRefObservable<TSource> second);
        IRefObservable<TSource> Catch<TSource>(params IRefObservable<TSource>[] sources);
        IRefObservable<TSource> Catch<TSource>(IEnumerable<IRefObservable<TSource>> sources);
        // NB: N-ary overloads of CombineLatest are generated in IQueryLanguage.tt.
        IRefObservable<TResult> CombineLatest<TSource, TResult>(IEnumerable<IRefObservable<TSource>> sources, Func<IList<TSource>, TResult> resultSelector);
        IRefObservable<List<TSource>> CombineLatest<TSource>(IEnumerable<IRefObservable<TSource>> sources);
        IRefObservable<List<TSource>> CombineLatest<TSource>(params IRefObservable<TSource>[] sources);
        IRefObservable<TSource> Concat<TSource>(IRefObservable<TSource> first, IRefObservable<TSource> second);
        IRefObservable<TSource> Concat<TSource>(params IRefObservable<TSource>[] sources);
        IRefObservable<TSource> Concat<TSource>(IEnumerable<IRefObservable<TSource>> sources);
        IRefObservable<TSource> Concat<TSource>(IRefObservable<IRefObservable<TSource>> sources);
        IRefObservable<TSource> Merge<TSource>(IRefObservable<IRefObservable<TSource>> sources);
        IRefObservable<TSource> Merge<TSource>(IRefObservable<IRefObservable<TSource>> sources, int maxConcurrent);
        IRefObservable<TSource> Merge<TSource>(IEnumerable<IRefObservable<TSource>> sources, int maxConcurrent);
        IRefObservable<TSource> Merge<TSource>(IEnumerable<IRefObservable<TSource>> sources, int maxConcurrent, IScheduler scheduler);
        IRefObservable<TSource> Merge<TSource>(IRefObservable<TSource> first, IRefObservable<TSource> second);
        IRefObservable<TSource> Merge<TSource>(IRefObservable<TSource> first, IRefObservable<TSource> second, IScheduler scheduler);
        IRefObservable<TSource> Merge<TSource>(params IRefObservable<TSource>[] sources);
        IRefObservable<TSource> Merge<TSource>(IScheduler scheduler, params IRefObservable<TSource>[] sources);
        IRefObservable<TSource> Merge<TSource>(IEnumerable<IRefObservable<TSource>> sources);
        IRefObservable<TSource> Merge<TSource>(IEnumerable<IRefObservable<TSource>> sources, IScheduler scheduler);
        IRefObservable<TSource> OnErrorResumeNext<TSource>(IRefObservable<TSource> first, IRefObservable<TSource> second);
        IRefObservable<TSource> OnErrorResumeNext<TSource>(params IRefObservable<TSource>[] sources);
        IRefObservable<TSource> OnErrorResumeNext<TSource>(IEnumerable<IRefObservable<TSource>> sources);
        IRefObservable<TSource> SkipUntil<TSource, TOther>(IRefObservable<TSource> source, IRefObservable<TOther> other);
        IRefObservable<TSource> Switch<TSource>(IRefObservable<IRefObservable<TSource>> sources);
        IRefObservable<TSource> TakeUntil<TSource, TOther>(IRefObservable<TSource> source, IRefObservable<TOther> other);
        IRefObservable<TSource> TakeUntil<TSource>(IRefObservable<TSource> source, Func<TSource, bool> stopPredicate);
        IRefObservable<IRefObservable<TSource>> Window<TSource, TWindowClosing>(IRefObservable<TSource> source, Func<IRefObservable<TWindowClosing>> windowClosingSelector);
        IRefObservable<IRefObservable<TSource>> Window<TSource, TWindowOpening, TWindowClosing>(IRefObservable<TSource> source, IRefObservable<TWindowOpening> windowOpenings, Func<TWindowOpening, IRefObservable<TWindowClosing>> windowClosingSelector);
        IRefObservable<IRefObservable<TSource>> Window<TSource, TWindowBoundary>(IRefObservable<TSource> source, IRefObservable<TWindowBoundary> windowBoundaries);
        IRefObservable<TResult> WithLatestFrom<TFirst, TSecond, TResult>(IRefObservable<TFirst> first, IRefObservable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector);
        IRefObservable<TResult> Zip<TSource, TResult>(IEnumerable<IRefObservable<TSource>> sources, Func<IList<TSource>, TResult> resultSelector);
        IRefObservable<IList<TSource>> Zip<TSource>(IEnumerable<IRefObservable<TSource>> sources);
        IRefObservable<IList<TSource>> Zip<TSource>(params IRefObservable<TSource>[] sources);
        // NB: N-ary overloads of Zip are generated in IQueryLanguage.tt.
        IRefObservable<TResult> Zip<TFirst, TSecond, TResult>(IRefObservable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector);

        IRefObservable<TSource> Concat<TSource>(IRefObservable<Task<TSource>> sources);
        IRefObservable<TSource> Merge<TSource>(IRefObservable<Task<TSource>> sources);
        IRefObservable<TSource> Switch<TSource>(IRefObservable<Task<TSource>> sources);

        #endregion

        #region * Single *

        IRefObservable<TSource> Append<TSource>(IRefObservable<TSource> source, TSource value);
        IRefObservable<TSource> Append<TSource>(IRefObservable<TSource> source, TSource value, IScheduler scheduler);
        IRefObservable<TSource> AsObservable<TSource>(IRefObservable<TSource> source);
        IRefObservable<IList<TSource>> Buffer<TSource>(IRefObservable<TSource> source, int count);
        IRefObservable<IList<TSource>> Buffer<TSource>(IRefObservable<TSource> source, int count, int skip);
        IRefObservable<TSource> Dematerialize<TSource>(IRefObservable<Notification<TSource>> source);
        IRefObservable<TSource> DistinctUntilChanged<TSource>(IRefObservable<TSource> source);
        IRefObservable<TSource> DistinctUntilChanged<TSource>(IRefObservable<TSource> source, IEqualityComparer<TSource> comparer);
        IRefObservable<TSource> DistinctUntilChanged<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector);
        IRefObservable<TSource> DistinctUntilChanged<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer);
        IRefObservable<TSource> Do<TSource>(IRefObservable<TSource> source, Action<TSource> onNext);
        IRefObservable<TSource> Do<TSource>(IRefObservable<TSource> source, Action<TSource> onNext, Action onCompleted);
        IRefObservable<TSource> Do<TSource>(IRefObservable<TSource> source, Action<TSource> onNext, Action<Exception> onError);
        IRefObservable<TSource> Do<TSource>(IRefObservable<TSource> source, Action<TSource> onNext, Action<Exception> onError, Action onCompleted);
        IRefObservable<TSource> Do<TSource>(IRefObservable<TSource> source, IRefObserver<TSource> observer);
        IRefObservable<TSource> Finally<TSource>(IRefObservable<TSource> source, Action finallyAction);
        IRefObservable<TSource> IgnoreElements<TSource>(IRefObservable<TSource> source);
        IRefObservable<Notification<TSource>> Materialize<TSource>(IRefObservable<TSource> source);
        IRefObservable<TSource> Prepend<TSource>(IRefObservable<TSource> source, TSource value);
        IRefObservable<TSource> Prepend<TSource>(IRefObservable<TSource> source, TSource value, IScheduler scheduler);
        IRefObservable<TSource> Repeat<TSource>(IRefObservable<TSource> source);
        IRefObservable<TSource> Repeat<TSource>(IRefObservable<TSource> source, int repeatCount);
        IRefObservable<TSource> RepeatWhen<TSource, TSignal>(IRefObservable<TSource> source, Func<IRefObservable<object>, IRefObservable<TSignal>> handler);
        IRefObservable<TSource> Retry<TSource>(IRefObservable<TSource> source);
        IRefObservable<TSource> Retry<TSource>(IRefObservable<TSource> source, int retryCount);
        IRefObservable<TSource> RetryWhen<TSource, TSignal>(IRefObservable<TSource> source, Func<IRefObservable<Exception>, IRefObservable<TSignal>> handler);
        IRefObservable<TAccumulate> Scan<TSource, TAccumulate>(IRefObservable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator);
        IRefObservable<TSource> Scan<TSource>(IRefObservable<TSource> source, Func<TSource, TSource, TSource> accumulator);
        IRefObservable<TSource> SkipLast<TSource>(IRefObservable<TSource> source, int count);
        IRefObservable<TSource> StartWith<TSource>(IRefObservable<TSource> source, params TSource[] values);
        IRefObservable<TSource> StartWith<TSource>(IRefObservable<TSource> source, IScheduler scheduler, params TSource[] values);
        IRefObservable<TSource> StartWith<TSource>(IRefObservable<TSource> source, IEnumerable<TSource> values);
        IRefObservable<TSource> StartWith<TSource>(IRefObservable<TSource> source, IScheduler scheduler, IEnumerable<TSource> values);
        IRefObservable<TSource> TakeLast<TSource>(IRefObservable<TSource> source, int count);
        IRefObservable<TSource> TakeLast<TSource>(IRefObservable<TSource> source, int count, IScheduler scheduler);
        IRefObservable<IList<TSource>> TakeLastBuffer<TSource>(IRefObservable<TSource> source, int count);
        IRefObservable<IRefObservable<TSource>> Window<TSource>(IRefObservable<TSource> source, int count, int skip);
        IRefObservable<IRefObservable<TSource>> Window<TSource>(IRefObservable<TSource> source, int count);

        #endregion

        #region * StandardSequenceOperators *

        IRefObservable<TResult> Cast<TResult>(IRefObservable<object> source);
        IRefObservable<TSource?> DefaultIfEmpty<TSource>(IRefObservable<TSource> source);
        IRefObservable<TSource> DefaultIfEmpty<TSource>(IRefObservable<TSource> source, TSource defaultValue);
        IRefObservable<TSource> Distinct<TSource>(IRefObservable<TSource> source);
        IRefObservable<TSource> Distinct<TSource>(IRefObservable<TSource> source, IEqualityComparer<TSource> comparer);
        IRefObservable<TSource> Distinct<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector);
        IRefObservable<TSource> Distinct<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer);
        IRefObservable<IGroupedObservable<TKey, TElement>> GroupBy<TSource, TKey, TElement>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector);
        IRefObservable<IGroupedObservable<TKey, TSource>> GroupBy<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer);
        IRefObservable<IGroupedObservable<TKey, TSource>> GroupBy<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector);
        IRefObservable<IGroupedObservable<TKey, TElement>> GroupBy<TSource, TKey, TElement>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer);
        IRefObservable<IGroupedObservable<TKey, TElement>> GroupBy<TSource, TKey, TElement>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, int capacity);
        IRefObservable<IGroupedObservable<TKey, TSource>> GroupBy<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, int capacity, IEqualityComparer<TKey> comparer);
        IRefObservable<IGroupedObservable<TKey, TSource>> GroupBy<TSource, TKey>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, int capacity);
        IRefObservable<IGroupedObservable<TKey, TElement>> GroupBy<TSource, TKey, TElement>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, int capacity, IEqualityComparer<TKey> comparer);
        IRefObservable<IGroupedObservable<TKey, TElement>> GroupByUntil<TSource, TKey, TElement, TDuration>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<IGroupedObservable<TKey, TElement>, IRefObservable<TDuration>> durationSelector, IEqualityComparer<TKey> comparer);
        IRefObservable<IGroupedObservable<TKey, TElement>> GroupByUntil<TSource, TKey, TElement, TDuration>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<IGroupedObservable<TKey, TElement>, IRefObservable<TDuration>> durationSelector);
        IRefObservable<IGroupedObservable<TKey, TSource>> GroupByUntil<TSource, TKey, TDuration>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<IGroupedObservable<TKey, TSource>, IRefObservable<TDuration>> durationSelector, IEqualityComparer<TKey> comparer);
        IRefObservable<IGroupedObservable<TKey, TSource>> GroupByUntil<TSource, TKey, TDuration>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<IGroupedObservable<TKey, TSource>, IRefObservable<TDuration>> durationSelector);
        IRefObservable<IGroupedObservable<TKey, TElement>> GroupByUntil<TSource, TKey, TElement, TDuration>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<IGroupedObservable<TKey, TElement>, IRefObservable<TDuration>> durationSelector, int capacity, IEqualityComparer<TKey> comparer);
        IRefObservable<IGroupedObservable<TKey, TElement>> GroupByUntil<TSource, TKey, TElement, TDuration>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<IGroupedObservable<TKey, TElement>, IRefObservable<TDuration>> durationSelector, int capacity);
        IRefObservable<IGroupedObservable<TKey, TSource>> GroupByUntil<TSource, TKey, TDuration>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<IGroupedObservable<TKey, TSource>, IRefObservable<TDuration>> durationSelector, int capacity, IEqualityComparer<TKey> comparer);
        IRefObservable<IGroupedObservable<TKey, TSource>> GroupByUntil<TSource, TKey, TDuration>(IRefObservable<TSource> source, Func<TSource, TKey> keySelector, Func<IGroupedObservable<TKey, TSource>, IRefObservable<TDuration>> durationSelector, int capacity);
        IRefObservable<TResult> GroupJoin<TLeft, TRight, TLeftDuration, TRightDuration, TResult>(IRefObservable<TLeft> left, IRefObservable<TRight> right, Func<TLeft, IRefObservable<TLeftDuration>> leftDurationSelector, Func<TRight, IRefObservable<TRightDuration>> rightDurationSelector, Func<TLeft, IRefObservable<TRight>, TResult> resultSelector);
        IRefObservable<TResult> Join<TLeft, TRight, TLeftDuration, TRightDuration, TResult>(IRefObservable<TLeft> left, IRefObservable<TRight> right, Func<TLeft, IRefObservable<TLeftDuration>> leftDurationSelector, Func<TRight, IRefObservable<TRightDuration>> rightDurationSelector, Func<TLeft, TRight, TResult> resultSelector);
        IRefObservable<TResult> OfType<TResult>(IRefObservable<object> source);
        IRefObservable<TResult> Select<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, TResult> selector);
        IRefObservable<TResult> Select<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, int, TResult> selector);
        IRefObservable<TOther> SelectMany<TSource, TOther>(IRefObservable<TSource> source, IRefObservable<TOther> other);
        IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TResult>> selector);
        IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, int, IRefObservable<TResult>> selector);
        IRefObservable<TResult> SelectMany<TSource, TCollection, TResult>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector);
        IRefObservable<TResult> SelectMany<TSource, TCollection, TResult>(IRefObservable<TSource> source, Func<TSource, int, IRefObservable<TCollection>> collectionSelector, Func<TSource, int, TCollection, int, TResult> resultSelector);
        IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TResult>> onNext, Func<Exception, IRefObservable<TResult>> onError, Func<IRefObservable<TResult>> onCompleted);
        IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, int, IRefObservable<TResult>> onNext, Func<Exception, IRefObservable<TResult>> onError, Func<IRefObservable<TResult>> onCompleted);
        IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, IEnumerable<TResult>> selector);
        IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector);
        IRefObservable<TResult> SelectMany<TSource, TCollection, TResult>(IRefObservable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector);
        IRefObservable<TResult> SelectMany<TSource, TCollection, TResult>(IRefObservable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, int, TCollection, int, TResult> resultSelector);
        IRefObservable<TSource> Skip<TSource>(IRefObservable<TSource> source, int count);
        IRefObservable<TSource> SkipWhile<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate);
        IRefObservable<TSource> SkipWhile<TSource>(IRefObservable<TSource> source, Func<TSource, int, bool> predicate);
        IRefObservable<TSource> Take<TSource>(IRefObservable<TSource> source, int count);
        IRefObservable<TSource> Take<TSource>(IRefObservable<TSource> source, int count, IScheduler scheduler);
        IRefObservable<TSource> TakeWhile<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate);
        IRefObservable<TSource> TakeWhile<TSource>(IRefObservable<TSource> source, Func<TSource, int, bool> predicate);
        IRefObservable<TSource> Where<TSource>(IRefObservable<TSource> source, Func<TSource, bool> predicate);
        IRefObservable<TSource> Where<TSource>(IRefObservable<TSource> source, Func<TSource, int, bool> predicate);

        IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, Task<TResult>> selector);
        IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, int, Task<TResult>> selector);
        IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, CancellationToken, Task<TResult>> selector);
        IRefObservable<TResult> SelectMany<TSource, TResult>(IRefObservable<TSource> source, Func<TSource, int, CancellationToken, Task<TResult>> selector);
        IRefObservable<TResult> SelectMany<TSource, TTaskResult, TResult>(IRefObservable<TSource> source, Func<TSource, Task<TTaskResult>> taskSelector, Func<TSource, TTaskResult, TResult> resultSelector);
        IRefObservable<TResult> SelectMany<TSource, TTaskResult, TResult>(IRefObservable<TSource> source, Func<TSource, int, Task<TTaskResult>> taskSelector, Func<TSource, int, TTaskResult, TResult> resultSelector);
        IRefObservable<TResult> SelectMany<TSource, TTaskResult, TResult>(IRefObservable<TSource> source, Func<TSource, CancellationToken, Task<TTaskResult>> taskSelector, Func<TSource, TTaskResult, TResult> resultSelector);
        IRefObservable<TResult> SelectMany<TSource, TTaskResult, TResult>(IRefObservable<TSource> source, Func<TSource, int, CancellationToken, Task<TTaskResult>> taskSelector, Func<TSource, int, TTaskResult, TResult> resultSelector);

        #endregion

        #region * Time *

        IRefObservable<IList<TSource>> Buffer<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan);
        IRefObservable<IList<TSource>> Buffer<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, IScheduler scheduler);
        IRefObservable<IList<TSource>> Buffer<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, TimeSpan timeShift);
        IRefObservable<IList<TSource>> Buffer<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, TimeSpan timeShift, IScheduler scheduler);
        IRefObservable<IList<TSource>> Buffer<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, int count);
        IRefObservable<IList<TSource>> Buffer<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, int count, IScheduler scheduler);
        IRefObservable<TSource> Delay<TSource>(IRefObservable<TSource> source, TimeSpan dueTime);
        IRefObservable<TSource> Delay<TSource>(IRefObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler);
        IRefObservable<TSource> Delay<TSource>(IRefObservable<TSource> source, DateTimeOffset dueTime);
        IRefObservable<TSource> Delay<TSource>(IRefObservable<TSource> source, DateTimeOffset dueTime, IScheduler scheduler);
        IRefObservable<TSource> Delay<TSource, TDelay>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TDelay>> delayDurationSelector);
        IRefObservable<TSource> Delay<TSource, TDelay>(IRefObservable<TSource> source, IRefObservable<TDelay> subscriptionDelay, Func<TSource, IRefObservable<TDelay>> delayDurationSelector);
        IRefObservable<TSource> DelaySubscription<TSource>(IRefObservable<TSource> source, TimeSpan dueTime);
        IRefObservable<TSource> DelaySubscription<TSource>(IRefObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler);
        IRefObservable<TSource> DelaySubscription<TSource>(IRefObservable<TSource> source, DateTimeOffset dueTime);
        IRefObservable<TSource> DelaySubscription<TSource>(IRefObservable<TSource> source, DateTimeOffset dueTime, IScheduler scheduler);
        IRefObservable<TResult> Generate<TState, TResult>(TState initialState, Func<TState, bool> condition, Func<TState, TState> iterate, Func<TState, TResult> resultSelector, Func<TState, TimeSpan> timeSelector);
        IRefObservable<TResult> Generate<TState, TResult>(TState initialState, Func<TState, bool> condition, Func<TState, TState> iterate, Func<TState, TResult> resultSelector, Func<TState, TimeSpan> timeSelector, IScheduler scheduler);
        IRefObservable<TResult> Generate<TState, TResult>(TState initialState, Func<TState, bool> condition, Func<TState, TState> iterate, Func<TState, TResult> resultSelector, Func<TState, DateTimeOffset> timeSelector);
        IRefObservable<TResult> Generate<TState, TResult>(TState initialState, Func<TState, bool> condition, Func<TState, TState> iterate, Func<TState, TResult> resultSelector, Func<TState, DateTimeOffset> timeSelector, IScheduler scheduler);
        IRefObservable<long> Interval(TimeSpan period);
        IRefObservable<long> Interval(TimeSpan period, IScheduler scheduler);
        IRefObservable<TSource> Sample<TSource>(IRefObservable<TSource> source, TimeSpan interval);
        IRefObservable<TSource> Sample<TSource>(IRefObservable<TSource> source, TimeSpan interval, IScheduler scheduler);
        IRefObservable<TSource> Sample<TSource, TSample>(IRefObservable<TSource> source, IRefObservable<TSample> sampler);
        IRefObservable<TSource> Skip<TSource>(IRefObservable<TSource> source, TimeSpan duration);
        IRefObservable<TSource> Skip<TSource>(IRefObservable<TSource> source, TimeSpan duration, IScheduler scheduler);
        IRefObservable<TSource> SkipLast<TSource>(IRefObservable<TSource> source, TimeSpan duration);
        IRefObservable<TSource> SkipLast<TSource>(IRefObservable<TSource> source, TimeSpan duration, IScheduler scheduler);
        IRefObservable<TSource> SkipUntil<TSource>(IRefObservable<TSource> source, DateTimeOffset startTime);
        IRefObservable<TSource> SkipUntil<TSource>(IRefObservable<TSource> source, DateTimeOffset startTime, IScheduler scheduler);
        IRefObservable<TSource> Take<TSource>(IRefObservable<TSource> source, TimeSpan duration);
        IRefObservable<TSource> Take<TSource>(IRefObservable<TSource> source, TimeSpan duration, IScheduler scheduler);
        IRefObservable<TSource> TakeLast<TSource>(IRefObservable<TSource> source, TimeSpan duration);
        IRefObservable<TSource> TakeLast<TSource>(IRefObservable<TSource> source, TimeSpan duration, IScheduler scheduler);
        IRefObservable<TSource> TakeLast<TSource>(IRefObservable<TSource> source, TimeSpan duration, IScheduler timerScheduler, IScheduler loopScheduler);
        IRefObservable<IList<TSource>> TakeLastBuffer<TSource>(IRefObservable<TSource> source, TimeSpan duration);
        IRefObservable<IList<TSource>> TakeLastBuffer<TSource>(IRefObservable<TSource> source, TimeSpan duration, IScheduler scheduler);
        IRefObservable<TSource> TakeUntil<TSource>(IRefObservable<TSource> source, DateTimeOffset endTime);
        IRefObservable<TSource> TakeUntil<TSource>(IRefObservable<TSource> source, DateTimeOffset endTime, IScheduler scheduler);
        IRefObservable<TSource> Throttle<TSource>(IRefObservable<TSource> source, TimeSpan dueTime);
        IRefObservable<TSource> Throttle<TSource>(IRefObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler);
        IRefObservable<TSource> Throttle<TSource, TThrottle>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TThrottle>> throttleDurationSelector);
        IRefObservable<Refactive.TimeInterval<TSource>> TimeInterval<TSource>(IRefObservable<TSource> source);
        IRefObservable<Refactive.TimeInterval<TSource>> TimeInterval<TSource>(IRefObservable<TSource> source, IScheduler scheduler);
        IRefObservable<TSource> Timeout<TSource>(IRefObservable<TSource> source, TimeSpan dueTime);
        IRefObservable<TSource> Timeout<TSource>(IRefObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler);
        IRefObservable<TSource> Timeout<TSource>(IRefObservable<TSource> source, TimeSpan dueTime, IRefObservable<TSource> other);
        IRefObservable<TSource> Timeout<TSource>(IRefObservable<TSource> source, TimeSpan dueTime, IRefObservable<TSource> other, IScheduler scheduler);
        IRefObservable<TSource> Timeout<TSource>(IRefObservable<TSource> source, DateTimeOffset dueTime);
        IRefObservable<TSource> Timeout<TSource>(IRefObservable<TSource> source, DateTimeOffset dueTime, IScheduler scheduler);
        IRefObservable<TSource> Timeout<TSource>(IRefObservable<TSource> source, DateTimeOffset dueTime, IRefObservable<TSource> other);
        IRefObservable<TSource> Timeout<TSource>(IRefObservable<TSource> source, DateTimeOffset dueTime, IRefObservable<TSource> other, IScheduler scheduler);
        IRefObservable<TSource> Timeout<TSource, TTimeout>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TTimeout>> timeoutDurationSelector);
        IRefObservable<TSource> Timeout<TSource, TTimeout>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TTimeout>> timeoutDurationSelector, IRefObservable<TSource> other);
        IRefObservable<TSource> Timeout<TSource, TTimeout>(IRefObservable<TSource> source, IRefObservable<TTimeout> firstTimeout, Func<TSource, IRefObservable<TTimeout>> timeoutDurationSelector);
        IRefObservable<TSource> Timeout<TSource, TTimeout>(IRefObservable<TSource> source, IRefObservable<TTimeout> firstTimeout, Func<TSource, IRefObservable<TTimeout>> timeoutDurationSelector, IRefObservable<TSource> other);
        IRefObservable<long> Timer(TimeSpan dueTime);
        IRefObservable<long> Timer(DateTimeOffset dueTime);
        IRefObservable<long> Timer(TimeSpan dueTime, TimeSpan period);
        IRefObservable<long> Timer(DateTimeOffset dueTime, TimeSpan period);
        IRefObservable<long> Timer(TimeSpan dueTime, IScheduler scheduler);
        IRefObservable<long> Timer(DateTimeOffset dueTime, IScheduler scheduler);
        IRefObservable<long> Timer(TimeSpan dueTime, TimeSpan period, IScheduler scheduler);
        IRefObservable<long> Timer(DateTimeOffset dueTime, TimeSpan period, IScheduler scheduler);
        IRefObservable<Timestamped<TSource>> Timestamp<TSource>(IRefObservable<TSource> source);
        IRefObservable<Timestamped<TSource>> Timestamp<TSource>(IRefObservable<TSource> source, IScheduler scheduler);
        IRefObservable<IRefObservable<TSource>> Window<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan);
        IRefObservable<IRefObservable<TSource>> Window<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, IScheduler scheduler);
        IRefObservable<IRefObservable<TSource>> Window<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, TimeSpan timeShift);
        IRefObservable<IRefObservable<TSource>> Window<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, TimeSpan timeShift, IScheduler scheduler);
        IRefObservable<IRefObservable<TSource>> Window<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, int count);
        IRefObservable<IRefObservable<TSource>> Window<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, int count, IScheduler scheduler);

        #endregion
    }
}
