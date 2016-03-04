using System;

namespace Trakkr.Core.Events
{
    public class Entry<TPayload> : BaseEvent<TPayload>, IEntry<TPayload>
    {
        public TimeSpan Duration { get; set; }
    }
}
