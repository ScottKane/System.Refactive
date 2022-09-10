// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Concurrency;

namespace System.Refactive.Linq
{
    internal partial class QueryLanguage
    {
        #region + Buffer +

        #region TimeSpan only

        public virtual IRefObservable<IList<TSource>> Buffer<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan)
        {
            return Buffer_(source, timeSpan, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<IList<TSource>> Buffer<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, IScheduler scheduler)
        {
            return Buffer_(source, timeSpan, scheduler);
        }

        private static IRefObservable<IList<TSource>> Buffer_<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, IScheduler scheduler)
        {
            return new Buffer<TSource>.TimeHopping(source, timeSpan, scheduler);
        }

        public virtual IRefObservable<IList<TSource>> Buffer<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, TimeSpan timeShift)
        {
            return Buffer_(source, timeSpan, timeShift, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<IList<TSource>> Buffer<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, TimeSpan timeShift, IScheduler scheduler)
        {
            return Buffer_(source, timeSpan, timeShift, scheduler);
        }

        private static IRefObservable<IList<TSource>> Buffer_<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, TimeSpan timeShift, IScheduler scheduler)
        {
            return new Buffer<TSource>.TimeSliding(source, timeSpan, timeShift, scheduler);
        }

        #endregion

        #region TimeSpan + int

        public virtual IRefObservable<IList<TSource>> Buffer<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, int count)
        {
            return Buffer_(source, timeSpan, count, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<IList<TSource>> Buffer<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, int count, IScheduler scheduler)
        {
            return Buffer_(source, timeSpan, count, scheduler);
        }

        private static IRefObservable<IList<TSource>> Buffer_<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, int count, IScheduler scheduler)
        {
            return new Buffer<TSource>.Ferry(source, timeSpan, count, scheduler);
        }

        #endregion

        #endregion

        #region + Delay +

        #region TimeSpan

        public virtual IRefObservable<TSource> Delay<TSource>(IRefObservable<TSource> source, TimeSpan dueTime)
        {
            return Delay_(source, dueTime, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<TSource> Delay<TSource>(IRefObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler)
        {
            return Delay_(source, dueTime, scheduler);
        }

        private static IRefObservable<TSource> Delay_<TSource>(IRefObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler)
        {
            return new Delay<TSource>.Relative(source, dueTime, scheduler);
        }

        #endregion

        #region DateTimeOffset

        public virtual IRefObservable<TSource> Delay<TSource>(IRefObservable<TSource> source, DateTimeOffset dueTime)
        {
            return Delay_(source, dueTime, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<TSource> Delay<TSource>(IRefObservable<TSource> source, DateTimeOffset dueTime, IScheduler scheduler)
        {
            return Delay_(source, dueTime, scheduler);
        }

        private static IRefObservable<TSource> Delay_<TSource>(IRefObservable<TSource> source, DateTimeOffset dueTime, IScheduler scheduler)
        {
            return new Delay<TSource>.Absolute(source, dueTime, scheduler);
        }

        #endregion

        #region Duration selector

        public virtual IRefObservable<TSource> Delay<TSource, TDelay>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TDelay>> delayDurationSelector)
        {
            return new Delay<TSource, TDelay>.Selector(source, delayDurationSelector);
        }

        public virtual IRefObservable<TSource> Delay<TSource, TDelay>(IRefObservable<TSource> source, IRefObservable<TDelay> subscriptionDelay, Func<TSource, IRefObservable<TDelay>> delayDurationSelector)
        {
            return new Delay<TSource, TDelay>.SelectorWithSubscriptionDelay(source, subscriptionDelay, delayDurationSelector);
        }

        #endregion

        #endregion

        #region + DelaySubscription +

        public virtual IRefObservable<TSource> DelaySubscription<TSource>(IRefObservable<TSource> source, TimeSpan dueTime)
        {
            return DelaySubscription_(source, dueTime, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<TSource> DelaySubscription<TSource>(IRefObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler)
        {
            return DelaySubscription_(source, dueTime, scheduler);
        }

        private static IRefObservable<TSource> DelaySubscription_<TSource>(IRefObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler)
        {
            return new DelaySubscription<TSource>.Relative(source, dueTime, scheduler);
        }

        public virtual IRefObservable<TSource> DelaySubscription<TSource>(IRefObservable<TSource> source, DateTimeOffset dueTime)
        {
            return DelaySubscription_(source, dueTime, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<TSource> DelaySubscription<TSource>(IRefObservable<TSource> source, DateTimeOffset dueTime, IScheduler scheduler)
        {
            return DelaySubscription_(source, dueTime, scheduler);
        }

        private static IRefObservable<TSource> DelaySubscription_<TSource>(IRefObservable<TSource> source, DateTimeOffset dueTime, IScheduler scheduler)
        {
            return new DelaySubscription<TSource>.Absolute(source, dueTime, scheduler);
        }

        #endregion

        #region + Generate +

        public virtual IRefObservable<TResult> Generate<TState, TResult>(TState initialState, Func<TState, bool> condition, Func<TState, TState> iterate, Func<TState, TResult> resultSelector, Func<TState, TimeSpan> timeSelector)
        {
            return Generate_(initialState, condition, iterate, resultSelector, timeSelector, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<TResult> Generate<TState, TResult>(TState initialState, Func<TState, bool> condition, Func<TState, TState> iterate, Func<TState, TResult> resultSelector, Func<TState, TimeSpan> timeSelector, IScheduler scheduler)
        {
            return Generate_(initialState, condition, iterate, resultSelector, timeSelector, scheduler);
        }

        private static IRefObservable<TResult> Generate_<TState, TResult>(TState initialState, Func<TState, bool> condition, Func<TState, TState> iterate, Func<TState, TResult> resultSelector, Func<TState, TimeSpan> timeSelector, IScheduler scheduler)
        {
            return new Generate<TState, TResult>.Relative(initialState, condition, iterate, resultSelector, timeSelector, scheduler);
        }

        public virtual IRefObservable<TResult> Generate<TState, TResult>(TState initialState, Func<TState, bool> condition, Func<TState, TState> iterate, Func<TState, TResult> resultSelector, Func<TState, DateTimeOffset> timeSelector)
        {
            return Generate_(initialState, condition, iterate, resultSelector, timeSelector, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<TResult> Generate<TState, TResult>(TState initialState, Func<TState, bool> condition, Func<TState, TState> iterate, Func<TState, TResult> resultSelector, Func<TState, DateTimeOffset> timeSelector, IScheduler scheduler)
        {
            return Generate_(initialState, condition, iterate, resultSelector, timeSelector, scheduler);
        }

        private static IRefObservable<TResult> Generate_<TState, TResult>(TState initialState, Func<TState, bool> condition, Func<TState, TState> iterate, Func<TState, TResult> resultSelector, Func<TState, DateTimeOffset> timeSelector, IScheduler scheduler)
        {
            return new Generate<TState, TResult>.Absolute(initialState, condition, iterate, resultSelector, timeSelector, scheduler);
        }

        #endregion

        #region + Interval +

        public virtual IRefObservable<long> Interval(TimeSpan period)
        {
            return Timer_(period, period, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<long> Interval(TimeSpan period, IScheduler scheduler)
        {
            return Timer_(period, period, scheduler);
        }

        #endregion

        #region + Sample +

        public virtual IRefObservable<TSource> Sample<TSource>(IRefObservable<TSource> source, TimeSpan interval)
        {
            return Sample_(source, interval, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<TSource> Sample<TSource>(IRefObservable<TSource> source, TimeSpan interval, IScheduler scheduler)
        {
            return Sample_(source, interval, scheduler);
        }

        private static IRefObservable<TSource> Sample_<TSource>(IRefObservable<TSource> source, TimeSpan interval, IScheduler scheduler)
        {
            return new Sample<TSource>(source, interval, scheduler);
        }

        public virtual IRefObservable<TSource> Sample<TSource, TSample>(IRefObservable<TSource> source, IRefObservable<TSample> sampler)
        {
            return Sample_(source, sampler);
        }

        private static IRefObservable<TSource> Sample_<TSource, TSample>(IRefObservable<TSource> source, IRefObservable<TSample> sampler)
        {
            return new Sample<TSource, TSample>(source, sampler);
        }

        #endregion

        #region + Skip +

        public virtual IRefObservable<TSource> Skip<TSource>(IRefObservable<TSource> source, TimeSpan duration)
        {
            return Skip_(source, duration, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<TSource> Skip<TSource>(IRefObservable<TSource> source, TimeSpan duration, IScheduler scheduler)
        {
            return Skip_(source, duration, scheduler);
        }

        private static IRefObservable<TSource> Skip_<TSource>(IRefObservable<TSource> source, TimeSpan duration, IScheduler scheduler)
        {
            if (source is Skip<TSource>.Time skip && skip._scheduler == scheduler)
            {
                return skip.Combine(duration);
            }

            return new Skip<TSource>.Time(source, duration, scheduler);
        }

        #endregion

        #region + SkipLast +

        public virtual IRefObservable<TSource> SkipLast<TSource>(IRefObservable<TSource> source, TimeSpan duration)
        {
            return SkipLast_(source, duration, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<TSource> SkipLast<TSource>(IRefObservable<TSource> source, TimeSpan duration, IScheduler scheduler)
        {
            return SkipLast_(source, duration, scheduler);
        }

        private static IRefObservable<TSource> SkipLast_<TSource>(IRefObservable<TSource> source, TimeSpan duration, IScheduler scheduler)
        {
            return new SkipLast<TSource>.Time(source, duration, scheduler);
        }

        #endregion

        #region + SkipUntil +

        public virtual IRefObservable<TSource> SkipUntil<TSource>(IRefObservable<TSource> source, DateTimeOffset startTime)
        {
            return SkipUntil_(source, startTime, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<TSource> SkipUntil<TSource>(IRefObservable<TSource> source, DateTimeOffset startTime, IScheduler scheduler)
        {
            return SkipUntil_(source, startTime, scheduler);
        }

        private static IRefObservable<TSource> SkipUntil_<TSource>(IRefObservable<TSource> source, DateTimeOffset startTime, IScheduler scheduler)
        {
            if (source is SkipUntil<TSource> skipUntil && skipUntil._scheduler == scheduler)
            {
                return skipUntil.Combine(startTime);
            }

            return new SkipUntil<TSource>(source, startTime, scheduler);
        }

        #endregion

        #region + Take +

        public virtual IRefObservable<TSource> Take<TSource>(IRefObservable<TSource> source, TimeSpan duration)
        {
            return Take_(source, duration, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<TSource> Take<TSource>(IRefObservable<TSource> source, TimeSpan duration, IScheduler scheduler)
        {
            return Take_(source, duration, scheduler);
        }

        private static IRefObservable<TSource> Take_<TSource>(IRefObservable<TSource> source, TimeSpan duration, IScheduler scheduler)
        {
            if (source is Take<TSource>.Time take && take._scheduler == scheduler)
            {
                return take.Combine(duration);
            }

            return new Take<TSource>.Time(source, duration, scheduler);
        }

        #endregion

        #region + TakeLast +

        public virtual IRefObservable<TSource> TakeLast<TSource>(IRefObservable<TSource> source, TimeSpan duration)
        {
            return TakeLast_(source, duration, SchedulerDefaults.TimeBasedOperations, SchedulerDefaults.Iteration);
        }

        public virtual IRefObservable<TSource> TakeLast<TSource>(IRefObservable<TSource> source, TimeSpan duration, IScheduler scheduler)
        {
            return TakeLast_(source, duration, scheduler, SchedulerDefaults.Iteration);
        }

        public virtual IRefObservable<TSource> TakeLast<TSource>(IRefObservable<TSource> source, TimeSpan duration, IScheduler timerScheduler, IScheduler loopScheduler)
        {
            return TakeLast_(source, duration, timerScheduler, loopScheduler);
        }

        private static IRefObservable<TSource> TakeLast_<TSource>(IRefObservable<TSource> source, TimeSpan duration, IScheduler timerScheduler, IScheduler loopScheduler)
        {
            return new TakeLast<TSource>.Time(source, duration, timerScheduler, loopScheduler);
        }

        public virtual IRefObservable<IList<TSource>> TakeLastBuffer<TSource>(IRefObservable<TSource> source, TimeSpan duration)
        {
            return TakeLastBuffer_(source, duration, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<IList<TSource>> TakeLastBuffer<TSource>(IRefObservable<TSource> source, TimeSpan duration, IScheduler scheduler)
        {
            return TakeLastBuffer_(source, duration, scheduler);
        }

        private static IRefObservable<IList<TSource>> TakeLastBuffer_<TSource>(IRefObservable<TSource> source, TimeSpan duration, IScheduler scheduler)
        {
            return new TakeLastBuffer<TSource>.Time(source, duration, scheduler);
        }

        #endregion

        #region + TakeUntil +

        public virtual IRefObservable<TSource> TakeUntil<TSource>(IRefObservable<TSource> source, DateTimeOffset endTime)
        {
            return TakeUntil_(source, endTime, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<TSource> TakeUntil<TSource>(IRefObservable<TSource> source, DateTimeOffset endTime, IScheduler scheduler)
        {
            return TakeUntil_(source, endTime, scheduler);
        }

        private static IRefObservable<TSource> TakeUntil_<TSource>(IRefObservable<TSource> source, DateTimeOffset endTime, IScheduler scheduler)
        {
            if (source is TakeUntil<TSource> takeUntil && takeUntil._scheduler == scheduler)
            {
                return takeUntil.Combine(endTime);
            }

            return new TakeUntil<TSource>(source, endTime, scheduler);
        }

        #endregion

        #region + Throttle +

        public virtual IRefObservable<TSource> Throttle<TSource>(IRefObservable<TSource> source, TimeSpan dueTime)
        {
            return Throttle_(source, dueTime, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<TSource> Throttle<TSource>(IRefObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler)
        {
            return Throttle_(source, dueTime, scheduler);
        }

        private static IRefObservable<TSource> Throttle_<TSource>(IRefObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler)
        {
            return new Throttle<TSource>(source, dueTime, scheduler);
        }

        public virtual IRefObservable<TSource> Throttle<TSource, TThrottle>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TThrottle>> throttleDurationSelector)
        {
            return new Throttle<TSource, TThrottle>(source, throttleDurationSelector);
        }

        #endregion

        #region + TimeInterval +

        public virtual IRefObservable<Refactive.TimeInterval<TSource>> TimeInterval<TSource>(IRefObservable<TSource> source)
        {
            return TimeInterval_(source, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<Refactive.TimeInterval<TSource>> TimeInterval<TSource>(IRefObservable<TSource> source, IScheduler scheduler)
        {
            return TimeInterval_(source, scheduler);
        }

        private static IRefObservable<Refactive.TimeInterval<TSource>> TimeInterval_<TSource>(IRefObservable<TSource> source, IScheduler scheduler)
        {
            return new TimeInterval<TSource>(source, scheduler);
        }

        #endregion

        #region + Timeout +

        #region TimeSpan

        public virtual IRefObservable<TSource> Timeout<TSource>(IRefObservable<TSource> source, TimeSpan dueTime)
        {
            return Timeout_(source, dueTime, Observable.Throw<TSource>(new TimeoutException()), SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<TSource> Timeout<TSource>(IRefObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler)
        {
            return Timeout_(source, dueTime, Observable.Throw<TSource>(new TimeoutException()), scheduler);
        }

        public virtual IRefObservable<TSource> Timeout<TSource>(IRefObservable<TSource> source, TimeSpan dueTime, IRefObservable<TSource> other)
        {
            return Timeout_(source, dueTime, other, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<TSource> Timeout<TSource>(IRefObservable<TSource> source, TimeSpan dueTime, IRefObservable<TSource> other, IScheduler scheduler)
        {
            return Timeout_(source, dueTime, other, scheduler);
        }

        private static IRefObservable<TSource> Timeout_<TSource>(IRefObservable<TSource> source, TimeSpan dueTime, IRefObservable<TSource> other, IScheduler scheduler)
        {
            return new Timeout<TSource>.Relative(source, dueTime, other, scheduler);
        }

        #endregion

        #region DateTimeOffset

        public virtual IRefObservable<TSource> Timeout<TSource>(IRefObservable<TSource> source, DateTimeOffset dueTime)
        {
            return Timeout_(source, dueTime, Observable.Throw<TSource>(new TimeoutException()), SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<TSource> Timeout<TSource>(IRefObservable<TSource> source, DateTimeOffset dueTime, IScheduler scheduler)
        {
            return Timeout_(source, dueTime, Observable.Throw<TSource>(new TimeoutException()), scheduler);
        }

        public virtual IRefObservable<TSource> Timeout<TSource>(IRefObservable<TSource> source, DateTimeOffset dueTime, IRefObservable<TSource> other)
        {
            return Timeout_(source, dueTime, other, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<TSource> Timeout<TSource>(IRefObservable<TSource> source, DateTimeOffset dueTime, IRefObservable<TSource> other, IScheduler scheduler)
        {
            return Timeout_(source, dueTime, other, scheduler);
        }

        private static IRefObservable<TSource> Timeout_<TSource>(IRefObservable<TSource> source, DateTimeOffset dueTime, IRefObservable<TSource> other, IScheduler scheduler)
        {
            return new Timeout<TSource>.Absolute(source, dueTime, other, scheduler);
        }

        #endregion

        #region Duration selector

        public virtual IRefObservable<TSource> Timeout<TSource, TTimeout>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TTimeout>> timeoutDurationSelector)
        {
            return Timeout_(source, Observable.Never<TTimeout>(), timeoutDurationSelector, Observable.Throw<TSource>(new TimeoutException()));
        }

        public virtual IRefObservable<TSource> Timeout<TSource, TTimeout>(IRefObservable<TSource> source, Func<TSource, IRefObservable<TTimeout>> timeoutDurationSelector, IRefObservable<TSource> other)
        {
            return Timeout_(source, Observable.Never<TTimeout>(), timeoutDurationSelector, other);
        }

        public virtual IRefObservable<TSource> Timeout<TSource, TTimeout>(IRefObservable<TSource> source, IRefObservable<TTimeout> firstTimeout, Func<TSource, IRefObservable<TTimeout>> timeoutDurationSelector)
        {
            return Timeout_(source, firstTimeout, timeoutDurationSelector, Observable.Throw<TSource>(new TimeoutException()));
        }

        public virtual IRefObservable<TSource> Timeout<TSource, TTimeout>(IRefObservable<TSource> source, IRefObservable<TTimeout> firstTimeout, Func<TSource, IRefObservable<TTimeout>> timeoutDurationSelector, IRefObservable<TSource> other)
        {
            return Timeout_(source, firstTimeout, timeoutDurationSelector, other);
        }

        private static IRefObservable<TSource> Timeout_<TSource, TTimeout>(IRefObservable<TSource> source, IRefObservable<TTimeout> firstTimeout, Func<TSource, IRefObservable<TTimeout>> timeoutDurationSelector, IRefObservable<TSource> other)
        {
            return new Timeout<TSource, TTimeout>(source, firstTimeout, timeoutDurationSelector, other);
        }

        #endregion

        #endregion

        #region + Timer +

        public virtual IRefObservable<long> Timer(TimeSpan dueTime)
        {
            return Timer_(dueTime, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<long> Timer(DateTimeOffset dueTime)
        {
            return Timer_(dueTime, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<long> Timer(TimeSpan dueTime, TimeSpan period)
        {
            return Timer_(dueTime, period, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<long> Timer(DateTimeOffset dueTime, TimeSpan period)
        {
            return Timer_(dueTime, period, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<long> Timer(TimeSpan dueTime, IScheduler scheduler)
        {
            return Timer_(dueTime, scheduler);
        }

        public virtual IRefObservable<long> Timer(DateTimeOffset dueTime, IScheduler scheduler)
        {
            return Timer_(dueTime, scheduler);
        }

        public virtual IRefObservable<long> Timer(TimeSpan dueTime, TimeSpan period, IScheduler scheduler)
        {
            return Timer_(dueTime, period, scheduler);
        }

        public virtual IRefObservable<long> Timer(DateTimeOffset dueTime, TimeSpan period, IScheduler scheduler)
        {
            return Timer_(dueTime, period, scheduler);
        }

        private static IRefObservable<long> Timer_(TimeSpan dueTime, IScheduler scheduler)
        {
            return new Timer.Single.Relative(dueTime, scheduler);
        }

        private static IRefObservable<long> Timer_(TimeSpan dueTime, TimeSpan period, IScheduler scheduler)
        {
            return new Timer.Periodic.Relative(dueTime, period, scheduler);
        }

        private static IRefObservable<long> Timer_(DateTimeOffset dueTime, IScheduler scheduler)
        {
            return new Timer.Single.Absolute(dueTime, scheduler);
        }

        private static IRefObservable<long> Timer_(DateTimeOffset dueTime, TimeSpan period, IScheduler scheduler)
        {
            return new Timer.Periodic.Absolute(dueTime, period, scheduler);
        }

        #endregion

        #region + Timestamp +

        public virtual IRefObservable<Timestamped<TSource>> Timestamp<TSource>(IRefObservable<TSource> source)
        {
            return Timestamp_(source, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<Timestamped<TSource>> Timestamp<TSource>(IRefObservable<TSource> source, IScheduler scheduler)
        {
            return Timestamp_(source, scheduler);
        }

        private static IRefObservable<Timestamped<TSource>> Timestamp_<TSource>(IRefObservable<TSource> source, IScheduler scheduler)
        {
            return new Timestamp<TSource>(source, scheduler);
        }

        #endregion

        #region + Window +

        #region TimeSpan only

        public virtual IRefObservable<IRefObservable<TSource>> Window<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan)
        {
            return Window_(source, timeSpan, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<IRefObservable<TSource>> Window<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, IScheduler scheduler)
        {
            return Window_(source, timeSpan, scheduler);
        }

        private static IRefObservable<IRefObservable<TSource>> Window_<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, IScheduler scheduler)
        {
            return new Window<TSource>.TimeHopping(source, timeSpan, scheduler);
        }

        public virtual IRefObservable<IRefObservable<TSource>> Window<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, TimeSpan timeShift)
        {
            return Window_(source, timeSpan, timeShift, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<IRefObservable<TSource>> Window<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, TimeSpan timeShift, IScheduler scheduler)
        {
            return Window_(source, timeSpan, timeShift, scheduler);
        }

        private static IRefObservable<IRefObservable<TSource>> Window_<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, TimeSpan timeShift, IScheduler scheduler)
        {
            return new Window<TSource>.TimeSliding(source, timeSpan, timeShift, scheduler);
        }

        #endregion

        #region TimeSpan + int

        public virtual IRefObservable<IRefObservable<TSource>> Window<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, int count)
        {
            return Window_(source, timeSpan, count, SchedulerDefaults.TimeBasedOperations);
        }

        public virtual IRefObservable<IRefObservable<TSource>> Window<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, int count, IScheduler scheduler)
        {
            return Window_(source, timeSpan, count, scheduler);
        }

        private static IRefObservable<IRefObservable<TSource>> Window_<TSource>(IRefObservable<TSource> source, TimeSpan timeSpan, int count, IScheduler scheduler)
        {
            return new Window<TSource>.Ferry(source, timeSpan, count, scheduler);
        }

        #endregion

        #endregion
    }
}
