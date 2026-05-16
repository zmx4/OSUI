using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Data;
using HandyControl.Themes;
using OSUI.Services;

namespace OSUI.ViewModels;

public partial class SettingsViewModel : PageViewModel
{
    private readonly IAuthService _authService;
    
    [ObservableProperty]
    private ObservableCollection<string>? _themeNames;

    private string? _selectedTheme;
    public string? SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (SetProperty(ref _selectedTheme, value) && value != null)
            {
                ApplyTheme(value);
            }
        }
    }

    [ObservableProperty]
    private ObservableCollection<string>? _fontNames;

    private string? _selectedFont;
    public string? SelectedFont
    {
        get => _selectedFont;
        set
        {
            if (SetProperty(ref _selectedFont, value) && value != null)
            {
                ApplyFont(value);
            }
        }
    }

    public Action? OnOpenChangePasswordWindow { get; set; }
    
    public SettingsViewModel(IAuthService  authService)
    {
        _authService = authService;
        ThemeNames = new ObservableCollection<string> { "Default", "Dark", "Violet" };
        _selectedTheme = "Default";

        var fonts = Fonts.SystemFontFamilies.Select(f => f.Source).OrderBy(f => f).ToList();
        FontNames = new ObservableCollection<string>(fonts);
        _selectedFont = fonts.FirstOrDefault();
    }

    #region 主题切换
    
    private void ApplyTheme(string themeName)
    {
        if (Enum.TryParse(themeName, out SkinType skinType))
        {
            var dictionaries = Application.Current.Resources.MergedDictionaries;
            var themeDict = dictionaries.FirstOrDefault(d => d is Theme);
            if (themeDict is Theme hcTheme)
            {
                hcTheme.Skin = skinType;
            }
        }
    }

    #endregion

    #region 字体切换

    private void ApplyFont(string fontName)
    {
        var app = Application.Current;
        if (app != null)
        {
            var fontFamily = new FontFamily(fontName);
            foreach (Window window in app.Windows)
            {
                window.FontFamily = fontFamily;
            }
        }
    }

    #endregion

    #region 修改密码窗口

    [RelayCommand]
    private void OpenChangePasswordWindow()
    {
        OnOpenChangePasswordWindow?.Invoke();
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