using UnityEngine;

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
}