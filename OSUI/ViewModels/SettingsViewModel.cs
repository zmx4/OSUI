using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OSUI.ViewModels;

public partial class SettingsViewModel : PageViewModel
{
    [ObservableProperty]
    private ObservableCollection<string>? _themeNames;
    
    public SettingsViewModel()
    {
        var app = Application.Current;
        ThemeNames = app.Resources["ThemeNames"] as ObservableCollection<string>;
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
    
}