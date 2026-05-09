namespace OSUI.Models;

public class Process
{
    public int Id { get; set; }           // 进程ID
    public int ArrivalTime { get; set; }  // 到达时间
    public int BurstTime { get; set; }    // 执行时间（服务时间）
    public int StartTime { get; set; }    // 开始执行时间
    public int CompletionTime { get; set; } // 完成时间
    public int TurnaroundTime => CompletionTime - ArrivalTime;     // 周转时间
    public double WeightedTurnaroundTime => (double)TurnaroundTime / BurstTime; // 带权周转时间
}

