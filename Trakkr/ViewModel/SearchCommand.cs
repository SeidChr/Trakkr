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
    internal class SearchCommand : ICommand
    {
        public MainViewModel MainViewModel { get; set; }

        public IRepository<string, IRepositoryPayload> Repositroy { get; set; }

        public bool CanExecute(object parameter) => true;
        //!string.IsNullOrWhiteSpace(MainViewModel.SelectedEvent.Query);

        public void Execute(object parameter)
        {
            MainViewModel.SelectedEvent.FoundTickets 
                = Repositroy.FindTickets(MainViewModel.SelectedEvent.Query);
        }

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged() 
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
