// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

namespace System.Refactive.Internal
{
    internal abstract class IdentitySink<T> : Sink<T, T>
    {
        protected IdentitySink(IRefObserver<T> observer) : base(observer)
        {
        }

        public override void OnNext(ref T value)
        {
            ForwardOnNext(ref value);
        }
    }
}
