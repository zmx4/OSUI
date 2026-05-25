using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using OSUI.Data;
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
        services.AddSingleton<IPreferenceStorage,JsonPreferenceStorage>();

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<HelloPageViewModel>();
        services.AddTransient<SchedulerPageViewModel>();
        services.AddTransient<BankerAlgorithmPageViewModel>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<ChangePasswordViewModel>();

        // Windows
        services.AddTransient<LoginWindow>();
        services.AddTransient<RegisterWindow>();
        services.AddTransient<MainWindow>();
        services.AddTransient<ChangePasswordWindow>();
        
        // Pages
        services.AddTransient<Views.Pages.HelloPage>();
        services.AddTransient<Views.Pages.Scheduler>();
        services.AddTransient<Views.Pages.BankerAlgorithmPage>();
        services.AddTransient<Views.Pages.SettingsPage>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var preferenceStorage = ServiceProvider.GetRequiredService<IPreferenceStorage>();
        var languageCode = preferenceStorage.Get(PreferenceKeys.Language, LocalizationService.ChineseLanguageCode);
        LocalizationService.Instance.ApplyLanguage(languageCode);

        // 首屏显示登录窗口
        var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
        loginWindow.Show();
    }
}
