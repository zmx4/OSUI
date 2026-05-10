using CommunityToolkit.Mvvm.ComponentModel;

namespace OSUI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string CurrentUsername { get; private set; } = "访客";
}