using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using OSUI.ViewModels;
using OSUI.Views.Windows;

namespace OSUI.Views.Pages;

public partial class SettingsPage : UserControl
{
    public SettingsPage()
    {
        InitializeComponent();
        DataContextChanged += SettingsPage_DataContextChanged;
        BindChangePasswordAction(DataContext as SettingsViewModel);
    }

    private void SettingsPage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is SettingsViewModel oldVm)
            oldVm.OnOpenChangePasswordWindow = null;

        BindChangePasswordAction(e.NewValue as SettingsViewModel);
    }

    private void BindChangePasswordAction(SettingsViewModel? vm)
    {
        if (vm is null)
            return;

        vm.OnOpenChangePasswordWindow = OpenChangePasswordWindow;
    }

    private void OpenChangePasswordWindow()
    {
        var window = App.ServiceProvider.GetRequiredService<ChangePasswordWindow>();
        var owner = Window.GetWindow(this);
        if (owner is not null)
        {
            window.Owner = owner;
        }

        window.ShowDialog();
    }
}