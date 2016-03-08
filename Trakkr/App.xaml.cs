using System;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using System.Windows.Input;
using Autofac;
using Autofac.Integration.Mef;
using Trakkr.Core;
using Trakkr.Core.Events;
using Trakkr.ViewModel;
using Trakkr.YouTrack;

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
                .Register(b => new MainViewModel())
                .OnActivating(e =>
                {
                    e.Instance.StartCommand = e.Context.Resolve<StartCommand>();
                    e.Instance.StopCommand = e.Context.Resolve<StopCommand>();
                })
                .AsSelf()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<YouTrackPayload>()
                .As<IRepositoryPayload>()
                .InstancePerDependency();

            builder
                .RegisterType<StartCommand>()
                .OnActivated(e =>
                {
                    e.Instance.MainViewModel = e.Context.Resolve<MainViewModel>();
                    e.Instance.StartEventFactory =
                        () => new TrakkrEventViewModel
                        {
                            Type = EventType.Start,
                            Timestamp = DateTime.Now,
                            Payload = Container.Resolve<IRepositoryPayload>()
                        };
                })
                .AsSelf()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<StopCommand>()
                .OnActivated(e =>
                {
                    e.Instance.MainViewModel = e.Context.Resolve<MainViewModel>();
                    e.Instance.StopEventFactory =
                        () => new TrakkrEventViewModel
                        {
                            Type = EventType.Stop,
                            Timestamp = DateTime.Now,
                            Payload = Container.Resolve<IRepositoryPayload>()
                        };
                })
                .AsSelf()
                .InstancePerLifetimeScope();
        
            Container = builder.Build();
        }

        public static IContainer Container { get; }
    }
}
