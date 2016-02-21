using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.Windows.Input;

namespace Trakkr.ViewModels
{
    public class MainViewModel
    {
        public ICommand Next { get; set; }

        public ObservableCollection<EventViewModel> Events { get; set; } = new ObservableCollection<EventViewModel>();
    }
}
