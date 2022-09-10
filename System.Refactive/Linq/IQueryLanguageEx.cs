// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Concurrency;

namespace System.Refactive.Linq
{
    /// <summary>
    /// Internal interface describing the LINQ to Events query language.
    /// </summary>
    internal partial interface IQueryLanguageEx
    {
        IRefObservable<TResult> Create<TResult>(Func<IRefObserver<TResult>, IEnumerable<IRefObservable<object>>> iteratorMethod);
        IRefObservable<Unit> Create(Func<IEnumerable<IRefObservable<object>>> iteratorMethod);

        IRefObservable<TSource> Expand<TSource>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TSource>> selector);
        IRefObservable<TSource> Expand<TSource>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TSource>> selector, IScheduler scheduler);

        IRefObservable<TResult> ForkJoin<TFirst, TSecond, TResult>(IRefObservable<TFirst> first, IRefObservable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector);
        IRefObservable<TSource[]> ForkJoin<TSource>(params IRefObservable<TSource>[] sources);
        IRefObservable<TSource[]> ForkJoin<TSource>(IEnumerable<IRefObservable<TSource>> sources);

        IRefObservable<TResult> Let<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, IRefObservable<TResult>> function);

        IRefObservable<TResult> ManySelect<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, TResult> selector);
        IRefObservable<TResult> ManySelect<TSource, TResult>(IRefObservable<TSource> source, Func<IRefObservable<TSource>, TResult> selector, IScheduler scheduler);

        ListObservable<TSource> ToListObservable<TSource>(IRefObservable<TSource> source);

        IRefObservable<(TFirst First, TSecond Second)> WithLatestFrom<TFirst, TSecond>(IRefObservable<TFirst> first, IRefObservable<TSecond> second);

        IRefObservable<(TFirst First, TSecond Second)> Zip<TFirst, TSecond>(IRefObservable<TFirst> first, IEnumerable<TSecond> second);
    }
}
