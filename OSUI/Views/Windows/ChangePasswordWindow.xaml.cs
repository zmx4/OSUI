using System.Windows;
using System.Windows.Controls;
using OSUI.ViewModels;

namespace OSUI.Views.Windows;

public partial class ChangePasswordWindow : Window
{
    public ChangePasswordWindow()
    {
        InitializeComponent();
        DataContextChanged += ChangePasswordWindow_DataContextChanged;
        BindActions(DataContext as ChangePasswordViewModel);
    }

    private void ChangePasswordWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is ChangePasswordViewModel oldVm)
        {
            oldVm.OnPasswordChanged = null;
            oldVm.OnCancel = null;
        }

        BindActions(e.NewValue as ChangePasswordViewModel);
    }

    private void BindActions(ChangePasswordViewModel? vm)
    {
        if (vm is null)
        {
            return;
        }

        vm.OnPasswordChanged = () =>
        {
            MessageBox.Show("密码已更新。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        };

        vm.OnCancel = Close;
    }

    private void CurrentPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChangePasswordViewModel vm)
            vm.CurrentPassword = ((PasswordBox)sender).Password;
    }

    private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChangePasswordViewModel vm)
            vm.NewPassword = ((PasswordBox)sender).Password;
    }

    private void ConfirmNewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChangePasswordViewModel vm)
            vm.ConfirmNewPassword = ((PasswordBox)sender).Password;
    }
}
