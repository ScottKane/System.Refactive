// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Refactive.Disposables;
using System.Refactive.Internal;

namespace System.Refactive.Linq
{
    internal class AddRef<TSource> : Producer<TSource, AddRef<TSource>._>
    {
        private readonly IRefObservable<TSource> _source;
        private readonly RefCountDisposable _refCount;

        public AddRef(IRefObservable<TSource> source, RefCountDisposable refCount)
        {
            _source = source;
            _refCount = refCount;
        }

        protected override _ CreateSink(IRefObserver<TSource> observer) => new _(observer, _refCount.GetDisposable());

        protected override void Run(_ sink) => sink.Run(_source);

        internal sealed class _ : IdentitySink<TSource>
        {
            private readonly IDisposable _refCountDisposable;

            public _(IRefObserver<TSource> observer, IDisposable refCountDisposable)
                : base(observer)
            {
                _refCountDisposable = refCountDisposable;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _refCountDisposable.Dispose();
                }

                base.Dispose(disposing);
            }
        }
    }
}
