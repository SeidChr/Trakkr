using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Trakkr.Core;
using Trakkr.Core.Events;
using Trakkr.YouTrack;

namespace Trakkr.ViewModel
{
    internal class StartCommand : ICommand
    {
        public MainViewModel MainViewModel { get; set; }

        public bool CanExecute(object parameter) => true;

        public Func<TrakkrEventViewModel> StartEventFactory { get; set; }

        public void Execute(object parameter)
        {
            var @event = StartEventFactory();
            MainViewModel.Events.Add(@event);
            MainViewModel.SelectedEvent = @event;
        }

        public event EventHandler CanExecuteChanged;
    }
}
