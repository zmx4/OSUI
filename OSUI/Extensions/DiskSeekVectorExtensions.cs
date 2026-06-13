using OSUI.Models;

namespace OSUI.Extensions;

static class DiskSeekVectorExtensions
{
    #region Helper Methods

    public static void ParseRequests(this DiskSeekVector diskSeekVector, string requestsInput)
    {
        string[] requests = requestsInput.Split(',');
        var requestVector = requests.Select(x => int.Parse(x.Trim())).ToArray();
        diskSeekVector.RequestCount = requestVector.Length;
        diskSeekVector.RequestVector = requestVector;
    }

    #endregion

    #region 算法实现
    /// <summary>
    /// SSFT算法,使用贪心思想,每次在待处理的请求队列中，寻找距离当前磁头位置最近的请求进行服务，并更新磁头位置，直到所有请求都被处理完毕。
    /// </summary>
    /// <param name="diskSeekVector">寻道向量</param>
    /// <returns></returns>
    public static DiskSeekResult SSFT(this DiskSeekVector diskSeekVector)
    {
        int[] seekSequence = new int[diskSeekVector.RequestCount + 1];
        seekSequence[0] = diskSeekVector.StartIndex;
        bool[] visited = new bool[diskSeekVector.RequestCount];
        int currentIndex = diskSeekVector.StartIndex;
        int count = 1;
        int totalLength = 0;
        while (count <= diskSeekVector.RequestCount)
        {
            int minDistance = int.MaxValue;
            int nextIndex = -1;
            for (int i = 0; i < diskSeekVector.RequestCount; i++)
            {
                if (!visited[i])
                {
                    int distance = Math.Abs(diskSeekVector.RequestVector[i] - currentIndex);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nextIndex = i;
                    }
                }
            }

            visited[nextIndex] = true;
            seekSequence[count] = diskSeekVector.RequestVector[nextIndex];
            totalLength += minDistance;
            currentIndex = diskSeekVector.RequestVector[nextIndex];
            count++;
        }

        return new DiskSeekResult(seekSequence, totalLength, (double)totalLength / diskSeekVector.RequestCount);
    }
    /// <summary>
    /// SCAN算法:磁头沿某一固定方向移动，依次服务途经的请求，直到到达该方向的磁盘边界（或没有更多请求），然后反向移动继续服务。
    /// </summary>
    /// <param name="diskSeekVector">寻道向量</param>
    /// <param name="direction">方向</param>
    /// <returns></returns>
    public static DiskSeekResult SCAN(this DiskSeekVector diskSeekVector,int direction = 1)
    {
        int totalSeekLength = 0;
        int[] seekSequence = new int[diskSeekVector.RequestCount + 1];
        seekSequence[0] = diskSeekVector.StartIndex;
        int currentIndex = diskSeekVector.StartIndex;
        int count = 1;
        bool[] visited = new bool[diskSeekVector.RequestCount];
        diskSeekVector.RequestVector.Sort();
        int splitIndex = diskSeekVector.RequestVector.GetLowerBound(currentIndex);
        if (direction == 1)
        {
            for(int i = splitIndex; i < diskSeekVector.RequestCount; i++)
            {
                visited[i] = true;
                seekSequence[count] = diskSeekVector.RequestVector[i];
                totalSeekLength += Math.Abs(diskSeekVector.RequestVector[i] - currentIndex);
                currentIndex = diskSeekVector.RequestVector[i];
                count++;
            }

            for (int i = splitIndex - 1; i >= 0; i--)
            {
                visited[i] = true;
                seekSequence[count] = diskSeekVector.RequestVector[i];
                totalSeekLength += Math.Abs(diskSeekVector.RequestVector[i] - currentIndex);
                currentIndex = diskSeekVector.RequestVector[i];
                count++;
            }
        }
        else
        {
            for (int i = splitIndex; i >= 0; i--)
            {
                visited[i] = true;
                seekSequence[count] = diskSeekVector.RequestVector[i];
                totalSeekLength += Math.Abs(diskSeekVector.RequestVector[i] - currentIndex);
                currentIndex = diskSeekVector.RequestVector[i];
                count++;
            }
            for(int i = splitIndex+1;i <diskSeekVector.RequestCount;i++)
            {
                visited[i] = true;
                seekSequence[count] = diskSeekVector.RequestVector[i];
                totalSeekLength += Math.Abs(diskSeekVector.RequestVector[i] - currentIndex);
                currentIndex = diskSeekVector.RequestVector[i];
                count++;
            }
        }
        return new DiskSeekResult(seekSequence, totalSeekLength, (double)totalSeekLength / diskSeekVector.RequestCount);
    }
    

    #endregion
    
}