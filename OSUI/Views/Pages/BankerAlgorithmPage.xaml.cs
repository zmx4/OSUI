using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using OSUI.Services;
using OSUI.ViewModels;

namespace OSUI.Views.Pages;

public partial class BankerAlgorithmPage : UserControl
{
    private BankerAlgorithmPageViewModel? _viewModel;

    public BankerAlgorithmPage()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AttachViewModel(DataContext as BankerAlgorithmPageViewModel);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        DetachViewModel();
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        DetachViewModel();
        AttachViewModel(e.NewValue as BankerAlgorithmPageViewModel);
    }

    private void AttachViewModel(BankerAlgorithmPageViewModel? viewModel)
    {
        if (viewModel is null)
        {
            return;
        }

        _viewModel = viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        BuildResourceColumns(_viewModel.ResourceTypeCount);
    }

    private void DetachViewModel()
    {
        if (_viewModel is null)
        {
            return;
        }

        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        _viewModel = null;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_viewModel is null)
        {
            return;
        }

        if (e.PropertyName == nameof(BankerAlgorithmPageViewModel.ResourceTypeCount))
        {
            BuildResourceColumns(_viewModel.ResourceTypeCount);
        }
    }

    private void BuildResourceColumns(int resourceCount)
    {
        ProcessGrid.Columns.Clear();

        var processHeader = LocalizationService.Instance.GetString("Banker.Column.ProcessId");
        ProcessGrid.Columns.Add(new DataGridTextColumn
        {
            Header = processHeader,
            Binding = new Binding("Id"),
            IsReadOnly = true
        });

        var allocationLabel = LocalizationService.Instance.GetString("Banker.Table.Allocation");
        var maxLabel = LocalizationService.Instance.GetString("Banker.Table.Max");
        var needLabel = LocalizationService.Instance.GetString("Banker.Table.Need");

        for (var i = 0; i < resourceCount; i++)
        {
            var header = LocalizationService.Instance.Format("Banker.Column.Resource", i + 1);
            ProcessGrid.Columns.Add(CreateResourceColumn($"{allocationLabel}-{header}", $"Allocation[{i}]", false));
            ProcessGrid.Columns.Add(CreateResourceColumn($"{maxLabel}-{header}", $"Max[{i}]", false));
            ProcessGrid.Columns.Add(CreateResourceColumn($"{needLabel}-{header}", $"Need[{i}]", true));
        }
    }

    private static DataGridTextColumn CreateResourceColumn(string header, string path, bool isReadOnly)
    {
        return new DataGridTextColumn
        {
            Header = header,
            Binding = new Binding(path)
            {
                Mode = isReadOnly ? BindingMode.OneWay : BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            },
            IsReadOnly = isReadOnly,
            MinWidth = 60
        };
    }
}
