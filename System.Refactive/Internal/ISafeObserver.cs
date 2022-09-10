// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

namespace System.Refactive.Internal
{
    /// <summary>
    /// Base interface for observers that can dispose of a resource on a terminal notification
    /// or when disposed itself.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface ISafeObserver<T> : IRefObserver<T>, IDisposable
    {
        void SetResource(IDisposable resource);
    }
}
