using CommunityToolkit.Mvvm.ComponentModel;
using OSUI.Data;

namespace OSUI.ViewModels;

public partial class PageViewModel : ObservableObject
{
    [ObservableProperty]
    private ApplicationPageNames _pageNames;
    /// <summary>
    /// 设置页面导航参数。
    /// </summary>
    /// <param name="parameter">页面参数对象。</param>
    public virtual void SetParameter(object parameter) { }
}