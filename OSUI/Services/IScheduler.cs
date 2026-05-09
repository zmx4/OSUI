using OSUI.Models;

namespace OSUI.Services;

public interface IScheduler
{
    private static List<Process> CloneProcesses(List<Process> origin)
    {
        return origin.Select(p => new Models.Process
        {
            Id = p.Id,
            ArrivalTime = p.ArrivalTime,
            BurstTime = p.BurstTime
        }).ToList();
    }
    public List<Process> Schedule(List<Process> processes);
}