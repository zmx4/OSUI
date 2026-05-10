using System.Windows.Controls;

namespace OSUI.Views.Pages;

public partial class HelloPage : Page
{
    public HelloPage(ViewModels.HelloPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}