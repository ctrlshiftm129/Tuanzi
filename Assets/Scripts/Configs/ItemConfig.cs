using UnityEditor;
using UnityEngine;

public class ItemConfig : ScriptableObject
{
    public Sprite icon;
    public ItemLevel level;
    public string itemName;
    public string itemDescription;
    public GameProperty itemProp;
    public int basePrice;
        
#if UNITY_EDITOR
    [MenuItem("工具/创建道具配置")]
    public static void CreateNewItemAsset()
    {
        var path = "Assets/Configs/Items/NewItem.asset";
        var newAsset = CreateInstance<ItemConfig>();
        AssetDatabase.CreateAsset(newAsset, path);
    }
#endif
}