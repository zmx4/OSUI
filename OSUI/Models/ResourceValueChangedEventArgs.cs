using System;

namespace OSUI.Models;

public sealed class ResourceValueChangedEventArgs : EventArgs
{
    public int Index { get; }
    public int Value { get; }

    public ResourceValueChangedEventArgs(int index, int value)
    {
        Index = index;
        Value = value;
    }
}
