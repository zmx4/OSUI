namespace OSUI.Models;

public class DiskSeekResult(int[] accessOrder, int totalSeekLength, double avgRequestLength)
{
    public int[] AccessOrder{get; set;} = accessOrder;
    public int TotalSeekLength = totalSeekLength;
    public double AvgRequestLength = avgRequestLength;
}