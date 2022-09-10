// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Refactive.Internal;
using System.Runtime.CompilerServices;

namespace System.Refactive.Linq
{
    internal sealed class ToList<TSource> : Producer<IList<TSource>, ToList<TSource>._>
    {
        private readonly IRefObservable<TSource> _source;

        public ToList(IRefObservable<TSource> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<IList<TSource>> observer) => new _(observer);

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : Sink<TSource, IList<TSource>>
        {
            private List<TSource> _list;

            public _(IRefObserver<IList<TSource>> observer)
                : base(observer)
            {
                _list = new List<TSource>();
            }

            public override void OnNext(ref TSource value)
            {
                _list.Add(value);
            }

            public override void OnError(Exception error)
            {
                Cleanup();
                ForwardOnError(error);
            }

            public override void OnCompleted()
            {
                var list = _list;
                Cleanup();
                ForwardOnNext(ref Unsafe.AsRef<IList<TSource>>(list));
                ForwardOnCompleted();
            }

            private void Cleanup()
            {
                _list = null!;
            }
        }
    }
}
