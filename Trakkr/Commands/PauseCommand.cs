using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Trakkr.ViewModels;

namespace Trakkr.Commands
{
    public class PauseCommand : ICommand
    {
        public PauseCommand(MainViewModel viewModel)
        {

        }

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        public event EventHandler CanExecuteChanged;
    }
}
