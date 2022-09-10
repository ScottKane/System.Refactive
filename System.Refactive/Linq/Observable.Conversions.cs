// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Concurrency;

namespace System.Refactive.Linq
{
    public static partial class Observable
    {
        #region + Subscribe +

        /// <summary>
        /// Subscribes an observer to an enumerable sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Enumerable sequence to subscribe to.</param>
        /// <param name="observer">Observer that will receive notifications from the enumerable sequence.</param>
        /// <returns>Disposable object that can be used to unsubscribe the observer from the enumerable</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="observer"/> is null.</exception>
        public static IDisposable Subscribe<TSource>(this IEnumerable<TSource> source, IRefObserver<TSource> observer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            return s_impl.Subscribe(source, observer);
        }

        /// <summary>
        /// Subscribes an observer to an enumerable sequence, using the specified scheduler to run the enumeration loop.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Enumerable sequence to subscribe to.</param>
        /// <param name="observer">Observer that will receive notifications from the enumerable sequence.</param>
        /// <param name="scheduler">Scheduler to perform the enumeration on.</param>
        /// <returns>Disposable object that can be used to unsubscribe the observer from the enumerable</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="observer"/> or <paramref name="scheduler"/> is null.</exception>
        public static IDisposable Subscribe<TSource>(this IEnumerable<TSource> source, IRefObserver<TSource> observer, IScheduler scheduler)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            return s_impl.Subscribe(source, observer, scheduler);
        }

        #endregion

        #region + ToEnumerable +

        /// <summary>
        /// Converts an observable sequence to an enumerable sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">An observable sequence to convert to an enumerable sequence.</param>
        /// <returns>The enumerable sequence containing the elements in the observable sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IEnumerable<TSource> ToEnumerable<TSource>(this IRefObservable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return s_impl.ToEnumerable(source);
        }

        #endregion

        #region ToEvent

        /// <summary>
        /// Exposes an observable sequence as an object with an <see cref="Action"/>-based .NET event.
        /// </summary>
        /// <param name="source">Observable source sequence.</param>
        /// <returns>The event source object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IEventSource<Unit> ToEvent(this IRefObservable<Unit> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return s_impl.ToEvent(source);
        }

        /// <summary>
        /// Exposes an observable sequence as an object with an <see cref="Action{TSource}"/>-based .NET event.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Observable source sequence.</param>
        /// <returns>The event source object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IEventSource<TSource> ToEvent<TSource>(this IRefObservable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return s_impl.ToEvent(source);
        }

        #endregion

        #region ToEventPattern

        #endregion

        #region + ToObservable +

        /// <summary>
        /// Converts an enumerable sequence to an observable sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Enumerable sequence to convert to an observable sequence.</param>
        /// <returns>The observable sequence whose elements are pulled from the given enumerable sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IRefObservable<TSource> ToObservable<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return s_impl.ToObservable(source);
        }

        /// <summary>
        /// Converts an enumerable sequence to an observable sequence, using the specified scheduler to run the enumeration loop.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Enumerable sequence to convert to an observable sequence.</param>
        /// <param name="scheduler">Scheduler to run the enumeration of the input sequence on.</param>
        /// <returns>The observable sequence whose elements are pulled from the given enumerable sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="scheduler"/> is null.</exception>
        public static IRefObservable<TSource> ToObservable<TSource>(this IEnumerable<TSource> source, IScheduler scheduler)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            return s_impl.ToObservable(source, scheduler);
        }

        #endregion
    }
}
