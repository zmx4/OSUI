using System.Collections.ObjectModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using OSUI.Data;
using OSUI.Services;

namespace OSUI.ViewModels;

public sealed record LanguageOption(string Code, string DisplayName);

public partial class SettingsViewModel : PageViewModel
{
    private readonly IAuthService _authService;
    private readonly IPreferenceStorage _preferenceStorage;
    private readonly IDialogService _dialogService;
    private bool _isUpdatingLanguageSelection;
    
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

    [ObservableProperty]
    private ObservableCollection<LanguageOption>? _languageOptions;

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

    private string? _selectedLanguageCode;
    public string? SelectedLanguageCode
    {
        get => _selectedLanguageCode;
        set
        {
            if (!SetProperty(ref _selectedLanguageCode, value)
                || string.IsNullOrWhiteSpace(value)
                || _isUpdatingLanguageSelection)
            {
                return;
            }

            ApplyLanguage(value);
        }
    }

    public Action? OnOpenChangePasswordWindow { get; set; }
    
    public SettingsViewModel(IAuthService authService, IPreferenceStorage preferenceStorage,  IDialogService dialogService)
    {
        _authService = authService;
        _preferenceStorage = preferenceStorage;
        _dialogService = dialogService;
        
        ThemeNames = new ObservableCollection<string> { "Light", "Dark" };
        PageNames = ApplicationPageNames.SettingsPage;
        
        var paletteHelper = new PaletteHelper();
        var theme = paletteHelper.GetTheme();
        var initialTheme = theme.GetBaseTheme() == BaseTheme.Dark ? "Dark" : "Light";
        
        // 从存储加载选中的主题，如果没有保存过则使用当前的
        _selectedTheme = _preferenceStorage.Get(PreferenceKeys.Theme, initialTheme);

        var fonts = Fonts.SystemFontFamilies.Select(f => f.Source).OrderBy(f => f).ToList();
        FontNames = new ObservableCollection<string>(fonts);
        _selectedFont = fonts.FirstOrDefault();

        InitializeLanguageSelection();
    }

    #region 主题切换
    
    private void ApplyTheme(string themeName)
    {
        var paletteHelper = new PaletteHelper();
        var theme = paletteHelper.GetTheme();
        var isDark = themeName == "Dark";

        if (isDark)
            theme.SetBaseTheme(BaseTheme.Dark);
        else
            theme.SetBaseTheme(BaseTheme.Light);

        paletteHelper.SetTheme(theme);
        _preferenceStorage.Set(PreferenceKeys.Theme, themeName);

        // 更新自定义主题颜色资源
        ApplyCustomThemeColors(isDark);
    }

    private static void ApplyCustomThemeColors(bool isDark)
    {
        var res = Application.Current.Resources;
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
        var color = (Color)ColorConverter.ConvertFromString(hexColor);
        // 始终创建新 Brush，避免修改 WPF 冻结 (Frozen) 的资源对象
        dict[key] = new SolidColorBrush(color);
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

    #region 语言切换

    private void InitializeLanguageSelection()
    {
        var savedLanguageCode = _preferenceStorage.Get(PreferenceKeys.Language, LocalizationService.Instance.CurrentLanguageCode);
        var normalizedLanguageCode = LocalizationService.Instance.NormalizeLanguageCode(savedLanguageCode);

        if (!string.Equals(LocalizationService.Instance.CurrentLanguageCode, normalizedLanguageCode, StringComparison.OrdinalIgnoreCase))
        {
            LocalizationService.Instance.ApplyLanguage(normalizedLanguageCode);
        }

        RefreshLanguageOptions(normalizedLanguageCode);
    }

    private void ApplyLanguage(string languageCode)
    {
        var normalizedLanguageCode = LocalizationService.Instance.NormalizeLanguageCode(languageCode);
        LocalizationService.Instance.ApplyLanguage(normalizedLanguageCode);
        _preferenceStorage.Set(PreferenceKeys.Language, normalizedLanguageCode);
        RefreshLanguageOptions(normalizedLanguageCode);
    }

    private void RefreshLanguageOptions(string selectedLanguageCode)
    {
        _isUpdatingLanguageSelection = true;
        LanguageOptions = new ObservableCollection<LanguageOption>
        {
            new(LocalizationService.ChineseLanguageCode, LocalizationService.Instance.GetString("Settings.Language.ZhCN")),
            new(LocalizationService.EnglishLanguageCode, LocalizationService.Instance.GetString("Settings.Language.EnUS"))
        };
        SelectedLanguageCode = selectedLanguageCode;
        _isUpdatingLanguageSelection = false;
    }

    #endregion
    
    #region 关于窗口

    [RelayCommand]
    private void ShowAboutWindow()
    {
        var userName = _authService.CurrentUser?.Username ?? LocalizationService.Instance.GetString("Common.NotLoggedIn");
        var aboutText = LocalizationService.Instance.Format("Settings.About.Text", userName);
        // MessageBox.Show(aboutText, LocalizationService.Instance.GetString("Settings.About.Title"));
        _dialogService.ShowDialog(aboutText);
    }

    #endregion
}
