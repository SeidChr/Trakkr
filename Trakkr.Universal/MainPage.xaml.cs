using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Autofac;
using Trakkr.Core.Events;
using Event = Trakkr.Core.Events.IEvent<Trakkr.Core.IRepositoryPayload>;

// Die Vorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 dokumentiert.

namespace Trakkr.Universal
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            AddEvent(App.StartEventConstructorServiceId);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            var lastEvent = EventCollection.LastOrDefault();
            if (lastEvent != null && lastEvent.Type != EventType.Stop)
            {
                AddEvent(App.StopEventConstructorServiceId);
            }
        }

        private void AddEvent(string constructorServiceId)
        {
            var newEvent = App.Container.ResolveKeyed<Event>(constructorServiceId);
            EventCollection.Add(newEvent);
            EventList.ScrollIntoView(EventList.Items.LastOrDefault());
            EventList.SelectedItem = newEvent;
        }
    }
}
