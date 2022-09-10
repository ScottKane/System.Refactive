// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Internal;
using System.Runtime.CompilerServices;

namespace System.Refactive.Linq
{
    internal sealed class Materialize<TSource> : Producer<Notification<TSource>, Materialize<TSource>._>
    {
        private readonly IRefObservable<TSource> _source;

        public Materialize(IRefObservable<TSource> source)
        {
            _source = source;
        }

        public IRefObservable<TSource> Dematerialize() => _source.AsObservable();

        protected override _ CreateSink(IRefObserver<Notification<TSource>> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : Sink<TSource, Notification<TSource>>
        {
            public _(IRefObserver<Notification<TSource>> observer)
                : base(observer)
            {
            }

            public override void OnNext(ref TSource value)
            {
                ForwardOnNext(ref Unsafe.AsRef(Notification.CreateOnNext(value)));
            }

            public override void OnError(Exception error)
            {
                ForwardOnNext(ref Unsafe.AsRef(Notification.CreateOnError<TSource>(error)));
                ForwardOnCompleted();
            }

            public override void OnCompleted()
            {
                ForwardOnNext(ref Unsafe.AsRef(Notification.CreateOnCompleted<TSource>()));
                ForwardOnCompleted();
            }
        }
    }
}
