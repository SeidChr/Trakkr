using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trakkr.Core.Events
{
    public abstract class BaseEvent<TPayload> : IBaseEvent<TPayload>
    {
        public DateTime Timestamp { get; set; }

        public TPayload Payload { get; set; }
    }
}
