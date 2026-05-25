using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OSUI.Models;

public sealed class ResourceVector : ObservableObject
{
    private int[] _values;

    public int Count => _values.Length;

    public ResourceVector(int count)
    {
        _values = new int[Math.Max(0, count)];
    }

    public int this[int index]
    {
        get => _values[index];
        set
        {
            if (_values[index] == value)
            {
                return;
            }

            _values[index] = value;
            OnPropertyChanged($"Item[{index}]");
            OnPropertyChanged("Item[]");
            ValueChanged?.Invoke(this, new ResourceValueChangedEventArgs(index, value));
        }
    }

    public event EventHandler<ResourceValueChangedEventArgs>? ValueChanged;

    public void Resize(int count)
    {
        if (count < 0)
        {
            count = 0;
        }

        if (count == _values.Length)
        {
            return;
        }

        var next = new int[count];
        Array.Copy(_values, next, Math.Min(count, _values.Length));
        _values = next;
        OnPropertyChanged(nameof(Count));
        OnPropertyChanged("Item[]");
        ValueChanged?.Invoke(this, new ResourceValueChangedEventArgs(-1, 0));
    }
}
