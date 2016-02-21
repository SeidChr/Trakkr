using System.ComponentModel.Composition.Hosting;
using System.Windows;
using Autofac;
using Autofac.Integration.Mef;
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
                .Register(b => b.Resolve<Application>().Resources["MainViewModel"])
                .As<MainViewModel>()
                .ExternallyOwned();

            builder.Register(b => b.ResolveKeyed<IEventRepository>("TextFile"))
                .Named<IEventRepository>("EventRepository");

            Container = builder.Build();
        }

        public static IContainer Container { get; }
    }
}
