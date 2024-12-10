using System;
using System.Collections;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : Manager
{
    public GameObject damageTextPrefab;
    
    [Space]
    public GameObject playerPropertyPanel;
    
    [Space]
    public GameObject levelPanel;
    public GameObject levelTimePanel;
    public GameObject moneyPanel;
    
    private void Awake()
    {
        InitPoolRoot();
        Register2Locator();
    }

    #region Show UI Context

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
        panel.text = $"关卡 {level}";
    }
    
    public void ShowMoney(int money)
    {
        var panel = moneyPanel.GetComponent<TextMeshProUGUI>();
        panel.text = money.ToString();
    }

    private static readonly int Flicker = Animator.StringToHash("Flicker");
    public void ShowLevelTime(int seconds)
    {
        var color = Color.white;
        var animator = levelTimePanel.GetComponent<Animator>();
        if (seconds <= 10)
        {
            color = Color.red;
            animator.SetBool(Flicker, true);
        }
        else
        {
            animator.SetBool(Flicker, false);
        }
        
        var panel = levelTimePanel.GetComponent<TextMeshProUGUI>();
        var min = seconds / 60;
        seconds %= 60;
        panel.text = $"{min:00}:{seconds:00}";
        panel.color = color;
    }

    public void ShowCountdownText(string text)
    {
        var panel = levelTimePanel.GetComponent<TextMeshProUGUI>();
        panel.text = text;
        panel.color = Color.red;
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

    #endregion

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
