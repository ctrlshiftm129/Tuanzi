using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class RandomUtils
{
    public static Vector3 RandomVector2Pos(float minX, float maxX, float minY, float maxY)
    {
        return new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);
    }

    public static Vector3 RandomVector2WithFixedX(float x, float minY, float maxY)
    {
        return new Vector3(x, Random.Range(minY, maxY), 0);
    }

    public static Vector3 RandomVector2WithFixedY(float y, float minX, float maxX)
    {
        return new Vector3(Random.Range(minX, maxX), y, 0);
    }
    
    /// <summary>
    /// 洗牌算法
    /// </summary>
    public static void Shuffle<T>(Span<T> data, int len)
    {
        len = Math.Min(len, data.Length);
        while (len > 1)
        {
            len--;
            var k = Random.Range(0, len);
            (data[k], data[len]) = (data[len], data[k]);
        }
    }
    
    public static void Shuffle<T>(List<T> data)
    {
        var len = data.Count;
        while (len > 1)
        {
            len--;
            var k = Random.Range(0, len);
            (data[k], data[len]) = (data[len], data[k]);
        }
    }
}