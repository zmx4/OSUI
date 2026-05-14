using OSUI.Models;

namespace OSUI.Services;

public class FCFSScheduler : IScheduler
{
    public List<Process> Schedule(List<Process> processes)
    {
        var sortedProcesses = processes.OrderBy(p => p.ArrivalTime).ToList();
        int currentTime = 0;
        foreach (var process in sortedProcesses)
        {
            if (currentTime < process.ArrivalTime)
            {
                currentTime = process.ArrivalTime;
            }
            process.StartTime = currentTime;
            process.CompletionTime = currentTime + process.BurstTime;
            currentTime += process.BurstTime;
        }
        return sortedProcesses;
    }
}