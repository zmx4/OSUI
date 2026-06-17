using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using OSUI.ViewModels;

namespace OSUI.Views.Pages;

public partial class PageReplacementAlgorithmPage : UserControl
{
    public PageReplacementAlgorithmPage()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is PageReplacementAlgorithmPageViewModel oldVm)
            oldVm.PropertyChanged -= ViewModel_PropertyChanged;

        if (e.NewValue is PageReplacementAlgorithmPageViewModel newVm)
            newVm.PropertyChanged += ViewModel_PropertyChanged;
    }

    /// <summary>
    /// 当 CurrentStepIndex 变化时，自动滚动到对应步骤卡片
    /// </summary>
    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(PageReplacementAlgorithmPageViewModel.CurrentStepIndex))
            return;

        if (sender is not PageReplacementAlgorithmPageViewModel vm) return;
        if (vm.CurrentStepIndex < 0 || vm.CurrentStepIndex >= vm.Steps.Count) return;

        // 通过 ItemsControl 的 ItemContainerGenerator 获取容器并滚动到可见
        Dispatcher.BeginInvoke(() =>
        {
            var container = IcSteps.ItemContainerGenerator.ContainerFromIndex(vm.CurrentStepIndex);
            if (container is FrameworkElement element)
            {
                element.BringIntoView();
            }
        }, System.Windows.Threading.DispatcherPriority.Loaded);
    }
}
