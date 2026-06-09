using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Controls;
using MaterialDesignThemes.Wpf;
using OSUI.Messages;
using OSUI.Services;

namespace OSUI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public INavigationService NavigationService { get; }
    
    [ObservableProperty]
    public partial string CurrentUsername { get; private set; } = LocalizationService.Instance.GetString("Common.Guest");
    
    public  MainWindowViewModel(INavigationService navigationService)
    {
        // WeakReferenceMessenger.Default.Register<DialogMessage>(this, (r, m) =>
        // {
        //     _ = ShowDialog(m.Value.Item1);
        // } );
        NavigationService = navigationService;
        navigationService.NavigateTo<HelloPageViewModel>();
    }

    private async Task ShowDialog(string message)
    {
        await DialogHost.Show(message, "DialogHost");
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
