using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OSUI.Converters;

public sealed class TimeInputConverter : IMultiValueConverter
{
    private static readonly string[] TimeFormats = { "h\\:m", "h\\:mm", "hh\\:m", "hh\\:mm" };

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 2 || values[0] == null || values[0] == DependencyProperty.UnsetValue)
        {
            return string.Empty;
        }

        if (!TryGetMinutes(values[0], out var minutes))
        {
            return string.Empty;
        }

        var mode = values[1]?.ToString() ?? string.Empty;
        if (string.Equals(mode, "HH:MM", StringComparison.OrdinalIgnoreCase))
        {
            minutes = Math.Max(0, minutes);
            var hours = minutes / 60;
            var mins = minutes % 60;
            return string.Format(CultureInfo.InvariantCulture, "{0:D2}:{1:D2}", hours, mins);
        }

        return minutes.ToString(CultureInfo.InvariantCulture);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        var text = (value as string)?.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            return new object[] { Binding.DoNothing, Binding.DoNothing };
        }

        if (TryParseMinutes(text, out var minutes))
        {
            return new object[] { minutes, Binding.DoNothing };
        }

        return new object[] { Binding.DoNothing, Binding.DoNothing };
    }

    private static bool TryGetMinutes(object value, out int minutes)
    {
        if (value is int intValue)
        {
            minutes = intValue;
            return true;
        }

        if (value is long longValue)
        {
            minutes = (int)longValue;
            return true;
        }

        if (value is double doubleValue)
        {
            minutes = (int)doubleValue;
            return true;
        }

        minutes = 0;
        return false;
    }

    private static bool TryParseMinutes(string text, out int minutes)
    {
        if (text.Contains(":", StringComparison.Ordinal))
        {
            if (TimeSpan.TryParseExact(text, TimeFormats, CultureInfo.InvariantCulture, out var timeSpan)
                || TimeSpan.TryParse(text, CultureInfo.InvariantCulture, out timeSpan))
            {
                minutes = (int)timeSpan.TotalMinutes;
                if (minutes < 0)
                {
                    minutes = 0;
                }
                return true;
            }
        }

        if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out minutes))
        {
            if (minutes < 0)
            {
                minutes = 0;
            }
            return true;
        }

        minutes = 0;
        return false;
    }
}
