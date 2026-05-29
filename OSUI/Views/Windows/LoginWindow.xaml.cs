using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using OSUI.ViewModels;

namespace OSUI.Views.Windows;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        
        if (DataContext is LoginViewModel vm)
        {
            vm.OnLoginSuccess = () =>
            {
                // 登录成功：打开主窗口，关闭自身
                var main = App.ServiceProvider.GetRequiredService<MainWindow>();
                Application.Current.MainWindow = main;
                main.Show();
                Close();
            };
            
            vm.OnGoToRegister = () =>
            {
                // 跳转到注册窗口
                var register = App.ServiceProvider.GetRequiredService<RegisterWindow>();
                register.Show();
                Close();
            };
        }
    }
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm)
            vm.Password = ((PasswordBox)sender).Password;
    }
}