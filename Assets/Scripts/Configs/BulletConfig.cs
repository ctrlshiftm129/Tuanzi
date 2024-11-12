using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BulletConfig : ScriptableObject
{
    public GameObject bulletPrefab;
    public float speed;
    public bool penetrate;

#if UNITY_EDITOR
    [MenuItem("工具/创建子弹配置")]
    public static void CreateNewBulletAsset()
    {
        var path = "Assets/Configs/Bullets/NewBullet.asset";
        var newAsset = CreateInstance<BulletConfig>();
        AssetDatabase.CreateAsset(newAsset, path);
    }
#endif
}
