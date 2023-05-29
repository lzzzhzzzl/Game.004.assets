using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通用工具
/// </summary>
public class CommandMethod : MonoBehaviour
{
    public static bool[,] DilateForVector2Array(bool[,] originalArray, int length)
    {
        bool[,] currentArray = new bool[originalArray.GetLength(0), originalArray.GetLength(1)];
        for (int x = 0; x < currentArray.GetLength(0); x++)
            for (int y = 0; y < currentArray.GetLength(1); y++)
            {
                for (int i = -length; i <= length; i++)
                    for (int j = -length; j <= length; j++)
                    {
                        if (x + i >= 0 && x + i < currentArray.GetLength(0) && y + j >= 0 && y + j < currentArray.GetLength(1))
                        {
                            if (originalArray[x + i, y + j])
                            {
                                currentArray[x, y] = true;
                            }
                        }
                    }
            }

        return currentArray;
    }
    public static float[,] DilateForVector2Array(float[,] originalArray, int length)
    {
        float[,] currentArray = new float[originalArray.GetLength(0), originalArray.GetLength(1)];
        for (int x = 0; x < currentArray.GetLength(0); x++)
            for (int y = 0; y < currentArray.GetLength(1); y++)
            {
                for (int i = -length; i <= length; i++)
                    for (int j = -length; j <= length; j++)
                    {
                        if (x + i >= 0 && x + i < currentArray.GetLength(0) && y + j >= 0 && y + j < currentArray.GetLength(1))
                        {
                            if (originalArray[x + i, y + j] == 1)
                            {
                                currentArray[x, y] = 1;
                            }
                        }
                    }
            }

        return currentArray;
    }
    public static float[,] ErosionForVector2Array(float[,] originalArray, int length)
    {
        float[,] currentArray = new float[originalArray.GetLength(0), originalArray.GetLength(1)];
        for (int x = 0; x < originalArray.GetLength(0); x++)
            for (int y = 0; y < originalArray.GetLength(1); y++)
                currentArray[x, y] = originalArray[x, y];

        float[,] mapArray = new float[originalArray.GetLength(0) + 2, originalArray.GetLength(1) + 2];
        for (int x = 0; x < originalArray.GetLength(0); x++)
            for (int y = 0; y < originalArray.GetLength(1); y++)
                mapArray[x + 1, y + 1] = originalArray[x, y];

        for (int x = 1; x < currentArray.GetLength(0) + 1; x++)
            for (int y = 1; y < currentArray.GetLength(1) + 1; y++)
            {
                for (int i = -length; i <= length; i++)
                    for (int j = -length; j <= length; j++)
                    {
                        if (x + i >= 0 && x + i < currentArray.GetLength(0) + 2 && y + j >= 0 && y + j < currentArray.GetLength(1) + 2)
                        {
                            if (mapArray[x + i, y + j] == 0)
                            {
                                currentArray[x - 1, y - 1] = 0;
                            }
                        }
                    }
            }

        return currentArray;
    }
    public static bool[,] ErosionForVector2Array(bool[,] originalArray, int length)
    {
        bool[,] currentArray = new bool[originalArray.GetLength(0), originalArray.GetLength(1)];
        for (int x = 0; x < originalArray.GetLength(0); x++)
            for (int y = 0; y < originalArray.GetLength(1); y++)
                currentArray[x, y] = originalArray[x, y];

        bool[,] mapArray = new bool[originalArray.GetLength(0) + 2, originalArray.GetLength(1) + 2];
        for (int x = 0; x < originalArray.GetLength(0); x++)
            for (int y = 0; y < originalArray.GetLength(1); y++)
                mapArray[x + 1, y + 1] = originalArray[x, y];

        for (int x = 1; x < currentArray.GetLength(0) + 1; x++)
            for (int y = 1; y < currentArray.GetLength(1) + 1; y++)
            {
                for (int i = -length; i <= length; i++)
                    for (int j = -length; j <= length; j++)
                    {
                        if (x + i >= 0 && x + i < currentArray.GetLength(0) + 2 && y + j >= 0 && y + j < currentArray.GetLength(1) + 2)
                        {
                            if (!mapArray[x + i, y + j])
                            {
                                currentArray[x - 1, y - 1] = false;
                            }
                        }
                    }
            }

        return currentArray;
    }
    public static Dictionary<int, List<int>> Dijkstra(float[,] weightList, int startIndex)
    {
        Dictionary<int, List<int>> pathList = new Dictionary<int, List<int>>();
        bool[] visited = new bool[weightList.GetLength(0)];
        float[] shortest = new float[weightList.GetLength(0)];

        for (int num = 0; num < weightList.GetLength(0); num++)
        {
            List<int> list = new List<int>();
            list.Add(startIndex);
            list.Add(num);
            pathList.Add(num, list);
        }

        visited[startIndex] = true;
        shortest[startIndex] = 0;

        for (int i = 0; i < weightList.GetLength(0) - 1; i++)
        {
            int shortIndex = -1;
            float shortWeight = float.MaxValue;

            for (int index = 0; index < weightList.GetLength(0); index++)
            {
                if (!visited[index] && weightList[startIndex, index] < shortWeight)
                {
                    shortIndex = index;
                    shortWeight = weightList[startIndex, index];
                }
            }

            shortest[shortIndex] = shortWeight;
            visited[shortIndex] = true;

            for (int index = 0; index < weightList.GetLength(0); index++)
            {
                if (!visited[index] && weightList[startIndex, shortIndex] + weightList[shortIndex, index] < weightList[startIndex, index])
                {
                    pathList[index] = new List<int>(pathList[shortIndex]);
                    pathList[index].Add(index);
                }
            }
        }

        return pathList;
    }

    public static float[,] CombinatorialFloatArray(float[,] fristArray, float[,] secondArray)
    {
        int xLength = fristArray.GetLength(0) < secondArray.GetLength(0) ? fristArray.GetLength(0) : secondArray.GetLength(0);
        int yLength = fristArray.GetLength(1) < secondArray.GetLength(1) ? fristArray.GetLength(1) : secondArray.GetLength(1);

        float[,] outArray = new float[xLength, yLength];
        for (int x = 0; x < xLength; x++)
            for (int y = 0; y < yLength; y++)
            {
                if (fristArray[x, y] == 1)
                    outArray[x, y] = 1;
                if (secondArray[x, y] == 1)
                    outArray[x, y] = 1;
            }
        return outArray;
    }
    public static Vector3 GetRelativeDirection(Vector3 fromPosition, Vector3 targetPosition)
    {
        Vector3 position = fromPosition - targetPosition;
        float xAbs = Mathf.Abs(position.x);
        float yAbs = Mathf.Abs(position.y);

        if (xAbs >= yAbs)
        {
            if (position.x >= 0)
                return Vector3.right;
            else if (position.x < 0)
                return Vector3.left;
        }
        else
        {
            if (position.y >= 0)
                return Vector3.up;
            else if (position.y < 0)
                return Vector3.down;
        }
        return Vector3.zero;
    }
}

