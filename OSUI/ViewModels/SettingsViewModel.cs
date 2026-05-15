using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OSUI.Services;

namespace OSUI.ViewModels;

public partial class SettingsViewModel : PageViewModel
{
    private readonly IAuthService _authService;
    
    [ObservableProperty]
    private ObservableCollection<string>? _themeNames;
    
    public SettingsViewModel(IAuthService  authService)
    {
        _authService = authService;
        var app = Application.Current;
        ThemeNames = ["Default"];
    }

    #region 主题切换

    private void OnSelectTheme(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems[0] is string theme)
        {
            ApplyTheme(theme);
        }
    }
    
    private void ApplyTheme(string themeName)
    {
        var app = Application.Current;
        if(app == null)return;
        var theme = app.Resources["ThemeName"] as string;
        if(theme == null)return;
        if (theme == themeName) return;
        app.Resources["ThemeName"] = themeName;
    }

    #endregion

    #region 关于窗口

    [RelayCommand]
    private void ShowAboutWindow()
    {
        var userName = _authService.CurrentUser?.Username;
        MessageBox.Show($"OSUI - \n" +
                        $"当前用户: {userName ?? "未登录"}" +
                        $"\n版本: 1.0.0\n© 2024 OSUI Team", 
                    "关于 OSUI");
    }

    #endregion
}