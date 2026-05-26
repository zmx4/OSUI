using System;
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

    [ObservableProperty]
    private string _availableResourcesInput = string.Empty;

    private bool _suppressAvailableResourcesSync;

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

    [RelayCommand]
    private void UseDemoData()
    {
        const int demoProcessCount = 5;
        const int demoResourceCount = 3;

        ResourceTypeCount = demoResourceCount;
        ProcessCount = demoProcessCount;

        var allocation = new[,]
        {
            { 0, 1, 0 },
            { 2, 0, 0 },
            { 3, 0, 2 },
            { 2, 1, 1 },
            { 0, 0, 2 }
        };

        var max = new[,]
        {
            { 7, 5, 3 },
            { 3, 2, 2 },
            { 9, 0, 2 },
            { 2, 2, 2 },
            { 4, 3, 3 }
        };

        var available = new[] { 3, 3, 2 };

        for (var i = 0; i < demoProcessCount; i++)
        {
            var row = Processes[i];
            for (var j = 0; j < demoResourceCount; j++)
            {
                row.Allocation[j] = allocation[i, j];
                row.Max[j] = max[i, j];
            }
        }

        if (AvailableResources.Count == 0)
        {
            AvailableResources.Add(new ResourceVector(demoResourceCount));
        }
        else
        {
            AvailableResources[0].Resize(demoResourceCount);
        }

        for (var i = 0; i < demoResourceCount; i++)
        {
            AvailableResources[0][i] = available[i];
        }
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
        UpdateAvailableResourcesInputFromVector();
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

        UpdateAvailableResourcesInputFromVector();
    }

    partial void OnAvailableResourcesInputChanged(string value)
    {
        if (_suppressAvailableResourcesSync)
        {
            return;
        }

        SyncAvailableResourcesFromInput(value);
    }

    private void SyncAvailableResourcesFromInput(string value)
    {
        if (ResourceTypeCount < 1)
        {
            return;
        }

        if (AvailableResources.Count == 0)
        {
            AvailableResources.Add(new ResourceVector(ResourceTypeCount));
        }

        var vector = AvailableResources[0];
        var parts = value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        for (var i = 0; i < ResourceTypeCount; i++)
        {
            var parsed = 0;
            if (i < parts.Length && int.TryParse(parts[i], out var valuePart))
            {
                parsed = Math.Max(0, valuePart);
            }

            vector[i] = parsed;
        }
    }

    private void UpdateAvailableResourcesInputFromVector()
    {
        if (AvailableResources.Count == 0 || ResourceTypeCount < 1)
        {
            return;
        }

        var vector = AvailableResources[0];
        var values = new string[ResourceTypeCount];
        for (var i = 0; i < ResourceTypeCount; i++)
        {
            values[i] = vector[i].ToString();
        }

        _suppressAvailableResourcesSync = true;
        AvailableResourcesInput = string.Join(" ", values);
        _suppressAvailableResourcesSync = false;
    }
}
