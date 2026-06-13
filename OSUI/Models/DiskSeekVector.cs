namespace OSUI.Models;

public class DiskSeekVector(int startIndex,int requestCount,int[] requestVector)
{
    public int StartIndex { get; set; } = startIndex;
    public int RequestCount { get; set; } =  requestCount;
    public int[] RequestVector{ get; set;} = requestVector;
}

