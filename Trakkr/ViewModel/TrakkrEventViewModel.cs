using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Trakkr.Annotations;
using Trakkr.Core;
using Trakkr.Core.Events;

namespace Trakkr.ViewModel
{
    public class TrakkrEventViewModel : Event<IRepositoryPayload>, INotifyPropertyChanged
    {
        private IEnumerable<string> foundTickets;

        public string Time
        {
            get { return Timestamp.ToShortTimeString(); }
        }

        public string Kind
        {
            get
            {
                string result = "stop";
                if (Type == EventType.Start)
                {
                    result = "start";
                }
                return result;
            }
        }

        public string Query
        {
            get { return Payload.Query; }
            set
            {
                if (value == Payload.Query) return;
                Payload.Query = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FoundTickets));
            }
        }

        public IEnumerable<string> FoundTickets
        {
            get { return foundTickets; }
            set
            {
                if (Equals(value, foundTickets)) return;
                foundTickets = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
