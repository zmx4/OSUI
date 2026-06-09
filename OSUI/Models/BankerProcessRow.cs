using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OSUI.Models;

public sealed class BankerProcessRow : ObservableObject
{
    public int Id { get; }
    public ResourceVector Allocation { get; }
    public ResourceVector Max { get; }
    public ResourceVector Need { get; }

    public string AllocationText
    {
        get => FormatVector(Allocation);
        set => UpdateVectorFromText(Allocation, value);
    }

    public string MaxText
    {
        get => FormatVector(Max);
        set => UpdateVectorFromText(Max, value);
    }

    public string NeedText => FormatVector(Need);

    public BankerProcessRow(int id, int resourceCount)
    {
        Id = id;
        Allocation = new ResourceVector(resourceCount);
        Max = new ResourceVector(resourceCount);
        Need = new ResourceVector(resourceCount);
        Allocation.ValueChanged += OnResourceChanged;
        Max.ValueChanged += OnResourceChanged;
        RecalculateNeed();
    }

    public void ResizeResources(int resourceCount)
    {
        Allocation.ValueChanged -= OnResourceChanged;
        Max.ValueChanged -= OnResourceChanged;

        Allocation.Resize(resourceCount);
        Max.Resize(resourceCount);
        Need.Resize(resourceCount);

        Allocation.ValueChanged += OnResourceChanged;
        Max.ValueChanged += OnResourceChanged;
        RecalculateNeed();
    }

    private void OnResourceChanged(object? sender, ResourceValueChangedEventArgs e)
    {
        RecalculateNeed();
    }

    private void RecalculateNeed()
    {
        var count = Math.Min(Need.Count, Math.Min(Max.Count, Allocation.Count));
        for (var i = 0; i < count; i++)
        {
            Need[i] = Max[i] - Allocation[i];
        }

        NotifyVectorTextChanged();
    }

    private void NotifyVectorTextChanged()
    {
        OnPropertyChanged(nameof(AllocationText));
        OnPropertyChanged(nameof(MaxText));
        OnPropertyChanged(nameof(NeedText));
    }

    private static string FormatVector(ResourceVector vector)
    {
        if (vector.Count == 0)
        {
            return string.Empty;
        }

        var values = new string[vector.Count];
        for (var i = 0; i < vector.Count; i++)
        {
            values[i] = vector[i].ToString(CultureInfo.InvariantCulture);
        }

        return string.Join(",", values);
    }

    private static bool TryParseVector(string? text, out int[] values)
    {
        values = Array.Empty<int>();
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var trimmed = text.Trim();
        if (trimmed.StartsWith("[", StringComparison.Ordinal) && trimmed.EndsWith("]", StringComparison.Ordinal))
        {
            trimmed = trimmed[1..^1];
        }

        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return false;
        }

        var tokens = trimmed.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length == 0)
        {
            return false;
        }

        var parsed = new int[tokens.Length];
        for (var i = 0; i < tokens.Length; i++)
        {
            if (!int.TryParse(tokens[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            {
                return false;
            }

            parsed[i] = value;
        }

        values = parsed;
        return true;
    }

    private void UpdateVectorFromText(ResourceVector vector, string? text)
    {
        if (!TryParseVector(text, out var values))
        {
            return;
        }

        var count = vector.Count;
        for (var i = 0; i < count; i++)
        {
            var nextValue = i < values.Length ? values[i] : 0;
            if (vector[i] != nextValue)
            {
                vector[i] = nextValue;
            }
        }
    }
}
