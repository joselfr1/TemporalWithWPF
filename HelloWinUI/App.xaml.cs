using HelloWinUI.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Model.Dtos;
using System.Configuration;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Temporalio.Client;

namespace HelloWinUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            try
            {
                // 1. Load Configurations
                IConfiguration config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var options = config.GetSection("Configurations").Get<ExecutionWorkflowClient>()
                    ?? throw new ArgumentException("Configuration missing");


                // 2. Connect and Register the Temporal Client as a Singleton
                var client = await TemporalClient.ConnectAsync(new(options.Endpoint ?? "localhost:7233"));

                services.AddSingleton(options);
                services.AddSingleton<ITemporalClient>(client);

                // Register both components here
                services.AddTransient<SayHelloViewModel>();
                services.AddTransient<MainWindow>();

                var serviceProvider = services.BuildServiceProvider();

                // Option A: Resolve directly from the provider
                var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
   
           
        }

    }

}
