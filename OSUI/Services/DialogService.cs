using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace OSUI.Services;

public class DialogService : IDialogService
{
    public async Task ShowDialog(string message)
    {
        var content = new TextBlock
        {
            Text = message,
            Margin = new Thickness(32, 24, 32, 24),
            FontSize = 16,
            TextWrapping = TextWrapping.Wrap,
            MaxWidth = 400
        };

        await DialogHost.Show(content, "DialogHost");
    }
}