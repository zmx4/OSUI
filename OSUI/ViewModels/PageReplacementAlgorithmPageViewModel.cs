using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OSUI.Data;
using OSUI.Models;
using OSUI.Services;

namespace OSUI.ViewModels;

public partial class PageReplacementAlgorithmPageViewModel : PageViewModel
{
    private List<StepRecord> _records = [];

    [ObservableProperty]
    private string _pageSequenceInput = "7,0,1,2,0,3,0,4,2,3,0,3,2,1,2,0,1,7,0,1";

    [ObservableProperty]
    private string _frameCountInput = "3";

    [ObservableProperty]
    private string[] _availableAlgorithms = ["FIFO", "LRU", "OPT"];

    [ObservableProperty]
    private string _selectedAlgorithm = "FIFO";

    [ObservableProperty]
    private ObservableCollection<PageReplacementStepViewModel> _steps = [];

    [ObservableProperty]
    private int _currentStepIndex = -1;

    [ObservableProperty]
    private string _statusText = string.Empty;

    [ObservableProperty]
    private string _statsText = string.Empty;

    [ObservableProperty]
    private bool _isAutoPlaying;

    [ObservableProperty]
    private string _autoPlayButtonText;

    private readonly DispatcherTimer _timer;

    public PageReplacementAlgorithmPageViewModel()
    {
        PageNames = ApplicationPageNames.PageReplacementAlgorithmPage;
        AutoPlayButtonText = LocalizationService.Instance.GetString("PageReplacement.Button.AutoPlay");

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(800) };
        _timer.Tick += Timer_Tick;
    }

    // ── 命令 ─────────────────────────────────────────────────

    [RelayCommand]
    private void StartDemo()
    {
        StopAutoPlay();

        try
        {
            var pages = PageSequenceInput
                .Split([',', ' ', ';'], StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.Parse(s.Trim()))
                .ToArray();

            if (!int.TryParse(FrameCountInput, out var frameCount) || frameCount <= 0 || pages.Length == 0)
                throw new Exception(LocalizationService.Instance.GetString("PageReplacement.Error.InvalidInput"));

            _records = SelectedAlgorithm switch
            {
                "LRU" => PageReplacementEngine.RunLru(pages, frameCount),
                "OPT" => PageReplacementEngine.RunOpt(pages, frameCount),
                _ => PageReplacementEngine.RunFifo(pages, frameCount)
            };

            BuildStepViewModels(pages.Length);
            UpdateStats(pages.Length);

            CurrentStepIndex = -1;
            if (_records.Count > 0) MoveToStep(0);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                string.Format(
                    LocalizationService.Instance.GetString("PageReplacement.Error.Title") + ": {0}",
                    ex.Message),
                LocalizationService.Instance.GetString("PageReplacement.Error.Title"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void NextStep()
    {
        if (_records.Count == 0) return;
        MoveToStep(CurrentStepIndex + 1);
    }

    [RelayCommand]
    private void PreviousStep()
    {
        if (_records.Count == 0) return;
        MoveToStep(CurrentStepIndex - 1);
    }

    [RelayCommand]
    private void ToggleAutoPlay()
    {
        if (IsAutoPlaying)
        {
            StopAutoPlay();
            return;
        }

        if (_records.Count == 0)
        {
            StartDemo();
            if (_records.Count == 0) return;
        }

        IsAutoPlaying = true;
        AutoPlayButtonText = LocalizationService.Instance.GetString("PageReplacement.Button.Pause");
        if (CurrentStepIndex >= _records.Count - 1) CurrentStepIndex = -1;
        _timer.Start();
    }

    // ── 内部方法 ─────────────────────────────────────────────

    private void BuildStepViewModels(int totalPages)
    {
        Steps.Clear();
        for (int i = 0; i < _records.Count; i++)
        {
            var r = _records[i];
            Steps.Add(new PageReplacementStepViewModel
            {
                StepIndex = i,
                StepLabel = $"T{i + 1}",
                PageNum = r.CurrentPage.ToString(),
                Frames = new ObservableCollection<string>(r.MemoryState.Select(f => f.ToString())),
                Note = r.AlgorithmNote,
                IsHit = r.IsHit,
                IsCurrent = false
            });
        }
    }

    private void UpdateStats(int totalPages)
    {
        var pageFaults = _records.Count(r => !r.IsHit);
        var faultRate = pageFaults * 100.0 / totalPages;
        StatsText = string.Format(
            LocalizationService.Instance.GetString("PageReplacement.Stats.TotalPages") + " | " +
            LocalizationService.Instance.GetString("PageReplacement.Stats.PageFaults") + " | " +
            LocalizationService.Instance.GetString("PageReplacement.Stats.PageFaultRate"),
            totalPages, pageFaults, faultRate);
    }

    private void MoveToStep(int step)
    {
        if (step < 0 || step >= _records.Count) return;

        // 清除上一步高亮
        if (CurrentStepIndex >= 0 && CurrentStepIndex < Steps.Count)
            Steps[CurrentStepIndex].IsCurrent = false;

        CurrentStepIndex = step;
        Steps[step].IsCurrent = true;

        StatusText = string.Format(
            LocalizationService.Instance.GetString("PageReplacement.Status.CurrentStep") + " | " +
            LocalizationService.Instance.GetString("PageReplacement.Status.AccessPage") + " | " +
            "{2} | {3}",
            step + 1, _records.Count, _records[step].CurrentPage, _records[step].AlgorithmNote);
    }

    private void StopAutoPlay()
    {
        _timer.Stop();
        IsAutoPlaying = false;
        AutoPlayButtonText = LocalizationService.Instance.GetString("PageReplacement.Button.AutoPlay");
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (CurrentStepIndex >= _records.Count - 1)
        {
            StopAutoPlay();
            return;
        }

        MoveToStep(CurrentStepIndex + 1);
    }
}
