using System;

namespace Trakkr.Core.Events
{
    public interface IBaseEvent<out TPayload>
    {
        DateTime Timestamp { get; }

        TPayload Payload { get; }
    }
}
