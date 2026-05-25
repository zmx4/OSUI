using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OSUI.Models;

public sealed class BankerProcessRow : ObservableObject
{
    public int Id { get; }
    public ResourceVector Allocation { get; }
    public ResourceVector Max { get; }
    public ResourceVector Need { get; }

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
    }
}
