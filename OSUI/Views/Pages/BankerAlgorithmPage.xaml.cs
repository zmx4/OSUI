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
        AvailableResourceGrid.Children.Clear();
        AvailableResourceGrid.ColumnDefinitions.Clear();

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
        var resourceSuffix = BuildResourceSuffix(resourceCount);

        ProcessGrid.Columns.Add(CreateVectorColumn($"{maxLabel}{resourceSuffix}", "MaxText", false));
        ProcessGrid.Columns.Add(CreateVectorColumn($"{allocationLabel}{resourceSuffix}", "AllocationText", false));
        ProcessGrid.Columns.Add(CreateVectorColumn($"{needLabel}{resourceSuffix}", "NeedText", true));

        for (var i = 0; i < resourceCount; i++)
        {
            var header = LocalizationService.Instance.Format("Banker.Column.Resource", i + 1);
            BuildAvailableResourceColumn(i, header);
        }
    }

    private void BuildAvailableResourceColumn(int index, string header)
    {
        AvailableResourceGrid.ColumnDefinitions.Add(new ColumnDefinition
        {
            Width = new GridLength(1, GridUnitType.Star)
        });

        var headerBlock = new TextBlock
        {
            Text = header,
            FontWeight = FontWeights.SemiBold,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 6)
        };
        Grid.SetRow(headerBlock, 0);
        Grid.SetColumn(headerBlock, index);
        AvailableResourceGrid.Children.Add(headerBlock);

        var inputBox = new TextBox
        {
            MinWidth = 60,
            HorizontalContentAlignment = HorizontalAlignment.Center
        };
        inputBox.SetBinding(TextBox.TextProperty, new Binding($"AvailableResources[0][{index}]")
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });
        Grid.SetRow(inputBox, 1);
        Grid.SetColumn(inputBox, index);
        AvailableResourceGrid.Children.Add(inputBox);
    }

    private static DataGridTextColumn CreateVectorColumn(string header, string path, bool isReadOnly)
    {
        return new DataGridTextColumn
        {
            Header = header,
            Binding = new Binding(path)
            {
                Mode = isReadOnly ? BindingMode.OneWay : BindingMode.TwoWay,
                UpdateSourceTrigger = isReadOnly ? UpdateSourceTrigger.PropertyChanged : UpdateSourceTrigger.LostFocus
            },
            IsReadOnly = isReadOnly,
            MinWidth = 120
        };
    }

    private static string BuildResourceSuffix(int resourceCount)
    {
        if (resourceCount <= 0)
        {
            return string.Empty;
        }

        var labels = new string[resourceCount];
        for (var i = 0; i < resourceCount; i++)
        {
            labels[i] = $"R{i + 1}";
        }

        return $"({string.Join(",", labels)})";
    }
}
