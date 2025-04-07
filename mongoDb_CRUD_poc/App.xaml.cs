using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using MongoDB.Driver;
using mongoDb_CRUD_poc.Core.Contracts.Services;
using mongoDb_CRUD_poc.Core.Seeders;
using mongoDb_CRUD_poc.Core.Services;
using MongoDbCrudPOC.Activation;
using MongoDbCrudPOC.Contracts.Services;
using MongoDbCrudPOC.Core.Contracts.Services;
using MongoDbCrudPOC.Core.Models;
using MongoDbCrudPOC.Core.Services;
using MongoDbCrudPOC.Helpers;
using MongoDbCrudPOC.Services;
using MongoDbCrudPOC.ViewModels;
using MongoDbCrudPOC.Views;

namespace MongoDbCrudPOC;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    //public static WindowEx MainWindow { get; } = new MainWindow();
    public static WindowEx MainWindow { get; private set; }

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers

            // Services
            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // File Services
            services.AddSingleton<IFileService, FileService>();

            // Sample Data Services
            services.AddSingleton<ISampleDataService, SampleDataService>();
            services.AddSingleton<ISamplePrintDataService, SamplePrintDataService>();

            // Register MongoDb client
            services.AddSingleton<IMongoClient>(_ => new MongoClient("mongodb://localhost:27017"));

            // MongoDb Services
            services.AddSingleton<IMongoDbService, MongoDbService>();
            services.AddSingleton<IPrintService, PrintService>();
            services.AddSingleton<ISliceService, SliceService>();
            services.AddSingleton<IMongoDbSeeder, MongoDbSeeder>();
            services.AddSingleton<IPrintSeeder, PrintSeeder>();

            // Create a scope and call the seeding method to add prints to the db
            var serviceProvider = services.BuildServiceProvider();
            var mongoDbSeeder = serviceProvider.GetRequiredService<IMongoDbSeeder>();

            // Seed or clear magnetoDb
            Task.Run(async () =>
            {
                // WARNING: only run one of these
                await mongoDbSeeder.ClearDatabaseAsync(true);
                //await mongoDbSeeder.SeedDatabaseAsync();
            });

            // Views and ViewModels
            services.AddTransient<DataGridViewModel>();
            services.AddTransient<DataGridPage>();
            services.AddTransient<BlankViewModel>();
            services.AddTransient<BlankPage>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();

            // Configuration
        }).
        Build();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        MainWindow = new MainWindow();
        MainWindow.Activate();

        await App.GetService<IActivationService>().ActivateAsync(args);
    }
}
