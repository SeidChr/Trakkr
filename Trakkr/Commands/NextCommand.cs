using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Trakkr.ViewModels;

namespace Trakkr.Commands
{
    class NextCommand : ICommand
    {
        private MainViewModel ViewModel { get; set; }

        public NextCommand(MainViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ViewModel.
        }

        public event EventHandler CanExecuteChanged;
    }
}
