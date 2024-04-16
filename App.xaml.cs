using Microsoft.Extensions.DependencyInjection;
using SharpYTDWPF.Core;
using SharpYTDWPF.MVVM.View;
using SharpYTDWPF.MVVM.ViewModel;
using SharpYTDWPF.Services;
using System.Configuration;
using System.Data;
using System.Windows;

namespace SharpYTDWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<MainWindow>(serviceProvider => new MainWindow
            {
                DataContext = serviceProvider.GetRequiredService<MainViewModel>()
            });
            services.AddSingleton<DownloadViewModel>();
            services.AddSingleton<ConvertViewModel>();
            services.AddSingleton<StatusViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDownloadService, DownloadService>();

            services.AddSingleton<Func<Type, ViewModel>>(serviceProvider => viewModelType => (ViewModel)serviceProvider.GetRequiredService(viewModelType));
            
            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _serviceProvider.GetRequiredService<MainWindow>().Show();
            base.OnStartup(e);
        }
    }

}
