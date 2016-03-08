using System;
using System.Windows;
using System.Windows.Controls;
using Autofac;
using Trakkr.ViewModel;

namespace Trakkr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel MainViewModel = App.Container.Resolve<MainViewModel>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void EventList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EventList.ScrollIntoView(EventList.SelectedItem);
        }
    }
}
