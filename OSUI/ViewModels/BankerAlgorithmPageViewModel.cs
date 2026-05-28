using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OSUI.Models;
using OSUI.Services;

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
    private ObservableCollection<BankerProcessRow> _processes = [];

    [ObservableProperty]
    private ObservableCollection<ResourceVector> _availableResources = [];

    [ObservableProperty]
    private string _availableResourcesInput = string.Empty;

    [ObservableProperty]
    private string _requestProcessIdInput = string.Empty;

    [ObservableProperty]
    private string _requestVectorInput = string.Empty;

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

    [RelayCommand]
    private void CheckSafety()
    {
        if (CheckSafetyInternal(AvailableResources[0], Processes, out var safeSequence))
        {
            var msg = LocalizationService.Instance.GetString("Banker.Message.Safe") ?? "系统处于安全状态。";
            msg += $"\n安全序列: {string.Join(" -> ", safeSequence.Select(id => $"P{id}"))}";
            MessageBox.Show(msg, LocalizationService.Instance.GetString("Banker.Title.Safe") ?? "安全", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            var msg = LocalizationService.Instance.GetString("Banker.Message.Unsafe") ?? "系统处于不安全状态！可能会发生死锁。";
            MessageBox.Show(msg, LocalizationService.Instance.GetString("Banker.Title.Unsafe") ?? "不安全", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private bool CheckSafetyInternal(ResourceVector available, System.Collections.Generic.IReadOnlyList<BankerProcessRow> processes, out System.Collections.Generic.List<int> safeSequence)
    {
        var n = processes.Count;
        var m = ResourceTypeCount;
        var work = new int[m];
        var finish = new bool[n];
        safeSequence = new System.Collections.Generic.List<int>();

        for (int i = 0; i < m; i++)
        {
            work[i] = available[i];
        }

        int count = 0;
        while (count < n)
        {
            bool found = false;
            for (int p = 0; p < n; p++)
            {
                if (finish[p]) continue;

                bool canAllocate = true;
                for (int j = 0; j < m; j++)
                {
                    if (processes[p].Need[j] > work[j])
                    {
                        canAllocate = false;
                        break;
                    }
                }

                if (canAllocate)
                {
                    for (int j = 0; j < m; j++)
                    {
                        work[j] += processes[p].Allocation[j];
                    }
                    safeSequence.Add(processes[p].Id);
                    finish[p] = true;
                    found = true;
                    count++;
                }
            }
            if (!found) break; // unsafe
        }
        return count == n;
    }

    [RelayCommand]
    private void SimulateRequest()
    {
        if (!int.TryParse(RequestProcessIdInput, out var pid))
        {
            MessageBox.Show(LocalizationService.Instance.GetString("Banker.Error.InvalidPid") ?? "无效的进程 ID。", LocalizationService.Instance.GetString("General.Error") ?? "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var process = Processes.FirstOrDefault(p => p.Id == pid);
        if (process == null)
        {
            MessageBox.Show((LocalizationService.Instance.GetString("Banker.Error.ProcessNotFound") ?? "未找到进程: P") + pid, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var parts = RequestVectorInput?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        if (parts.Length != ResourceTypeCount)
        {
            MessageBox.Show((LocalizationService.Instance.GetString("Banker.Error.VectorLength") ?? "请求向量必须包含资源数: ") + ResourceTypeCount, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var request = new int[ResourceTypeCount];
        for (int i = 0; i < ResourceTypeCount; i++)
        {
            if (!int.TryParse(parts[i], out var req) || req < 0)
            {
                MessageBox.Show(LocalizationService.Instance.GetString("Banker.Error.InvalidAmount") ?? "无效的请求数量。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            request[i] = req;
        }

        for (int i = 0; i < ResourceTypeCount; i++)
        {
            if (request[i] > process.Need[i])
            {
                MessageBox.Show(LocalizationService.Instance.GetString("Banker.Error.ExceedNeed") ?? $"请求超出资源 {i + 1} 的最大需求。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        for (int i = 0; i < ResourceTypeCount; i++)
        {
            if (request[i] > AvailableResources[0][i])
            {
                MessageBox.Show(LocalizationService.Instance.GetString("Banker.Warning.Wait") ?? $"请求超出资源 {i + 1} 的当前可用数量。进程必须等待。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        var availableClone = new ResourceVector(ResourceTypeCount);
        for(int i = 0; i < ResourceTypeCount; i++) availableClone[i] = AvailableResources[0][i] - request[i];

        var processesClone = new System.Collections.Generic.List<BankerProcessRow>();
        foreach (var p in Processes)
        {
            var pClone = new BankerProcessRow(p.Id, ResourceTypeCount);
            for (int i = 0; i < ResourceTypeCount; i++)
            {
                pClone.Allocation[i] = p.Allocation[i];
                pClone.Max[i] = p.Max[i];
            }
            processesClone.Add(pClone);
        }

        var pReq = processesClone.First(p => p.Id == pid);
        for (int i = 0; i < ResourceTypeCount; i++)
        {
            pReq.Allocation[i] += request[i];
        }

        if (CheckSafetyInternal(availableClone, processesClone, out var safeSequence))
        {
            for (int i = 0; i < ResourceTypeCount; i++)
            {
                AvailableResources[0][i] -= request[i];
                process.Allocation[i] += request[i];
            }
            UpdateAvailableResourcesInputFromVector();
            
            var msg = LocalizationService.Instance.GetString("Banker.Message.RequestGranted") ?? "请求被允许！\n安全序列: ";
            msg += string.Join(" -> ", safeSequence.Select(id => $"P{id}"));
            MessageBox.Show(msg, LocalizationService.Instance.GetString("Banker.Title.RequestGranted") ?? "成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show(LocalizationService.Instance.GetString("Banker.Message.RequestDenied") ?? "请求被拒绝：分配将导致不安全状态。", LocalizationService.Instance.GetString("Banker.Title.RequestDenied") ?? "不安全", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
