using Autofac;
using Autofac.Core;
using HR.DataAccess;
using HR.UI.Data;
using HR.UI.Data.Lookups;
using HR.UI.Data.Repositories;
using HR.UI.View.Services;
using HR.UI.ViewModel;
using Prism.Events;

namespace HR.UI.Startup
{
    class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<EventAggregator>().As<IEventAggregator>()
                .SingleInstance();

            builder.RegisterType<HrDbContext>().AsSelf();

            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MessageDialogService>()
                .As<IMessageDialogService>();

            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<NavigationViewModel>()
                .As<INavigationViewModel>();
            builder.RegisterType<CandidateDetailViewModel>()
                .As<ICandidateDetailViewModel>();

            builder.RegisterType<CandidateRepository>()
                .As<ICandidateRepository>();
            builder.RegisterType<LookupDataService>()
                .AsImplementedInterfaces();
            
            return builder.Build();
        }
    }
}
