// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Internal;

namespace System.Refactive.Concurrency
{
    internal sealed class Synchronize<TSource> : Producer<TSource, Synchronize<TSource>._>
    {
        private readonly IRefObservable<TSource> _source;
        private readonly object? _gate;

        public Synchronize(IRefObservable<TSource> source, object gate)
        {
            _source = source;
            _gate = gate;
        }

        public Synchronize(IRefObservable<TSource> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IRefObserver<TSource> observer) => new _(this, observer);

        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Visibility restricted to friend assemblies. Those should be correct by inspection.")]
        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<TSource>
        {
            private readonly object _gate;

            public _(Synchronize<TSource> parent, IRefObserver<TSource> observer)
                : base(observer)
            {
                _gate = parent._gate ?? new object();
            }

            public override void OnNext(ref TSource value)
            {
                lock (_gate)
                {
                    ForwardOnNext(ref value);
                }
            }

            public override void OnError(Exception error)
            {
                lock (_gate)
                {
                    ForwardOnError(error);
                }
            }

            public override void OnCompleted()
            {
                lock (_gate)
                {
                    ForwardOnCompleted();
                }
            }
        }
    }
}
