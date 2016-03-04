using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.Windows.Input;
using Autofac;
using Autofac.Core;
using Trakkr.Commands;
using Trakkr.Model;

namespace Trakkr.ViewModels
{
    public class MainViewModel
    {
        public ICommand Pause { get; set; }

        public ICommand Next { get; set; }

        public IEventCaptureSet<string> EventCaptureSet { get; set; }
    }
}
