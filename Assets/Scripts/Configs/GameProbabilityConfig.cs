using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameProbabilityConfig : ScriptableObject
{
    public int[] chest = {
        20, 10, 5
    };
        
        
#if UNITY_EDITOR
    [MenuItem("工具/创建概率配置文件")]
    public static void CreateNewBulletAsset()
    {
        var path = "Assets/Configs/GameProbabilityConfig.asset";
        var newAsset = CreateInstance<GameProbabilityConfig>();
        AssetDatabase.CreateAsset(newAsset, path);
    }
#endif
}