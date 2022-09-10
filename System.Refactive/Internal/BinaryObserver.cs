// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

namespace System.Refactive.Internal
{
    internal sealed class BinaryObserver<TLeft, TRight> : IRefObserver<Either<Notification<TLeft>, Notification<TRight>>>
    {
        public BinaryObserver(IRefObserver<TLeft> leftObserver, IRefObserver<TRight> rightObserver)
        {
            LeftObserver = leftObserver;
            RightObserver = rightObserver;
        }

        public BinaryObserver(Action<Notification<TLeft>> left, Action<Notification<TRight>> right)
            : this(left.ToObserver(), right.ToObserver())
        {
        }

        public IRefObserver<TLeft> LeftObserver { get; }
        public IRefObserver<TRight> RightObserver { get; }

        void IRefObserver<Either<Notification<TLeft>, Notification<TRight>>>.OnNext(ref Either<Notification<TLeft>, Notification<TRight>> value)
        {
            value.Switch(left => left.Accept(LeftObserver), right => right.Accept(RightObserver));
        }

        void IRefObserver<Either<Notification<TLeft>, Notification<TRight>>>.OnError(Exception exception)
        {
        }

        void IRefObserver<Either<Notification<TLeft>, Notification<TRight>>>.OnCompleted()
        {
        }
    }
}
