using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OSUI.Services;

namespace OSUI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public INavigationService NavigationService { get; }
    
    [ObservableProperty]
    public partial string CurrentUsername { get; private set; } = LocalizationService.Instance.GetString("Common.Guest");
    
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

    [RelayCommand]
    private void NavigateToBankerAlgorithm()
    {
        NavigationService.NavigateTo<BankerAlgorithmPageViewModel>();
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        NavigationService.NavigateTo<SettingsViewModel>();
    }
}
