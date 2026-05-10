using CommunityToolkit.Mvvm.ComponentModel;
using OSUI.Data;
using OSUI.Services;

namespace OSUI.ViewModels;

public partial class HelloPageViewModel : PageViewModel
{
    [ObservableProperty]
    private string _helloText;

    public HelloPageViewModel(IAuthService authService)
    {
        PageNames = ApplicationPageNames.HelloPage;
        var username = authService.CurrentUser?.Username ?? "访客";
        _helloText = $"Hello, {username}!";
    }
}