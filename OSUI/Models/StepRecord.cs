namespace OSUI.Models;

public class StepRecord
{
    public int StepIndex { get; set; }
    public int CurrentPage { get; set; }
    public List<int> MemoryState { get; set; } // 当前内存中的页面
    public bool IsHit { get; set; } // 是否命中
    public string AlgorithmNote { get; set; } // 算法备注（如：替换了最久未使用的页面）
}