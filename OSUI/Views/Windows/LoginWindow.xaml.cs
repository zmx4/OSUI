using System.Windows;
using System.Windows.Controls;
using OSUI.ViewModels;

namespace OSUI.Views.Windows;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        
        DataContext = new LoginViewModel(
            onLoginSuccess: () =>
            {
                // 登录成功：打开主窗口，关闭自身
                var main = new MainWindow();
                main.Show();
                Close();
            },
            onGoToRegister: () =>
            {
                // 跳转到注册窗口
                var register = new RegisterWindow();
                register.Show();
                Close();
            });
    }
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm)
            vm.Password = ((PasswordBox)sender).Password;
    }
}