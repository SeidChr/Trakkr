using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trakkr.Model
{
    public class Event : IEvent
    {
        public EventType Type { get; set; } = EventType.Next;

        public DateTime UtcTimestamp { get; set; } = DateTime.UtcNow;

        public string Description { get; set; } = "";
    }

    public enum EventType
    {
        Next = 0,
        Stop = 1,
    }
}
