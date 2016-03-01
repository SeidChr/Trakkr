using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Trakkr.Model;

namespace Trakkr.ViewModels
{
    public class EventViewModel
    {
        public IEvent Event { get; set; }

        public EventViewModel(IEvent @event)
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
