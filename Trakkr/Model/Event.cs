using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Trakkr.Annotations;

namespace Trakkr.Model
{
    public class Event : IEvent
    {
        public EventType Type { get; set; } = EventType.Next;

        public DateTime UtcTimestamp { get; set; } = DateTime.UtcNow;

        public string Description { get; set; } = "";

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
