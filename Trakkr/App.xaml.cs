using System;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using System.Windows.Input;
using Autofac;
using Autofac.Integration.Mef;
using Trakkr.Core;
using Trakkr.Core.Events;
using Trakkr.Properties;
using Trakkr.ViewModel;
using Trakkr.YouTrack;
using YouTrackSharp.Issues;

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
                .ExternallyOwned()
                .InstancePerLifetimeScope();

            builder
                .Register(b => new MainViewModel())
                .OnActivating(e =>
                {
                    e.Instance.StartCommand = e.Context.Resolve<StartCommand>();
                    e.Instance.StopCommand = e.Context.Resolve<StopCommand>();
                    e.Instance.SearchCommand = e.Context.Resolve<SearchCommand>();
                })
                .AsSelf()
                .InstancePerLifetimeScope();

            builder
                .Register(b => YouTrackManager.GetConnection("youtrack.neveling.net", "testuser", "testuser"))
                .As<IConnection>()
                .InstancePerLifetimeScope();

            builder
                .Register(b=>new IssueManagement(b.Resolve<IConnection>()))
                .AsSelf()
                .InstancePerLifetimeScope();

            builder
                .RegisterType(typeof (YouTrackRepository))
                .As(typeof(IRepository<string, IRepositoryPayload>))
                .InstancePerLifetimeScope();

            builder
                .RegisterType<YouTrackPayload>()
                .OnActivated(e=>e.Instance.Repository = e.Context.Resolve<IRepository<string, IRepositoryPayload>>())
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

            builder
                .RegisterType<SearchCommand>()
                .OnActivated(e =>
                {
                    e.Instance.MainViewModel = e.Context.Resolve<MainViewModel>();
                    e.Instance.Repositroy = e.Context.Resolve<IRepository<string, IRepositoryPayload>>();
                })
                .AsSelf()
                .InstancePerLifetimeScope();

            Container = builder.Build();
        }

        public static IContainer Container { get; }
    }
}
