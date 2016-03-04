using System;

using Trakkr.Core.Events;

namespace Trakkr.ViewModels
{
    public class EventViewModel
    {
        public IEvent<string> Event { get; set; }

        public EventViewModel(IEvent<string> @event)
        {
            Event = @event;
        }

        public DateTime UtcTimestamp { get; set; } = DateTime.UtcNow;

        public DateTime LocaTimestamp
        {
            get { return UtcTimestamp.ToLocalTime(); }
            set { UtcTimestamp = value.ToUniversalTime(); }
        }
        
        public string Description { get; set; }
    }
}
