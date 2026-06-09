using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OSUI.Models;
using OSUI.Services;
using System.Linq;
using OSUI.Data;

namespace OSUI.ViewModels;

public partial class SchedulerPageViewModel : PageViewModel
{
    public sealed record TimeInputModeOption(string Mode, string DisplayName);

    private const string MinutesMode = "MINUTES";
    private const string HourMinuteMode = "HH:MM";

    public SchedulerPageViewModel()
    {
        PageNames = ApplicationPageNames.SchedulerPage;
    }
    
    [ObservableProperty]
    private ObservableCollection<Process> _processes = new();

    public string[] Algorithms { get; } = { "FCFS", "SJFS" };

    public TimeInputModeOption[] TimeInputModes { get; } =
    [
        new(MinutesMode, LocalizationService.Instance.GetString("Scheduler.TimeMode.Minutes")),
        new(HourMinuteMode, LocalizationService.Instance.GetString("Scheduler.TimeMode.Clock"))
    ];

    [ObservableProperty]
    private string _selectedAlgorithm = "FCFS";

    [ObservableProperty]
    private string _selectedTimeInputMode = MinutesMode;

    [ObservableProperty]
    private double _totalTurnaroundTime;

    [ObservableProperty]
    private double _totalWeightedTurnaroundTime;

    [ObservableProperty]
    private double _averageTurnaroundTime;

    [ObservableProperty]
    private double _averageWeightedTurnaroundTime;

    [RelayCommand]
    private void AddProcess()
    {
        int nextId = Processes.Count > 0 ? Processes.Max(p => p.Id) + 1 : 1;
        Processes.Add(new Process { Id = nextId, ArrivalTime = 0, BurstTime = 1 });
    }

    [RelayCommand]
    private void LoadExample()
    {
        SelectedTimeInputMode = HourMinuteMode;
        Processes.Clear();
        Processes.Add(new Process { Id = 1, ArrivalTime = 8 * 60, BurstTime = 120 });
        Processes.Add(new Process { Id = 2, ArrivalTime = 8 * 60 + 50, BurstTime = 50 });
        Processes.Add(new Process { Id = 3, ArrivalTime = 9 * 60, BurstTime = 10 });
        Processes.Add(new Process { Id = 4, ArrivalTime = 9 * 60 + 50, BurstTime = 20 });
    }
    [RelayCommand]
    private void LoadSecondExample()
    {
        SelectedTimeInputMode = HourMinuteMode;
        Processes.Clear();
        Processes.Add(new Process { Id = 1, ArrivalTime = 0, BurstTime = 4 });
        Processes.Add(new Process { Id = 2, ArrivalTime = 1, BurstTime = 3 });
        Processes.Add(new Process { Id = 3, ArrivalTime = 2, BurstTime = 5 });
        Processes.Add(new Process { Id = 4, ArrivalTime = 3, BurstTime = 2 });
        Processes.Add(new Process { Id = 5, ArrivalTime = 4, BurstTime = 4 });
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

        TotalTurnaroundTime = System.Math.Round(Processes.Sum(p => (double)p.TurnaroundTime), 2);
        TotalWeightedTurnaroundTime = System.Math.Round(Processes.Sum(p => p.WeightedTurnaroundTime), 2);
        AverageTurnaroundTime = System.Math.Round(Processes.Average(p => (double)p.TurnaroundTime), 2);
        AverageWeightedTurnaroundTime = System.Math.Round(Processes.Average(p => p.WeightedTurnaroundTime), 2);
    }
}
