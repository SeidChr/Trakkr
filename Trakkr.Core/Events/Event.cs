using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trakkr.Core.Events
{
    public class Event<TPayload> : BaseEvent<TPayload>, IEvent<TPayload>
    {
        public EventType Type { get; set; }
    }
}
