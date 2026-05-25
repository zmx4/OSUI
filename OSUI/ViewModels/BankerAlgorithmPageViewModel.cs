using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OSUI.Models;

namespace OSUI.ViewModels;

public partial class BankerAlgorithmPageViewModel : PageViewModel
{
    private const int DefaultProcessCount = 5;
    private const int DefaultResourceTypeCount = 3;

    [ObservableProperty]
    private int _processCount = DefaultProcessCount;

    [ObservableProperty]
    private int _resourceTypeCount = DefaultResourceTypeCount;

    [ObservableProperty]
    private int _processCountInput = DefaultProcessCount;

    [ObservableProperty]
    private int _resourceTypeCountInput = DefaultResourceTypeCount;

    [ObservableProperty]
    private ObservableCollection<BankerProcessRow> _processes = new();

    [ObservableProperty]
    private ObservableCollection<ResourceVector> _availableResources = new();

    public BankerAlgorithmPageViewModel()
    {
        InitializeData();
    }

    public bool CanApplySize =>
        ProcessCountInput >= 1
        && ResourceTypeCountInput >= 1
        && (ProcessCountInput != ProcessCount || ResourceTypeCountInput != ResourceTypeCount);

    [RelayCommand(CanExecute = nameof(CanApplySize))]
    private void ApplySize()
    {
        if (ProcessCountInput < 1 || ResourceTypeCountInput < 1)
        {
            return;
        }

        ProcessCount = ProcessCountInput;
        ResourceTypeCount = ResourceTypeCountInput;
    }

    partial void OnProcessCountChanged(int value)
    {
        if (ProcessCountInput != value)
        {
            ProcessCountInput = value;
        }

        ApplySizeCommand.NotifyCanExecuteChanged();
        UpdateProcessCount();
    }

    partial void OnProcessCountInputChanged(int value)
    {
        ApplySizeCommand.NotifyCanExecuteChanged();
    }

    partial void OnResourceTypeCountChanged(int value)
    {
        if (ResourceTypeCountInput != value)
        {
            ResourceTypeCountInput = value;
        }

        ApplySizeCommand.NotifyCanExecuteChanged();
        UpdateResourceCount();
    }

    partial void OnResourceTypeCountInputChanged(int value)
    {
        ApplySizeCommand.NotifyCanExecuteChanged();
    }

    private void InitializeData()
    {
        Processes.Clear();
        for (var i = 0; i < ProcessCount; i++)
        {
            Processes.Add(new BankerProcessRow(i + 1, ResourceTypeCount));
        }

        AvailableResources.Clear();
        AvailableResources.Add(new ResourceVector(ResourceTypeCount));
    }

    private void UpdateProcessCount()
    {
        if (ProcessCount < 1)
        {
            return;
        }

        while (Processes.Count < ProcessCount)
        {
            Processes.Add(new BankerProcessRow(Processes.Count + 1, ResourceTypeCount));
        }

        while (Processes.Count > ProcessCount)
        {
            Processes.RemoveAt(Processes.Count - 1);
        }
    }

    private void UpdateResourceCount()
    {
        if (ResourceTypeCount < 1)
        {
            return;
        }

        foreach (var row in Processes)
        {
            row.ResizeResources(ResourceTypeCount);
        }

        if (AvailableResources.Count == 0)
        {
            AvailableResources.Add(new ResourceVector(ResourceTypeCount));
        }
        else
        {
            AvailableResources[0].Resize(ResourceTypeCount);
        }
    }
}
