using Autofac;
using Autofac.Core;
using HR.DataAccess;
using HR.UI.Data;
using HR.UI.ViewModel;

namespace HR.UI.Startup
{
    class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<HrDbContext>().AsSelf();
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<CandidateDataService>()
                .As<ICandidateDataService>();

            return builder.Build();
        }
    }
}
