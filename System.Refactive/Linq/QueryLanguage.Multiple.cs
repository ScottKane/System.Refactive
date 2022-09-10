// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Linq;
using System.Refactive.Concurrency;
using System.Refactive.Threading.Tasks;
using System.Threading.Tasks;

namespace System.Refactive.Linq
{
    internal partial class QueryLanguage
    {
        #region + Amb +

        public virtual IRefObservable<TSource> Amb<TSource>(IRefObservable<TSource> first, IRefObservable<TSource> second)
        {
            return new Amb<TSource>(first, second);
        }

        public virtual IRefObservable<TSource> Amb<TSource>(params IRefObservable<TSource>[] sources)
        {
            return new AmbManyArray<TSource>(sources);
        }

        public virtual IRefObservable<TSource> Amb<TSource>(IEnumerable<IRefObservable<TSource>> sources)
        {
            return new AmbManyEnumerable<TSource>(sources);
        }

        #endregion

        #region + Buffer +

        public virtual IRefObservable<IList<TSource>> Buffer<TSource, TBufferClosing>(IRefObservable<TSource> source, Func<IRefObservable<TBufferClosing>> bufferClosingSelector)
        {
            return new Buffer<TSource, TBufferClosing>.Selector(source, bufferClosingSelector);
        }

        public virtual IRefObservable<IList<TSource>> Buffer<TSource, TBufferOpening, TBufferClosing>(IRefObservable<TSource> source, IRefObservable<TBufferOpening> bufferOpenings, Func<TBufferOpening, IRefObservable<TBufferClosing>> bufferClosingSelector)
        {
            return source.Window(bufferOpenings, bufferClosingSelector).SelectMany(ToList);
        }

        public virtual IRefObservable<IList<TSource>> Buffer<TSource, TBufferBoundary>(IRefObservable<TSource> source, IRefObservable<TBufferBoundary> bufferBoundaries)
        {
            return new Buffer<TSource, TBufferBoundary>.Boundaries(source, bufferBoundaries);
        }

        #endregion

        #region + Catch +

        public virtual IRefObservable<TSource> Catch<TSource, TException>(IRefObservable<TSource> source, Func<TException, IRefObservable<TSource>> handler) where TException : Exception
        {
            return new Catch<TSource, TException>(source, handler);
        }

        public virtual IRefObservable<TSource> Catch<TSource>(IRefObservable<TSource> first, IRefObservable<TSource> second)
        {
            return Catch_(new[] { first, second });
        }

        public virtual IRefObservable<TSource> Catch<TSource>(params IRefObservable<TSource>[] sources)
        {
            return Catch_(sources);
        }

        public virtual IRefObservable<TSource> Catch<TSource>(IEnumerable<IRefObservable<TSource>> sources)
        {
            return Catch_(sources);
        }

        private static IRefObservable<TSource> Catch_<TSource>(IEnumerable<IRefObservable<TSource>> sources)
        {
            return new Catch<TSource>(sources);
        }

        #endregion

        #region + CombineLatest +

        public virtual IRefObservable<TResult> CombineLatest<TFirst, TSecond, TResult>(IRefObservable<TFirst> first, IRefObservable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            return new CombineLatest<TFirst, TSecond, TResult>(first, second, resultSelector);
        }

        public virtual IRefObservable<TResult> CombineLatest<TSource, TResult>(IEnumerable<IRefObservable<TSource>> sources, Func<IList<TSource>, TResult> resultSelector)
        {
            return CombineLatest_(sources, resultSelector);
        }

        // public virtual IRefObservable<IList<TSource>> CombineLatest<TSource>(IEnumerable<IRefObservable<TSource>> sources)
        // {
        //     return CombineLatest_(sources, res => res.ToList());
        // }
        public virtual IRefObservable<List<TSource>> CombineLatest<TSource>(IEnumerable<IRefObservable<TSource>> sources)
        {
            return CombineLatest_(sources, res => res.ToList());
        }

        // public virtual IRefObservable<IList<TSource>> CombineLatest<TSource>(params IRefObservable<TSource>[] sources)
        // {
        //     return CombineLatest_(sources, res => res.ToList());
        // }
        public virtual IRefObservable<List<TSource>> CombineLatest<TSource>(params IRefObservable<TSource>[] sources)
        {
            return CombineLatest_(sources, res => res.ToList());
        }

        private static IRefObservable<TResult> CombineLatest_<TSource, TResult>(IEnumerable<IRefObservable<TSource>> sources, Func<IList<TSource>, TResult> resultSelector)
        {
            return new CombineLatest<TSource, TResult>(sources, resultSelector);
        }

        #endregion

        #region + Concat +

        public virtual IRefObservable<TSource> Concat<TSource>(IRefObservable<TSource> first, IRefObservable<TSource> second)
        {
            return Concat_(new[] { first, second });
        }

        public virtual IRefObservable<TSource> Concat<TSource>(params IRefObservable<TSource>[] sources)
        {
            return Concat_(sources);
        }

        public virtual IRefObservable<TSource> Concat<TSource>(IEnumerable<IRefObservable<TSource>> sources)
        {
            return Concat_(sources);
        }

        private static IRefObservable<TSource> Concat_<TSource>(IEnumerable<IRefObservable<TSource>> sources)
        {
            return new Concat<TSource>(sources);
        }

        public virtual IRefObservable<TSource> Concat<TSource>(IRefObservable<IRefObservable<TSource>> sources)
        {
            return Concat_(sources);
        }

        public virtual IRefObservable<TSource> Concat<TSource>(IRefObservable<Task<TSource>> sources)
        {
            return Concat_(Select(sources, TaskObservableExtensions.ToObservable));
        }

        private static IRefObservable<TSource> Concat_<TSource>(IRefObservable<IRefObservable<TSource>> sources)
        {
            return new ConcatMany<TSource>(sources);
        }

        #endregion

        #region + Merge +

        public virtual IRefObservable<TSource> Merge<TSource>(IRefObservable<IRefObservable<TSource>> sources)
        {
            return Merge_(sources);
        }

        public virtual IRefObservable<TSource> Merge<TSource>(IRefObservable<Task<TSource>> sources)
        {
            return new Merge<TSource>.Tasks(sources);
        }

        public virtual IRefObservable<TSource> Merge<TSource>(IRefObservable<IRefObservable<TSource>> sources, int maxConcurrent)
        {
            return Merge_(sources, maxConcurrent);
        }

        public virtual IRefObservable<TSource> Merge<TSource>(IEnumerable<IRefObservable<TSource>> sources, int maxConcurrent)
        {
            return Merge_(sources.ToObservable(SchedulerDefaults.ConstantTimeOperations), maxConcurrent);
        }

        public virtual IRefObservable<TSource> Merge<TSource>(IEnumerable<IRefObservable<TSource>> sources, int maxConcurrent, IScheduler scheduler)
        {
            return Merge_(sources.ToObservable(scheduler), maxConcurrent);
        }

        public virtual IRefObservable<TSource> Merge<TSource>(IRefObservable<TSource> first, IRefObservable<TSource> second)
        {
            return Merge_(new[] { first, second }.ToObservable(SchedulerDefaults.ConstantTimeOperations));
        }

        public virtual IRefObservable<TSource> Merge<TSource>(IRefObservable<TSource> first, IRefObservable<TSource> second, IScheduler scheduler)
        {
            return Merge_(new[] { first, second }.ToObservable(scheduler));
        }

        public virtual IRefObservable<TSource> Merge<TSource>(params IRefObservable<TSource>[] sources)
        {
            return Merge_(sources.ToObservable(SchedulerDefaults.ConstantTimeOperations));
        }

        public virtual IRefObservable<TSource> Merge<TSource>(IScheduler scheduler, params IRefObservable<TSource>[] sources)
        {
            return Merge_(sources.ToObservable(scheduler));
        }

        public virtual IRefObservable<TSource> Merge<TSource>(IEnumerable<IRefObservable<TSource>> sources)
        {
            return Merge_(sources.ToObservable(SchedulerDefaults.ConstantTimeOperations));
        }

        public virtual IRefObservable<TSource> Merge<TSource>(IEnumerable<IRefObservable<TSource>> sources, IScheduler scheduler)
        {
            return Merge_(sources.ToObservable(scheduler));
        }

        private static IRefObservable<TSource> Merge_<TSource>(IRefObservable<IRefObservable<TSource>> sources)
        {
            return new Merge<TSource>.Observables(sources);
        }

        private static IRefObservable<TSource> Merge_<TSource>(IRefObservable<IRefObservable<TSource>> sources, int maxConcurrent)
        {
            return new Merge<TSource>.ObservablesMaxConcurrency(sources, maxConcurrent);
        }

        #endregion

        #region + OnErrorResumeNext +

        public virtual IRefObservable<TSource> OnErrorResumeNext<TSource>(IRefObservable<TSource> first, IRefObservable<TSource> second)
        {
            return OnErrorResumeNext_(new[] { first, second });
        }

        public virtual IRefObservable<TSource> OnErrorResumeNext<TSource>(params IRefObservable<TSource>[] sources)
        {
            return OnErrorResumeNext_(sources);
        }

        public virtual IRefObservable<TSource> OnErrorResumeNext<TSource>(IEnumerable<IRefObservable<TSource>> sources)
        {
            return OnErrorResumeNext_(sources);
        }

        private static IRefObservable<TSource> OnErrorResumeNext_<TSource>(IEnumerable<IRefObservable<TSource>> sources)
        {
            return new OnErrorResumeNext<TSource>(sources);
        }

        #endregion

        #region + SkipUntil +

        public virtual IRefObservable<TSource> SkipUntil<TSource, TOther>(IRefObservable<TSource> source, IRefObservable<TOther> other)
        {
            return new SkipUntil<TSource, TOther>(source, other);
        }

        #endregion

        #region + Switch +

        public virtual IRefObservable<TSource> Switch<TSource>(IRefObservable<IRefObservable<TSource>> sources)
        {
            return Switch_(sources);
        }

        public virtual IRefObservable<TSource> Switch<TSource>(IRefObservable<Task<TSource>> sources)
        {
            return Switch_(Select(sources, TaskObservableExtensions.ToObservable));
        }

        private static IRefObservable<TSource> Switch_<TSource>(IRefObservable<IRefObservable<TSource>> sources)
        {
            return new Switch<TSource>(sources);
        }

        #endregion

        #region + TakeUntil +

        public virtual IRefObservable<TSource> TakeUntil<TSource, TOther>(IRefObservable<TSource> source, IRefObservable<TOther> other)
        {
            return new TakeUntil<TSource, TOther>(source, other);
        }

        public virtual IRefObservable<TSource> TakeUntil<TSource>(IRefObservable<TSource> source, Func<TSource, bool> stopPredicate)
        {
            return new TakeUntilPredicate<TSource>(source, stopPredicate);
        }

        #endregion

        #region + Window +

        public virtual IRefObservable<IRefObservable<TSource>> Window<TSource, TWindowClosing>(IRefObservable<TSource> source, Func<IRefObservable<TWindowClosing>> windowClosingSelector)
        {
            return new Window<TSource, TWindowClosing>.Selector(source, windowClosingSelector);
        }

        public virtual IRefObservable<IRefObservable<TSource>> Window<TSource, TWindowOpening, TWindowClosing>(IRefObservable<TSource> source, IRefObservable<TWindowOpening> windowOpenings, Func<TWindowOpening, IRefObservable<TWindowClosing>> windowClosingSelector)
        {
            return windowOpenings.GroupJoin<TWindowOpening, TSource, TWindowClosing, Unit, IRefObservable<TSource>>(source, windowClosingSelector, _ => Observable.Empty<Unit>(), (_, window) => window);
        }

        public virtual IRefObservable<IRefObservable<TSource>> Window<TSource, TWindowBoundary>(IRefObservable<TSource> source, IRefObservable<TWindowBoundary> windowBoundaries)
        {
            return new Window<TSource, TWindowBoundary>.Boundaries(source, windowBoundaries);
        }

        #endregion

        #region + WithLatestFrom +

        public virtual IRefObservable<TResult> WithLatestFrom<TFirst, TSecond, TResult>(IRefObservable<TFirst> first, IRefObservable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            return new WithLatestFrom<TFirst, TSecond, TResult>(first, second, resultSelector);
        }

        #endregion

        #region + Zip +

        public virtual IRefObservable<TResult> Zip<TFirst, TSecond, TResult>(IRefObservable<TFirst> first, IRefObservable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            return new Zip<TFirst, TSecond, TResult>.Observable(first, second, resultSelector);
        }

        public virtual IRefObservable<TResult> Zip<TSource, TResult>(IEnumerable<IRefObservable<TSource>> sources, Func<IList<TSource>, TResult> resultSelector)
        {
            return Zip_(sources).Select(resultSelector);
        }

        public virtual IRefObservable<IList<TSource>> Zip<TSource>(IEnumerable<IRefObservable<TSource>> sources)
        {
            return Zip_(sources);
        }

        public virtual IRefObservable<IList<TSource>> Zip<TSource>(params IRefObservable<TSource>[] sources)
        {
            return Zip_(sources);
        }

        private static IRefObservable<IList<TSource>> Zip_<TSource>(IEnumerable<IRefObservable<TSource>> sources)
        {
            return new Zip<TSource>(sources);
        }

        public virtual IRefObservable<TResult> Zip<TFirst, TSecond, TResult>(IRefObservable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            return new Zip<TFirst, TSecond, TResult>.Enumerable(first, second, resultSelector);
        }

        #endregion
    }
}
