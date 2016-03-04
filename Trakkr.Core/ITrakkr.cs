using System;
using Trakkr.Core.Events;

namespace Trakkr.Core
{
    public interface ITrakkr<TPayload>
    {
        IEntry<TPayload> HandleEvent(IEvent<TPayload> @event);
    }
}