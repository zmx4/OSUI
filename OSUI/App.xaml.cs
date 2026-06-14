using System.Configuration;
using System.Data;
using System.Windows;
using MaterialDesignThemes.Wpf;
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
        services.AddSingleton<IDialogService, DialogService>();

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<HelloPageViewModel>();
        services.AddTransient<SchedulerPageViewModel>();
        services.AddTransient<BankerAlgorithmPageViewModel>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<ChangePasswordViewModel>();
        services.AddTransient<DiskSeekAlgorithmPageViewModel>();

        // Windows
        services.AddTransient<LoginWindow>();
        services.AddTransient<RegisterWindow>();
        services.AddSingleton<MainWindow>();
        services.AddTransient<ChangePasswordWindow>();
        
        // Pages
        services.AddTransient<Views.Pages.HelloPage>();
        services.AddTransient<Views.Pages.Scheduler>();
        services.AddTransient<Views.Pages.BankerAlgorithmPage>();
        services.AddTransient<Views.Pages.SettingsPage>();
        services.AddTransient<Views.Pages.DiskSeekAlgorithmPage>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var preferenceStorage = ServiceProvider.GetRequiredService<IPreferenceStorage>();
        var languageCode = preferenceStorage.Get(PreferenceKeys.Language, LocalizationService.ChineseLanguageCode);
        LocalizationService.Instance.ApplyLanguage(languageCode);
        
        var themeString = preferenceStorage.Get(PreferenceKeys.Theme, "Light");
        var paletteHelper = new PaletteHelper();
        var theme = paletteHelper.GetTheme();
        theme.SetBaseTheme(themeString == "Dark" ? BaseTheme.Dark : BaseTheme.Light);
        paletteHelper.SetTheme(theme);

        // 首屏显示登录窗口
        var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
        loginWindow.Show();
    }
}
