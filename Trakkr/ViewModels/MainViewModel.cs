using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trakkr.ViewModels
{
    public class MainViewModel
    {

        public ObservableCollection<EventViewModel> Events { get; set; } = new ObservableCollection<EventViewModel>();

    }
}
