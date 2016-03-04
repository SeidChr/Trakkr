using System.ComponentModel.Composition.Hosting;
using System.Windows;
using System.Windows.Input;
using Autofac;
using Autofac.Integration.Mef;
using Trakkr.Commands;
using Trakkr.Model;
using Trakkr.ViewModels;

namespace Trakkr
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            var catalog = new AssemblyCatalog(typeof(App).Assembly);
            var builder = new ContainerBuilder();

            builder.RegisterComposablePartCatalog(catalog);

            builder
                .Register(_ => App.Current)
                .As<Application>()
                .ExternallyOwned();

            builder
                .RegisterType<MainViewModel>()
                .AsSelf()
                .PropertiesAutowired();

            builder.Register(b => b.ResolveKeyed<IEventRepository>("TextFile"))
                .Named<IEventRepository>("EventRepository");

            builder.RegisterType<EventCaptureSet>().As<IEventCaptureSet<string>>();

            builder.RegisterType<NextCommand>().Named<ICommand>("Next").PropertiesAutowired();
            builder.RegisterType<PauseCommand>().Named<ICommand>("Pause").PropertiesAutowired();

            Container = builder.Build();
        }

        public static IContainer Container { get; }
    }
}
