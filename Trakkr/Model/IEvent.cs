using System;

namespace Trakkr.Model
{
    public interface IEvent
    {
        EventType Type { get; set; }
        DateTime UtcTimestamp { get; set; }
        string Description { get; set; }
    }
}