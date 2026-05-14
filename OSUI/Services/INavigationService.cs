using OSUI.ViewModels;

namespace OSUI.Services;

public interface INavigationService
{
    // 当前显示的 ViewModel
    PageViewModel CurrentView { get; }
        
    // 导航到指定的 ViewModel
    void NavigateTo<TViewModel>() where TViewModel : PageViewModel;
}