
using System.Collections.Generic;
using System.Linq;
using OSUI.Models;

namespace OSUI.Services;

public class SJFScheduler : IScheduler
{
    public List<Process> Schedule(List<Process> processes)
    {
        // 拷贝避免影响原数据
        var remaining = new List<Process>(processes);
        var completed = new List<Process>();   // 已完成进程
        int currentTime = 0;

        while (remaining.Count > 0)
        {
            // 找出所有已经到达且尚未完成的进程
            var ready = remaining.Where(p => p.ArrivalTime <= currentTime).ToList();

            if (ready.Count == 0)
            {
                // 没有就绪进程，将时间推进到下一个最早到达的进程
                currentTime = remaining.Min(p => p.ArrivalTime);
                continue;
            }

            // 在就绪队列中选择执行时间最短的进程
            var nextProcess = ready.OrderBy(p => p.BurstTime)
                .ThenBy(p => p.ArrivalTime)  // 若执行时间相同，先到达优先
                .First();

            // 执行该进程
            nextProcess.StartTime = currentTime;
            nextProcess.CompletionTime = currentTime + nextProcess.BurstTime;
            currentTime = nextProcess.CompletionTime;

            // 将完成的进程移出 remaining，加入 completed
            remaining.Remove(nextProcess);
            completed.Add(nextProcess);
        }

        // 按完成顺序返回（若需要按到达排序，可自行修改）
        return completed;
    }
}