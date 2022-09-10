// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Internal;
using System.Runtime.CompilerServices;

namespace System.Refactive.Linq
{
    internal sealed class Dematerialize<TSource> : Producer<TSource, Dematerialize<TSource>._>
    {
        private readonly IRefObservable<Notification<TSource>> _source;

        public Dematerialize(IRefObservable<Notification<TSource>> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<TSource> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : Sink<Notification<TSource>, TSource>
        {
            public _(IRefObserver<TSource> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref Notification<TSource> value)
            {
                switch (value.Kind)
                {
                    case NotificationKind.OnNext:
                        ForwardOnNext(ref Unsafe.AsRef(value.Value));
                        break;
                    case NotificationKind.OnError:
                        ForwardOnError(value.Exception!);
                        break;
                    case NotificationKind.OnCompleted:
                        ForwardOnCompleted();
                        break;
                }
            }
        }
    }
}
