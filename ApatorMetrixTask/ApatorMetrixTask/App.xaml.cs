using ApatorMetrixTask.Implementation;
using ApatorMetrixTask.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace ApatorMetrixTask
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        private void ConfigureServices()
        {

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IRepository, Repository>();
            serviceCollection.AddSingleton<IMessageBoxService, MessageBoxService>();
            serviceCollection.AddSingleton<MainWindow>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ConfigureServices();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
