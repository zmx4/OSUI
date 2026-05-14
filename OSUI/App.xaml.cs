using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using OSUI.Services;
using OSUI.ViewModels;
using OSUI.Views.Windows;

namespace OSUI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Services
        services.AddSingleton<Func<Type, PageViewModel>>(serviceProvider => 
            viewModelType => (PageViewModel)serviceProvider.GetRequiredService(viewModelType));
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IAuthService, AuthService>();

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<HelloPageViewModel>();
        services.AddTransient<MainWindowViewModel>();

        // Windows
        services.AddTransient<LoginWindow>();
        services.AddTransient<RegisterWindow>();
        services.AddTransient<MainWindow>();
        
        // Pages
        services.AddTransient<Views.Pages.HelloPage>();
        services.AddTransient<Views.Pages.Scheduler>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 首屏显示登录窗口
        var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
        loginWindow.Show();
    }
}
