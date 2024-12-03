using UnityEngine;

public static class RandomUtils
{
    public static Vector3 RandomVector2Pos(float min, float max)
    {
        return new Vector3(Random.Range(min, max), Random.Range(min, max), 0);
    }
}