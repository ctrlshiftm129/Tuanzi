using System;
using System.Text;
using TMPro;
using UnityEngine;

public class UIManager : Manager
{
    public GameObject playerPropertyPanel;
    public GameObject levelPanel;
    public GameObject moneyPanel;
    
    private void Awake()
    {
        Register2Locator();
    }

    private void Start()
    {
       
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
}
