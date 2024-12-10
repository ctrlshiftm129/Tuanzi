using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WeaponConfig : ScriptableObject
{
    public string weaponName;
    public GameObject weaponPrefab;
    public string description;
    public GameProperty weaponProp;
    public BulletConfig bulletConfig;

#if UNITY_EDITOR
    [MenuItem("工具/创建武器配置")]
    public static void CreateNewWeaponAsset()
    {
        var path = "Assets/Configs/Weapons/NewWeapon.asset";
        var newAsset = CreateInstance<WeaponConfig>();
        AssetDatabase.CreateAsset(newAsset, path);
    }
#endif
}
