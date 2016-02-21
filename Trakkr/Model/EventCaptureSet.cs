using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trakkr.Model
{
    public class EventCaptureSet : IEventCaptureSet
    {
        private ObservableCollection<IEvent> events;

        public EventCaptureSet()
        {
            events = new ObservableCollection<IEvent>();
            Events = new ReadOnlyObservableCollection<IEvent>(events);
        }

        public IEvent Next()
        {
            var result = new Event
            {
                Type = EventType.Next,
            };

            events.Add(result);

            return result;
        }

        public IEvent Pause()
        {
            var result = new Event
            {
                Type = EventType.Pause,
            };

            events.Add(result);

            return result;
        }

        public IEnumerable<IEvent> Events { get; }
    }
}
