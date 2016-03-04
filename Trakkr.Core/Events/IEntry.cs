using System;

namespace Trakkr.Core.Events
{
    public interface IEntry<out TPayload> : IBaseEvent<TPayload>
    {
        TimeSpan Duration { get; }
    }
}
