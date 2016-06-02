using System;
using System.Windows;
using System.Windows.Controls;
using Autofac;
using Trakkr.Core;
using Trakkr.Core.Events;
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

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            new TrakkrEventViewModel
            {
                Type = EventType.Start,
                Timestamp = DateTime.Now,
                Payload = App.Container.Resolve<IRepositoryPayload>()
            };
        }
    }
}
