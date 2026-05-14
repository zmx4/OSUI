using OSUI.ViewModels;

namespace OSUI.Services;

public class NavigationService : PageViewModel, INavigationService
{
    // 用于从 DI 容器中解析 ViewModel 的工厂委托
    private readonly Func<Type, PageViewModel> _viewModelFactory;
    private PageViewModel _currentView;

    public PageViewModel CurrentView
    {
        get => _currentView;
        private set => SetProperty(ref _currentView, value); // 通知 UI 更新
    }

    public NavigationService(Func<Type, PageViewModel> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    public void NavigateTo<TViewModel>() where TViewModel : PageViewModel
    {
        // 通过 DI 容器获取目标 ViewModel 实例，并设置为当前视图
        CurrentView = _viewModelFactory(typeof(TViewModel));
    }
}