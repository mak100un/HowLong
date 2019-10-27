using Autofac;
using HowLong.Data;
using HowLong.Navigation;
using HowLong.ViewModels;
using HowLong.Views;

namespace HowLong.Containers
{
    public class CompositionRoot
    {
        private static IContainer _container;
        public static void Init()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<HistoryViewModel>().SingleInstance();
            builder.RegisterType<AccountViewModel>().InstancePerDependency();
            builder.RegisterType<MainViewModel>().SingleInstance();
            builder.RegisterType<SettingsViewModel>().SingleInstance();
            builder.RegisterType<WorkDaysViewModel>().SingleInstance();
            builder.RegisterType<MainPage>().SingleInstance();
            builder.RegisterType<TimeAccountingContext>().SingleInstance(); 
            builder.RegisterType<NavigationService>()
                .As<INavigationService>()
                .SingleInstance();
            _container = builder.Build();

            NavigationService.Init();
        }

        public static T Resolve<T>() => _container.Resolve<T>();
    }
}
