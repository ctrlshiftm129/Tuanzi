using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class UIManager : Manager
{
    public GameObject damageTextPrefab;
    
    public GameObject playerPropertyPanel;
    public GameObject levelPanel;
    public GameObject moneyPanel;
    public GameObject timePanel;
    
    private void Awake()
    {
        InitPoolRoot();
        Register2Locator();
    }

    public void ShowPlayerProperty(PlayerController player)
    {
        var panel = playerPropertyPanel.GetComponent<TextMeshProUGUI>();
        
        var str = new StringBuilder();
        str.Append($"攻击力: {player.WeaponDamage}\n");
        str.Append($"%伤害: {player.DamageRate}\n");
        str.Append($"%暴击率: {player.CriticalHitRate}\n");
        str.Append($"射速: {player.WeaponFireRate}s\n");
        str.Append($"范围: {player.WeaponRange}\n");
        str.Append($"移速: {player.Speed}\n");
        str.Append($"幸运: {player.Lucky}\n");

        panel.text = str.ToString();
    }

    public void ShowLevel(int level)
    {
        var panel = levelPanel.GetComponent<TextMeshProUGUI>();
        panel.text = $"Level {level}";
    }
    
    public void ShowMoney(int money)
    {
        var panel = moneyPanel.GetComponent<TextMeshProUGUI>();
        panel.text = $"金钱: {money}";
    }

    public void ShowTime(int seconds)
    {
        var panel = timePanel.GetComponent<TextMeshProUGUI>();
        var min = seconds / 60;
        seconds %= 60;
        panel.text = $"{min:00}:{seconds:00}";
    }

    public void ShowDamageAtPos(int damage, bool critical, Vector3 worldPos)
    {
        var damageTextGo = CreateDamageText(damage, critical, worldPos);
        StartCoroutine(RecycleDamageText(damageTextGo, 1));
    }

    private IEnumerator RecycleDamageText(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay); // 等待一段时间后回收
        RecycleDamageText(go);
    }

    #region UI Prefab Pool

    private readonly GameObjectPool m_gameObjectPool = new(1000);
    private GameObject m_damageTextRoot;
    
    private void InitPoolRoot()
    {
        m_damageTextRoot = new GameObject("DamageTextRoot");
        m_damageTextRoot.transform.SetParent(transform);
    }

    private GameObject CreateDamageText(int damage, bool critical, Vector3 worldPos)
    {
        var damageText = m_gameObjectPool.TryGetGameObject(damageTextPrefab);
        if (damageText == null)
        {
            damageText = Instantiate(damageTextPrefab);
        }
        
        var text = damageText.GetComponent<TextMeshPro>();
        text.text = damage.ToString();
        var color = critical ? Color.yellow : Color.white;
        text.color = color;
        damageText.transform.SetParent(m_damageTextRoot.transform);
        damageText.transform.position = worldPos;
        damageText.SetActive(true);
        return damageText;
    }

    private void RecycleDamageText(GameObject damageText)
    {
        damageText.SetActive(false);
        m_gameObjectPool.RecycleGameObject(damageTextPrefab, damageText);
    }

    #endregion
}
