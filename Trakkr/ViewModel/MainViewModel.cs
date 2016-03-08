using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Trakkr.Annotations;

namespace Trakkr.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private TrakkrEventViewModel selectedEvent;

        public ObservableCollection<TrakkrEventViewModel> Events { get; set; } 
            = new ObservableCollection<TrakkrEventViewModel>();

        public TrakkrEventViewModel SelectedEvent
        {
            get { return selectedEvent; }
            set
            {
                if (Equals(value, selectedEvent)) return;
                selectedEvent = value;
                OnPropertyChanged();
            }
        }

        public ICommand StartCommand { get; set; }

        public ICommand StopCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
