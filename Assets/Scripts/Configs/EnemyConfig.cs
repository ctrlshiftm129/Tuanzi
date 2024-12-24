using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyConfig : ScriptableObject
{
    public string enemyName;
    public GameObject enemyPrefab;
    public int hp;
    public float speed;
    public EnemyBehaviour behaviour;
    
#if UNITY_EDITOR
    [MenuItem("工具/创建敌人配置")]
    public static void CreateNewBulletAsset()
    {
        var path = "Assets/Configs/Enemies/NewEnemy.asset";
        var newAsset = CreateInstance<EnemyConfig>();
        AssetDatabase.CreateAsset(newAsset, path);
    }
#endif
}
