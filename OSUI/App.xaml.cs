using System.Configuration;
using System.Data;
using System.Windows;
using OSUI.Views.Windows;

namespace OSUI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 首屏显示登录窗口
        new LoginWindow().Show();
    }
}
