using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Trakkr.ViewModels;

namespace Trakkr.Commands
{
    internal class NextCommand : ICommand
    {
        public MainViewModel ViewModel { get; set; }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ViewModel.EventCaptureSet.Next();
        }

        public event EventHandler CanExecuteChanged;
    }
}
