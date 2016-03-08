using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace Trakkr.ViewModel
{
    public class ViewModelLocator
    {
        public MainViewModel MainViewModel => App.Container.Resolve<MainViewModel>();
    }
}
