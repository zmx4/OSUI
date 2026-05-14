using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OSUI.Models;
using OSUI.Services;
using System.Linq;

namespace OSUI.ViewModels;

public partial class SchedulerPageViewModel : PageViewModel
{
    [ObservableProperty]
    private ObservableCollection<Process> _processes = new();

    public string[] Algorithms { get; } = { "FCFS", "SJFS" };

    [ObservableProperty]
    private string _selectedAlgorithm = "FCFS";

    [RelayCommand]
    private void AddProcess()
    {
        int nextId = Processes.Count > 0 ? Processes.Max(p => p.Id) + 1 : 1;
        Processes.Add(new Process { Id = nextId, ArrivalTime = 0, BurstTime = 1 });
    }

    [RelayCommand]
    private void Run()
    {
        if (Processes.Count == 0) return;

        IScheduler scheduler = SelectedAlgorithm == "SJFS" 
            ? new SJFScheduler() 
            : new FCFSScheduler();

        // 重新调度
        var scheduled = scheduler.Schedule(Processes.ToList());

        // 更新数据，为了在界面上可以看到结果
        Processes.Clear();
        foreach (var p in scheduled)
        {
            Processes.Add(p);
        }
    }
}