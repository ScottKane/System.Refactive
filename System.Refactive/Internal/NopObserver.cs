﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

namespace System.Refactive.Internal
{
    internal sealed class NopObserver<T> : IRefObserver<T>
    {
        public static readonly IRefObserver<T> Instance = new NopObserver<T>();

        public void OnCompleted() { }

        public void OnError(Exception error) { }

        public void OnNext(ref T value) { }
    }
}
