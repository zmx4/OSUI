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
    
    public string[] AvailableAlgorithms { get; } = { "FCFS", "SSTF", "SCAN", "C-SCAN" };
    
    public DiskSeekAlgorithmPageViewModel()
    {
        PageNames = ApplicationPageNames.DiskSeekAlgorithmPage;
        _diskSeekVector = new DiskSeekVector(0, 0, Array.Empty<int>());
    }
    
    [ObservableProperty] 
    private string _selectedAlgorithm = "FCFS";

    [RelayCommand]
    private async Task StartSimulation()
    {
        _diskSeekVector = new DiskSeekVector(StartIndex, RequestCount, []);
        _diskSeekVector.ParseRequests(RequestsInput);
        
        // Simulate disk seek algorithm based on _selectedAlgorithm and _diskSeekVector
        // This is where you would implement the logic to perform the disk seek algorithm
        // and update the UI with the results.
        await Task.Delay(1000); // Simulate some processing time
    }
}