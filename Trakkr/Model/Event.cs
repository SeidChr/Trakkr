using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Trakkr.Annotations;
using Trakkr.Core;
using Trakkr.Core.Events;

namespace Trakkr.Model
{
    public class Event : IEvent<string>
    {
        public EventType Type { get; set; } = EventType.Start;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string Payload { get; set; } = "";

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
