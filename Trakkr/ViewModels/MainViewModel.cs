using System.Collections.ObjectModel;

namespace Trakkr.ViewModels
{
    public class MainViewModel
    {


        public ObservableCollection<EventViewModel> Events { get; set; } = new ObservableCollection<EventViewModel>();

    }
}
