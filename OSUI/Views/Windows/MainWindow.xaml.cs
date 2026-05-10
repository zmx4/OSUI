using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace OSUI.Views.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var helloPage = App.ServiceProvider.GetRequiredService<Views.Pages.HelloPage>();
        MainFrame.Navigate(helloPage);
    }
}