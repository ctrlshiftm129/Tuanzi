using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIManager : Manager
{
    private static readonly int Next = Animator.StringToHash("Next");
    
    public GameObject damageTextPrefab;

    [Space] 
    public GameObject pauseGamePanel;
    public GameObject propertyPanel;
    
    [Space]
    public GameObject levelPanel;
    public GameObject levelTimePanel;
    public GameObject moneyPanel;
    public Slider bossHpBar;
    public GameObject bossNameLabel;
    
    [Space]
    public GameObject chestPanel;
    public GameObject shopPanel;
    public Color levelColor1;
    public Color levelColor2;
    public Color levelColor3;
    public Color levelColor4;
    
    private void Awake()
    {
        InitPoolRoot();
        Register2Locator();
    }

    #region Main Game UI

    public void ShowPlayerProperty(PlayerController player)
    {
        // var panel = playerPropertyPanel.GetComponent<TextMeshProUGUI>();
        //
        // var str = new StringBuilder();
        // str.Append($"攻击力: {player.AttAck}\n");
        // str.Append($"%伤害: {player.DamageRate}\n");
        // str.Append($"%暴击率: {player.CriticalHitRate}\n");
        // str.Append($"射速: {player.FireRate}s\n");
        // str.Append($"范围: {player.WeaponRange}\n");
        // str.Append($"移速: {player.Speed}\n");
        // str.Append($"幸运: {player.Lucky}\n");
        //
        // panel.text = str.ToString();
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
        if (seconds <= 10) color = Color.red;
        if (seconds <= 10 && seconds > 0)
        {
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

    public void ShowBossHp(string bossName, float animTime)
    {
        bossHpBar.value = 0;
        var nameText = bossNameLabel.GetComponent<TextMeshProUGUI>();
        nameText.text = bossName;
        bossHpBar.gameObject.SetActive(true);
        StartCoroutine(ShowBossHpAnim(animTime));
    }

    private IEnumerator ShowBossHpAnim(float time)
    {
        var timer = 0f;
        var delta = 1f / time;
        var hpValue = 0f;
        while (timer < time)
        {
            var deltaTime = Time.deltaTime;
            timer += deltaTime;
            hpValue += delta * deltaTime;
            bossHpBar.value = hpValue;
            yield return null;
        }
        bossNameLabel.SetActive(true);
    }

    public void UpdateBossHp(float ratio)
    {
        bossHpBar.value = ratio;
    }

    public void HideBossInfo()
    {
        bossNameLabel.SetActive(false);
        bossHpBar.gameObject.SetActive(false);
    }

    #endregion

    #region Chest UI

    private string GetIntPropStr(int number)
    {
        if (number > 0)
        {
            return $"<color=green>+{number}</color>";
        }
        return $"<color=red>-{number}</color>";
    }
    
    private string GetFloatPropStr(float number, bool revert = false)
    {
        var flagA = "+";
        var flagB = "-";
        if (revert) (flagA, flagB) = (flagB, flagA);
        if (number > 0)
        {
            return $"<color=green>{flagA}{number:F}</color>";
        }
        return $"<color=red>{flagB}{number:F}</color>";
    }
    
    private string GetItemPropText(GameProperty property)
    {
        var str = new StringBuilder();
        if (property.attack != 0) str.Append($"{GetIntPropStr(property.attack)} 攻击力\n");
        if (property.damageRate != 0) str.Append($"{GetIntPropStr(property.damageRate)} %伤害\n");
        if (property.criticalHitRate != 0) str.Append($"{GetIntPropStr(property.criticalHitRate)} %暴击率\n");
        if (property.fireRateChange != 0) str.Append($"{GetIntPropStr(property.fireRateChange)} 射速\n");
        if (property.range != 0) str.Append($"{GetIntPropStr(property.range)} 范围\n");
        if (property.speedChange != 0) str.Append($"{GetIntPropStr(property.speedChange)} %移速\n");
        if (property.lucky != 0) str.Append($"{GetIntPropStr(property.lucky)} 幸运\n");
        return str.ToString();
    }

    public void OpenChestPanel()
    {
        ResetChestBackGroundColor();
        chestPanel.SetActive(true);
    }

    public void SetChestPanelAnimNext()
    {
        var animator = chestPanel.GetComponent<Animator>();
        animator.SetTrigger(Next);
    }

    public void ResetChestBackGroundColor()
    {
        SetChestBackGroundColor(new Color(0.53f, 0.53f, 0.53f), 1);
    }
    
    public void SetChestBackGroundColor(ItemLevel maxLevel)
    {
        switch (maxLevel)
        {
            case ItemLevel.L2:
                SetChestBackGroundColor(levelColor2, 1);
                break;
            case ItemLevel.L3:
                SetChestBackGroundColor(levelColor3, 3);
                break;
            case ItemLevel.L4:
                SetChestBackGroundColor(levelColor4, 5);
                break;
        }
    }

    private void SetChestBackGroundColor(Color color, float speed)
    {
        var background = chestPanel.transform.Find("Background");
        var image = background.GetComponent<Image>();
        image.color = color;
        var bar = background.GetComponent<BackgroundScrollBar>();
        bar.speed = speed;
    }
    
    public void BuildChestSelection(ItemConfig item, int index, UnityAction action)
    {
        var selectionRoot = chestPanel.transform.Find("选项");
        var selection = selectionRoot.GetChild(index);
        var image = selection.GetComponent<Image>();
        var outline = selection.GetComponent<Outline>();
        var itemName = selection.Find("Info/Title/TitleName").GetComponent<TextMeshProUGUI>();
        switch (item.level)
        {
            case ItemLevel.L1:
                outline.effectColor = levelColor1;
                itemName.color = levelColor1;
                break;
            case ItemLevel.L2:
                outline.effectColor = levelColor2;
                itemName.color = levelColor2;
                break;
            case ItemLevel.L3:
                outline.effectColor = levelColor3;
                itemName.color = levelColor3;
                break;
            case ItemLevel.L4:
                outline.effectColor = levelColor4;
                itemName.color = levelColor4;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        var icon = selection.Find("Info/Title/TitleIcon");
        icon.GetComponent<Image>().sprite = item.icon;
        itemName.text = item.itemName;
        var description = selection.Find("Info/Description");
        description.GetComponent<TextMeshProUGUI>().text = item.itemDescription;
        var property = selection.Find("Info/Property");
        property.GetComponent<TextMeshProUGUI>().text = GetItemPropText(item.itemProp);
        
        var selectionButton = selection.Find("Select");
        var button = selectionButton.GetComponent<Button>();
        button.onClick.AddListener(action);
    }

    public void SetChestSelectionActive(bool value)
    {
        var selectionRoot = chestPanel.transform.Find("选项");
        for (var i = 0; i < selectionRoot.childCount; ++i)
        {
            var child = selectionRoot.GetChild(i);
            child.gameObject.SetActive(value);
        }
    }

    public void CloseChestPanel()
    {
        chestPanel.SetActive(false);
        SetChestSelectionActive(false);
    }

    #endregion

    #region Shop UI

    public void OpenShopPanel()
    {
        for (var i = 1; i < 5; ++i)
        {
            var content = shopPanel.transform.Find($"选项/Selection{i}/Content");
            if (!content.gameObject.activeSelf)
            {
                content.gameObject.SetActive(true);
            }
        }
        shopPanel.SetActive(true);
        propertyPanel.SetActive(true);
    }

    public void HideShopSelection(int index)
    {
        var content = shopPanel.transform.Find($"选项/Selection{index + 1}/Content");
        content.gameObject.SetActive(false);
    }

    public void CloseShopPanel()
    {
        shopPanel.SetActive(false);
        propertyPanel.SetActive(false);
    }

    #endregion
    
    #region System UI

    private enum PropertyState
    {
        Normal,
        Good,
        Bad
    }

    private PropertyState GetPropertyState(int value)
    {
        if (value == 0)
        {
            return PropertyState.Normal;
        }

        return value > 0 ? PropertyState.Good : PropertyState.Bad;
    }
    
    private PropertyState GetPropertyState(float value)
    {
        if (value.Equals(0))
        {
            return PropertyState.Normal;
        }
        return value > 0 ? PropertyState.Good : PropertyState.Bad;
    }
    
    private string GetPausePanelValueStr(string value, PropertyState state)
    {
        return state switch
        {
            PropertyState.Normal => value,
            PropertyState.Good => $"<color=green>{value}</color>",
            PropertyState.Bad => $"<color=red>{value}</color>",
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }

    public void SetPausePanelPropertyValue(PlayerController player)
    {
        var panel = propertyPanel.transform.Find("属性值").GetComponent<TextMeshProUGUI>();
        
        var str = new StringBuilder();
        str.Append($"{GetPausePanelValueStr(player.AttAck.ToString(), GetPropertyState(player.additionalProp.attack))}\n");
        str.Append($"{GetPausePanelValueStr($"{player.DamageRate}", GetPropertyState(player.additionalProp.damageRate))}%\n");
        str.Append($"{GetPausePanelValueStr($"{player.CriticalHitRate}", GetPropertyState(player.additionalProp.criticalHitRate))}%\n");
        var fireRateChangeState = GetPropertyState(player.additionalProp.fireRateChange);
        str.Append($"{GetPausePanelValueStr($"{player.FireRateChange}", fireRateChangeState)}%\n");
        str.Append($"{GetPausePanelValueStr($"{player.WeaponFireRate:F2}", fireRateChangeState)}s\n");
        str.Append($"{GetPausePanelValueStr($"{player.WeaponRange}", GetPropertyState(player.additionalProp.range))}\n");
        str.Append($"{GetPausePanelValueStr($"{player.PlayerSpeed}", GetPropertyState(player.additionalProp.speedChange))}\n");
        str.Append($"{GetPausePanelValueStr($"{player.Lucky}", GetPropertyState(player.additionalProp.lucky))}\n");
        panel.text = str.ToString();
    }
    
    public void SetPausePanelState(bool value)
    {
        pauseGamePanel.SetActive(value);
        propertyPanel.SetActive(value);
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
        var random = Random.insideUnitCircle * 0.3f;
        worldPos = new Vector3(worldPos.x + random.x, worldPos.y, worldPos.z + random.y);
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
