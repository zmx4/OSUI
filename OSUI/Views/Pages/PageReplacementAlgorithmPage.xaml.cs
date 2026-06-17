using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using OSUI.Models;
using OSUI.Services;

namespace OSUI.Views.Pages;

public class StepViewModel
{
    public string StepLabel { get; set; }
    public string PageNum { get; set; }
    public List<string> Frames { get; set; }
    public string Note { get; set; }
    public SolidColorBrush BgColor { get; set; }
    public SolidColorBrush FgColor { get; set; }
}

public partial class PageReplacementAlgorithmPage : UserControl
{
    private List<StepRecord> _records;
    private int _currentStep = -1;
    private readonly DispatcherTimer _timer;
    private bool _isAutoPlaying = false;

    public PageReplacementAlgorithmPage()
    {
        InitializeComponent();
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(800) };
        _timer.Tick += Timer_Tick;
    }

    private void BtnStart_Click(object sender, RoutedEventArgs e)
    {
        StopAutoPlay();

        try
        {
            // 解析输入
            int[] pages = TxtPages.Text.Split([',', ' ', ';'], StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.Parse(s.Trim())).ToArray();
            int frames = int.Parse(TxtFrames.Text);

            if (frames <= 0 || pages.Length == 0) throw new Exception("输入无效");

            // 执行算法
            string algo = ((System.Windows.Controls.ComboBoxItem)CmbAlgorithm.SelectedItem).Content.ToString();
            _records = algo switch
            {
                "LRU" => PageReplacementEngine.RunLru(pages, frames),
                "OPT" => PageReplacementEngine.RunOpt(pages, frames),
                _ => PageReplacementEngine.RunFifo(pages, frames)
            };

            _currentStep = -1;
            IcSteps.ItemsSource = null;

            // 显示所有步骤（或者你可以改为只显示到当前步骤）
            UpdateVisualization();
            TxtStats.Text =
                $"总页数: {pages.Length} | 缺页次数: {_records.Count(r => !r.IsHit)} | 缺页率: {(_records.Count(r => !r.IsHit) * 100.0 / pages.Length):F1}%";

            // 自动跳到第一步
            if (_records.Any()) MoveToStep(0);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"错误: {ex.Message}", "输入解析失败", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateVisualization()
    {
        if (_records == null) return;

        var vms = _records.Select((r, idx) => new StepViewModel
        {
            StepLabel = $"T{idx + 1}",
            PageNum = r.CurrentPage.ToString(),
            Frames = r.MemoryState.Select(f => f.ToString()).ToList(),
            Note = r.AlgorithmNote,
            BgColor = r.IsHit
                ? new SolidColorBrush(Color.FromRgb(232, 245, 233))
                : // 绿色背景-命中
                new SolidColorBrush(Color.FromRgb(255, 235, 238)), // 红色背景-缺页
            FgColor = r.IsHit
                ? new SolidColorBrush(Color.FromRgb(46, 125, 50))
                : new SolidColorBrush(Color.FromRgb(198, 40, 40))
        }).ToList();

        IcSteps.ItemsSource = vms;
    }

    private void MoveToStep(int step)
    {
        if (step < 0 || step >= _records.Count) return;
        _currentStep = step;

        // 高亮当前步骤（简单实现：滚动到对应位置）
        // 在实际项目中，可以通过修改VM的边框颜色来实现高亮
        TxtStatus.Text = $"当前步骤: {_currentStep + 1} / {_records.Count} | " +
                         $"访问页面: {_records[_currentStep].CurrentPage} | " +
                         $"{_records[_currentStep].AlgorithmNote}";
    }

    private void BtnNext_Click(object sender, RoutedEventArgs e) => MoveToStep(_currentStep + 1);
    private void BtnPrev_Click(object sender, RoutedEventArgs e) => MoveToStep(_currentStep - 1);

    private void BtnAutoPlay_Click(object sender, RoutedEventArgs e)
    {
        if (_isAutoPlaying)
        {
            StopAutoPlay();
            return;
        }

        if (_records == null)
        {
            BtnStart_Click(sender, e);
            return;
        }

        _isAutoPlaying = true;
        BtnAutoPlay.Content = "⏸ 暂停";
        if (_currentStep >= _records.Count - 1) _currentStep = -1;
        _timer.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        if (_currentStep >= _records.Count - 1)
        {
            StopAutoPlay();
            return;
        }

        MoveToStep(_currentStep + 1);
    }

    private void StopAutoPlay()
    {
        _timer.Stop();
        _isAutoPlaying = false;
        BtnAutoPlay.Content = "▶ 自动播放";
    }
}