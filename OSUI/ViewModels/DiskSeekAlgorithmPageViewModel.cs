using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OSUI.Data;
using OSUI.Extensions;
using OSUI.Models;

namespace OSUI.ViewModels;

public partial class DiskSeekAlgorithmPageViewModel : PageViewModel
{
    private DiskSeekVector _diskSeekVector;
    
    [ObservableProperty]
    private int _requestCount;

    [ObservableProperty] private int _startIndex;
    [ObservableProperty] private string _requestsInput;
    
    public string[] AvailableAlgorithms { get; } = ["SSTF", "SCAN", "C-SCAN"];
    
    public DiskSeekAlgorithmPageViewModel()
    {
        PageNames = ApplicationPageNames.DiskSeekAlgorithmPage;
        RequestsInput = "55,58,39,18,90,160,150,38,184";
        StartIndex = 100;
        RequestCount = 9;
        _diskSeekVector = new DiskSeekVector(0, 0, []);
    }
    
    [ObservableProperty] 
    private string _selectedAlgorithm = "SSTF";

    [ObservableProperty]
    private DiskSeekResult? _currentResult;

    [RelayCommand]
    private async Task StartSimulation()
    {
        _diskSeekVector = new DiskSeekVector(StartIndex, RequestCount, []);
        _diskSeekVector.ParseRequests(RequestsInput);
        
        switch (SelectedAlgorithm)
        {
            case "SSTF":
                CurrentResult = _diskSeekVector.SSFT();
                break;
            case "SCAN":
                CurrentResult = _diskSeekVector.SCAN();
                break;
            // C-SCAN requires implementation, fallback to SCAN
            case "C-SCAN":
                CurrentResult = _diskSeekVector.SCAN();
                break;
        }
    }
}