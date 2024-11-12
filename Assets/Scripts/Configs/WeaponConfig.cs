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

    [MenuItem("工具/测试")]
    public static void AAA()
    {
        Vector3 a = new Vector3(10, 10, 0);
        Vector3 b = new Vector3(12, 14, 0);
        Vector3 c = new Vector3(0, 1, 0);

        float angle = Vector3.Angle(Vector3.right, b - a);
        Debug.Log(angle);
        float angle2 = Vector3.Angle(Vector3.right, c);
        Debug.Log(angle2);

        var gameObject = Selection.activeGameObject;
        gameObject.transform.Rotate(Vector3.forward, angle2);
    }

    [MenuItem("工具/测试2")]
    public static void AAAA()
    {
        var gameObject = Selection.activeGameObject;
        gameObject.transform.Translate(Vector3.right);
    }
#endif
}
