// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Disposables;

namespace System.Refactive
{
    /// <summary>
    /// Class to create an <see cref="IRefObservable{T}"/> instance from a delegate-based implementation of the <see cref="IRefObservable{T}.Subscribe(IRefObserver{T})"/> method.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    public sealed class AnonymousObservable<T> : ObservableBase<T>
    {
        private readonly Func<IRefObserver<T>, IDisposable> _subscribe;

        /// <summary>
        /// Creates an observable sequence object from the specified subscription function.
        /// </summary>
        /// <param name="subscribe"><see cref="IRefObservable{T}.Subscribe(IRefObserver{T})"/> method implementation.</param>
        /// <exception cref="ArgumentNullException"><paramref name="subscribe"/> is <c>null</c>.</exception>
        public AnonymousObservable(Func<IRefObserver<T>, IDisposable> subscribe)
        {
            _subscribe = subscribe ?? throw new ArgumentNullException(nameof(subscribe));
        }

        /// <summary>
        /// Calls the subscription function that was supplied to the constructor.
        /// </summary>
        /// <param name="observer">Observer to send notifications to.</param>
        /// <returns>Disposable object representing an observer's subscription to the observable sequence.</returns>
        protected override IDisposable SubscribeCore(IRefObserver<T> observer)
        {
            return _subscribe(observer) ?? Disposable.Empty;
        }
    }
}
