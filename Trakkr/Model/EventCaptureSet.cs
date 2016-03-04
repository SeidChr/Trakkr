using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trakkr.Core.Events;

namespace Trakkr.Model
{
    public class EventCaptureSet : IEventCaptureSet<string>
    {
        private readonly ObservableCollection<IEvent<string>> events;

        public EventCaptureSet()
        {
            events = new ObservableCollection<IEvent<string>>();
            Events = new ReadOnlyObservableCollection<IEvent<string>>(events);
        }

        public IEvent<string> Next()
        {
            var result = new Event
            {
                Type = EventType.Start,
            };

            events.Add(result);

            return result;
        }

        public IEvent<string> Pause()
        {
            var result = new Event
            {
                Type = EventType.Stop,
            };

            events.Add(result);

            return result;
        }

        public IEnumerable<IEvent<string>> Events { get; }
    }
}
