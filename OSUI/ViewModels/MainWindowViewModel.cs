using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OSUI.Services;

namespace OSUI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public INavigationService NavigationService { get; }
    
    [ObservableProperty]
    public partial string CurrentUsername { get; private set; } = "访客";
    
    public  MainWindowViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
        navigationService.NavigateTo<HelloPageViewModel>();
    }

    [RelayCommand]
    private void NavigateToScheduler()
    {
        NavigationService.NavigateTo<SchedulerPageViewModel>();
    }
}