using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Trakkr.Core;
using Trakkr.Core.Events;
using Trakkr.YouTrack;

namespace Trakkr.ViewModel
{
    internal class StopCommand : ICommand
    {
        private MainViewModel mainViewModel;

        public MainViewModel MainViewModel
        {
            get { return mainViewModel; }
            set
            {
                mainViewModel = value;
                mainViewModel.Events.CollectionChanged += (sender, args) => OnCanExecuteChanged();
            }
        }

        public Func<TrakkrEventViewModel> StopEventFactory { get; set; }

        public bool CanExecute(object parameter) 
            => MainViewModel.Events.Count > 0
            && MainViewModel.Events.Last().Type == EventType.Start;

        public void Execute(object parameter)
        {
            var @event = StopEventFactory();
            MainViewModel.Events.Add(@event);
            MainViewModel.SelectedEvent = @event;
        }

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged() 
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
