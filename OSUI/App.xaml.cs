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
        services.AddTransient<PageReplacementAlgorithmPageViewModel>();

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
        services.AddTransient<Views.Pages.PageReplacementAlgorithmPage>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var preferenceStorage = ServiceProvider.GetRequiredService<IPreferenceStorage>();
        var languageCode = preferenceStorage.Get(PreferenceKeys.Language, LocalizationService.ChineseLanguageCode);
        LocalizationService.Instance.ApplyLanguage(languageCode);
        
        var themeString = preferenceStorage.Get(PreferenceKeys.Theme, "Light");
        var isDark = themeString == "Dark";
        var paletteHelper = new PaletteHelper();
        var theme = paletteHelper.GetTheme();
        theme.SetBaseTheme(isDark ? BaseTheme.Dark : BaseTheme.Light);
        paletteHelper.SetTheme(theme);

        // 应用自定义主题颜色
        ApplyCustomThemeColors(isDark);

        // 首屏显示登录窗口
        var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
        loginWindow.Show();
    }

    private static void ApplyCustomThemeColors(bool isDark)
    {
        var res = Current.Resources;
        SetBrush(res, "StepHitBackground",      isDark ? "#1B3B1F" : "#E8F5E9");
        SetBrush(res, "StepHitForeground",      isDark ? "#66BB6A" : "#2E7D32");
        SetBrush(res, "StepFaultBackground",    isDark ? "#3B1B1F" : "#FFEBEE");
        SetBrush(res, "StepFaultForeground",    isDark ? "#EF5350" : "#C62828");
        SetBrush(res, "StepCurrentBorder",      isDark ? "#FFB74D" : "#FF9800");
        SetBrush(res, "StepBorder",             isDark ? "#424242" : "#CCCCCC");
        SetBrush(res, "FrameBackground",        isDark ? "#1A2733" : "#E3F2FD");
        SetBrush(res, "StepLabelForeground",    isDark ? "#BDBDBD" : "#666666");
        SetBrush(res, "NoteForeground",         isDark ? "#9E9E9E" : "#888888");
        SetBrush(res, "StatusForeground",       isDark ? "#E0E0E0" : "#333333");
        SetBrush(res, "VisualizationBorderBrush", isDark ? "#616161" : "#BDBDBD");
    }

    private static void SetBrush(ResourceDictionary dict, string key, string hexColor)
    {
        var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hexColor);
        // 始终创建新 Brush，避免修改 WPF 冻结 (Frozen) 的资源对象
        dict[key] = new System.Windows.Media.SolidColorBrush(color);
    }
}
