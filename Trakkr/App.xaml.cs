using System.ComponentModel.Composition.Hosting;
using System.Windows;
using Autofac;
using Autofac.Integration.Mef;

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
            builder.RegisterInstance(App.Current.Resources[""]).ExternallyOwned();
            Container = builder.Build();
        }

        public static IContainer Container { get; }
    }
}
