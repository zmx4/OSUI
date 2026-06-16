using OSUI.Models;
using OSUI.ViewModels;

namespace OSUI.Services;

public static class PageReplacementEngine
{
    /// <summary>
    /// FIFO 先进先出算法
    /// </summary>
    public static List<StepRecord> RunFifo(int[] pages, int frameCount)
    {
        var records = new List<StepRecord>();
        var memory = new List<int>();
        var queue = new Queue<int>(); // 用于追踪进入顺序

        for (int i = 0; i < pages.Length; i++)
        {
            int page = pages[i];
            bool hit = memory.Contains(page);
            string note = "";

            if (!hit)
            {
                if (memory.Count >= frameCount)
                {
                    int victim = queue.Dequeue();
                    memory.Remove(victim);
                    note = $"缺页! 淘汰最早进入的页面: {victim}";
                }
                else
                {
                    note = "缺页! 内存未满，直接载入";
                }

                memory.Add(page);
                queue.Enqueue(page);
            }
            else
            {
                note = "命中!";
            }

            records.Add(new StepRecord
            {
                StepIndex = i,
                CurrentPage = page,
                MemoryState = new List<int>(memory),
                IsHit = hit,
                AlgorithmNote = note
            });
        }

        return records;
    }

    /// <summary>
    /// LRU 最近最少使用算法
    /// </summary>
    public static List<StepRecord> RunLru(int[] pages, int frameCount)
    {
        var records = new List<StepRecord>();
        var memory = new List<int>();

        for (int i = 0; i < pages.Length; i++)
        {
            int page = pages[i];
            bool hit = memory.Contains(page);
            string note = "";

            if (!hit)
            {
                if (memory.Count >= frameCount)
                {
                    // 找到最久未被访问的页面（在列表头部）
                    int victim = memory[0];
                    memory.RemoveAt(0);
                    note = $"缺页! 淘汰最久未使用的页面: {victim}";
                }
                else
                {
                    note = "缺页! 内存未满，直接载入";
                }

                memory.Add(page);
            }
            else
            {
                // 命中时，将该页面移到列表末尾（表示最近使用）
                memory.Remove(page);
                memory.Add(page);
                note = "命中! 更新使用时间";
            }

            records.Add(new StepRecord
            {
                StepIndex = i,
                CurrentPage = page,
                MemoryState = new List<int>(memory),
                IsHit = hit,
                AlgorithmNote = note
            });
        }

        return records;
    }

    /// <summary>
    /// OPT 最佳置换算法 (理论最优)
    /// </summary>
    public static List<StepRecord> RunOpt(int[] pages, int frameCount)
    {
        var records = new List<StepRecord>();
        var memory = new List<int>();

        for (int i = 0; i < pages.Length; i++)
        {
            int page = pages[i];
            bool hit = memory.Contains(page);
            string note = "";

            if (!hit)
            {
                if (memory.Count >= frameCount)
                {
                    // 寻找未来最晚被使用的页面
                    int victim = -1;
                    int farthestNextUse = -1;

                    foreach (var m in memory)
                    {
                        int nextUse = int.MaxValue;
                        for (int j = i + 1; j < pages.Length; j++)
                        {
                            if (pages[j] == m)
                            {
                                nextUse = j;
                                break;
                            }
                        }

                        if (nextUse > farthestNextUse)
                        {
                            farthestNextUse = nextUse;
                            victim = m;
                        }
                    }

                    memory.Remove(victim);
                    note = $"缺页! 淘汰未来最晚使用的页面: {(farthestNextUse == int.MaxValue ? "不再使用" : victim.ToString())}";
                }
                else
                {
                    note = "缺页! 内存未满，直接载入";
                }

                memory.Add(page);
            }
            else
            {
                note = "命中!";
            }

            records.Add(new StepRecord
            {
                StepIndex = i,
                CurrentPage = page,
                MemoryState = new List<int>(memory),
                IsHit = hit,
                AlgorithmNote = note
            });
        }

        return records;
    }
}