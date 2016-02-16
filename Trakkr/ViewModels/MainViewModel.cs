using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;

namespace Trakkr.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<EventViewModel> Events { get; set; } = new ObservableCollection<EventViewModel>();
    }
}
