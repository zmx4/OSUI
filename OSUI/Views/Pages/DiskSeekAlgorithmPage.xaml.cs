using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using HandyControl.Tools.Extension;
using OSUI.ViewModels;
using OSUI.Models;

namespace OSUI.Views.Pages;

public partial class DiskSeekAlgorithmPage : UserControl
{
    public DiskSeekAlgorithmPage()
    {
        InitializeComponent();
        this.DataContextChanged += OnDataContextChanged;
        
        // 绑定初始化时可能已经存在的 DataContext
        if (this.DataContext is DiskSeekAlgorithmPageViewModel vm)
        {
            vm.PropertyChanged += ViewModel_PropertyChanged;
        }
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is DiskSeekAlgorithmPageViewModel oldVm)
        {
            oldVm.PropertyChanged -= ViewModel_PropertyChanged;
        }
        if (e.NewValue is DiskSeekAlgorithmPageViewModel newVm)
        {
            newVm.PropertyChanged += ViewModel_PropertyChanged;
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DiskSeekAlgorithmPageViewModel.CurrentResult))
        {
            DrawChart();
        }
    }

    private void ChartCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        DrawChart();
    }

    private void DrawChart()
    {
        ChartCanvas.Children.Clear();

        if (DataContext is not DiskSeekAlgorithmPageViewModel vm || vm.CurrentResult == null || vm.CurrentResult.AccessOrder == null)
            return;

        var sequence = vm.CurrentResult.AccessOrder;
        if (sequence.Length == 0) return;

        double canvasWidth = ChartCanvas.ActualWidth;
        double canvasHeight = ChartCanvas.ActualHeight;

        if (canvasWidth <= 0 || canvasHeight <= 0) return;

        int minVal = 0; // Or sequence.Min() if you prefer dynamic
        int maxVal = sequence.Max();
        maxVal = Math.Max(maxVal, 200); // Set a minimum upper bound for typical disk tracks

        double paddingLeft = 50; 
        double paddingRight = 30;
        double paddingTop = 40;
        double paddingBottom = 30;
        
        double usableWidth = canvasWidth - paddingLeft - paddingRight;
        double usableHeight = canvasHeight - paddingTop - paddingBottom;
        
        if (usableWidth <= 0 || usableHeight <= 0) return;

        // Draw Axis
        Line axisLine = new Line
        {
            X1 = paddingLeft,
            Y1 = paddingTop,
            X2 = paddingLeft + usableWidth,
            Y2 = paddingTop,
            Stroke = Brushes.Black,
            StrokeThickness = 1
        };
        ChartCanvas.Children.Add(axisLine);

        // Algorithm label on axis
        TextBlock algLabel = new TextBlock
        {
            Text = vm.SelectedAlgorithm,
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            Foreground = Brushes.Black
        };
        algLabel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        Canvas.SetLeft(algLabel, paddingLeft - algLabel.DesiredSize.Width - 5);
        Canvas.SetTop(algLabel, paddingTop - 8);
        ChartCanvas.Children.Add(algLabel);

        // Draw Sequence Points and Lines
        double yStep = usableHeight / (sequence.Length > 1 ? sequence.Length - 1 : 1);
        Point[] points = new Point[sequence.Length];

        for (int i = 0; i < sequence.Length; i++)
        {
            double x = paddingLeft + (sequence[i] - minVal) / (double)(maxVal - minVal) * usableWidth;
            double y = paddingTop + i * yStep + 20; // +20 so it starts a bit below the axis
            points[i] = new Point(x, y);

            // Ticks on Axis
            Line tick = new Line
            {
                X1 = x,
                Y1 = paddingTop - 8,
                X2 = x,
                Y2 = paddingTop,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            ChartCanvas.Children.Add(tick);

            // Labels on Axis
            TextBlock label = new TextBlock
            {
                Text = sequence[i].ToString(),
                FontSize = 12,
                Foreground = Brushes.Black
            };
            
            // Adjust position slightly to center it
            label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            // Ensure labels don't get clipped or overlap if they are very close
            Canvas.SetLeft(label, x - label.DesiredSize.Width / 2);
            Canvas.SetTop(label, paddingTop - 22);
            ChartCanvas.Children.Add(label);
        }

        // Draw Lines between nodes and calculate nodes
        for (int i = 0; i < sequence.Length - 1; i++)
        {
            Line pathLine = new Line
            {
                X1 = points[i].X,
                Y1 = points[i].Y,
                X2 = points[i + 1].X,
                Y2 = points[i + 1].Y,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                StrokeDashArray = [3, 3] // Dashed line
            };
            ChartCanvas.Children.Add(pathLine);

            // Draw Arrow
            DrawArrow(points[i], points[i + 1], ChartCanvas);
        }

        // Draw Nodes over lines
        for (int i = 0; i < sequence.Length; i++)
        {
            Ellipse node = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = i == 0 ? Brushes.White : Brushes.Gray,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            Canvas.SetLeft(node, points[i].X - 4);
            Canvas.SetTop(node, points[i].Y - 4);
            // Z-index to sure it stays top of the line
            Panel.SetZIndex(node, 10);
            ChartCanvas.Children.Add(node);
        }
    }

    private void DrawArrow(Point p1, Point p2, Canvas canvas)
    {
        double dx = p2.X - p1.X;
        double dy = p2.Y - p1.Y;
        double length = Math.Sqrt(dx * dx + dy * dy);
        
        if (length < 20) return; // Don't draw arrow if points are too close

        double arrowLength = 8;
        double arrowWidth = 5;

        // Position the arrow somewhat before the end point
        double ratio = (length - 10) / length;
        double ax = p1.X + dx * ratio;
        double ay = p1.Y + dy * ratio;

        // Normalize direction
        double dirX = dx / length;
        double dirY = dy / length;

        // Perpendicular vector
        double perpX = -dirY;
        double perpY = dirX;

        var pMid = new Point(ax, ay);
        var pBase1 = new Point(ax - dirX * arrowLength + perpX * arrowWidth, ay - dirY * arrowLength + perpY * arrowWidth);
        var pBase2 = new Point(ax - dirX * arrowLength - perpX * arrowWidth, ay - dirY * arrowLength - perpY * arrowWidth);

        var arrow = new Polygon
        {
            Points = [pMid, pBase1, pBase2],
            Fill = Brushes.Black
        };

        canvas.Children.Add(arrow);
    }
}