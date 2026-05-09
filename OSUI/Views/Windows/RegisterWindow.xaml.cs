using System.Windows;
using System.Windows.Controls;
using OSUI.ViewModels;

namespace OSUI.Views.Windows;

public partial class RegisterWindow
{
    public RegisterWindow()
    {
        InitializeComponent();
        
        DataContext = new RegisterViewModel(
            onRegisterSuccess: () =>
            {
                // 注册成功，提示并返回登录页面
                MessageBox.Show("注册成功！请登录。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                var login = new LoginWindow();
                login.Show();
                Close();
            },
            onCancel: () =>
            {
                // 取消，返回登录页面
                var login = new LoginWindow();
                login.Show();
                Close();
            });
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegisterViewModel vm)
            vm.Password = ((PasswordBox)sender).Password;
    }

    private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegisterViewModel vm)
            vm.ConfirmPassword = ((PasswordBox)sender).Password;
    }
}