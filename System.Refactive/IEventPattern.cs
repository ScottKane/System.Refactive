﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

namespace System.Refactive
{
    /// <summary>
    /// Represents a .NET event invocation consisting of the strongly typed object that raised the event and the data that was generated by the event.
    /// </summary>
    /// <typeparam name="TSender">
    /// The type of the sender that raised the event.
    /// This type parameter is covariant. That is, you can use either the type you specified or any type that is more derived. For more information about covariance and contravariance, see Covariance and Contravariance in Generics.
    /// </typeparam>
    /// <typeparam name="TEventArgs">
    /// The type of the event data generated by the event.
    /// This type parameter is covariant. That is, you can use either the type you specified or any type that is more derived. For more information about covariance and contravariance, see Covariance and Contravariance in Generics.
    /// </typeparam>
    public interface IEventPattern<out TSender, out TEventArgs>
    {
        /// <summary>
        /// Gets the sender object that raised the event.
        /// </summary>
        TSender? Sender { get; }

        /// <summary>
        /// Gets the event data that was generated by the event.
        /// </summary>
        TEventArgs EventArgs { get; }
    }
}
