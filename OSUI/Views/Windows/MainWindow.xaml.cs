using System.Windows;

namespace OSUI.Views.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(Views.Pages.HelloPage helloPage)
    {
        InitializeComponent();
        // MainFrame.Navigate(helloPage);
    }
}