using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OSUI.Models;

/// <summary>
/// 页面置换算法步骤可视化模型
/// </summary>
public partial class PageReplacementStepViewModel : ObservableObject
{
    [ObservableProperty]
    private string _stepLabel = string.Empty;

    [ObservableProperty]
    private string _pageNum = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> _frames = [];

    [ObservableProperty]
    private string _note = string.Empty;

    [ObservableProperty]
    private bool _isHit;

    [ObservableProperty]
    private bool _isCurrent;

    [ObservableProperty]
    private int _stepIndex;
}
