// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

namespace System.Refactive
{
    internal sealed class EventPatternSource<TEventArgs> : EventPatternSourceBase<object, TEventArgs>, IEventPatternSource<TEventArgs>
    {
        public EventPatternSource(IRefObservable<EventPattern<object, TEventArgs>> source, Action<RefAction<object?, TEventArgs>, /*object,*/ EventPattern<object, TEventArgs>> invokeHandler)
            : base(source, invokeHandler)
        {
        }

        event EventHandler<TEventArgs> IEventPatternSource<TEventArgs>.OnNext
        {
            add
            {
                Add(value, (ref object? o, ref TEventArgs e) => value(o, e));
            }

            remove
            {
                Remove(value);
            }
        }
    }
}
