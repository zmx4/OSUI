using System.Collections.ObjectModel;
using System;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Data;
using HandyControl.Themes;
using OSUI.Data;
using OSUI.Services;

namespace OSUI.ViewModels;

public sealed record LanguageOption(string Code, string DisplayName);

public partial class SettingsViewModel : PageViewModel
{
    private readonly IAuthService _authService;
    private readonly IPreferenceStorage _preferenceStorage;
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
    
    public SettingsViewModel(IAuthService authService, IPreferenceStorage preferenceStorage)
    {
        _authService = authService;
        _preferenceStorage = preferenceStorage;
        ThemeNames = new ObservableCollection<string> { "Default", "Dark", "Violet" };
        _selectedTheme = "Default";

        var fonts = Fonts.SystemFontFamilies.Select(f => f.Source).OrderBy(f => f).ToList();
        FontNames = new ObservableCollection<string>(fonts);
        _selectedFont = fonts.FirstOrDefault();

        InitializeLanguageSelection();
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
        MessageBox.Show(aboutText, LocalizationService.Instance.GetString("Settings.About.Title"));
    }

    #endregion
}
