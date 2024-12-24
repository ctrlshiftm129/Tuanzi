using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ItemLevel
{
    L1,
    L2,
    L3,
    L4
}

public class EconomicManager : Manager
{
    public bool trick;
    
    [Space]
    public List<ItemConfig> itemConfigs = new();
    public GameProbabilityConfig probabilityConfig;
    
    [Space]
    public GameObject goldCoin;
    public GameObject bossChest;
    
    private ManagerLocator m_locator;
    private UIManager m_uiManager;
    private EnemyManager m_enemyManager;
    private readonly Dictionary<ItemLevel, List<ItemConfig>> m_level2Items = new();
    
    private int m_money;
    private bool m_hasTrick;
    private readonly HashSet<ItemConfig> m_itemConfigHashSetTemp = new();
    
    private void Awake()
    {
        InitTrophyRoot();
        InitItemConfigs();
        m_locator = ManagerLocator.Instance;
        Register2Locator();
    }

    private void Start()
    {
        m_uiManager = m_locator.Get<UIManager>();
        m_enemyManager = m_locator.Get<EnemyManager>();
        m_enemyManager.OnNormalEnemyDead += OnNormalEnemyDead;
        m_enemyManager.OnBossDead += OnBossDead;
        m_uiManager.ShowMoney(m_money);
    }

    private void OnDisable()
    {
        m_enemyManager.OnNormalEnemyDead -= OnNormalEnemyDead;
        m_enemyManager.OnBossDead -= OnBossDead;
    }
    
    private void OnNormalEnemyDead(Enemy enemy)
    {
        CreateNewGoldCoinToPlayer(enemy.gameObject.transform.position);
    }

    private void OnBossDead(Enemy boss)
    {
        CreateBossTreasure(boss.gameObject.transform.position, 20);
    }

    #region Trophy

    private readonly HashSet<GameObject> m_activeCoin = new();
    private readonly GameObjectPool m_gameObjectPool = new(1000);
    private readonly float m_coinSpeed = 6f;
    private GameObject m_coinRoot;
    
    // temp
    private readonly HashSet<GameObject> m_gameObjectTemp = new();

    private void InitTrophyRoot()
    {
        m_coinRoot = new GameObject("CoinRoot");
        m_coinRoot.transform.SetParent(transform);
    }

    private void CreateBossTreasure(Vector3 pos, int coinNum)
    {
        for (var i = 0; i < coinNum; ++i)
        {
            var coinPos = new Vector2(pos.x, pos.y) + Random.insideUnitCircle * 2f;
            CreateNewGoldCoinToPlayer(coinPos);
        }

        var chest = Instantiate(bossChest, transform);
        chest.transform.position = pos;
    }

    private void CreateNewGoldCoinToPlayer(Vector3 createPos)
    {
        var newCoin = m_gameObjectPool.TryGetGameObject(goldCoin);
        if (newCoin == null)
        {
            newCoin = Instantiate(goldCoin);
            newCoin.SetActive(false);
        }

        newCoin.transform.position = createPos;
        newCoin.transform.SetParent(m_coinRoot.transform);
        newCoin.SetActive(true);
        m_activeCoin.Add(newCoin);
    }

    public void UpdateActiveCoin(Vector3 playerPos)
    {
        var deleteCoins = m_gameObjectTemp;
        var delta = Time.deltaTime * m_coinSpeed;
        foreach (var coin in m_activeCoin)
        {
            var coinPos = coin.transform.position;
            if (Vector3.Distance(playerPos, coinPos) < delta)
            {
                deleteCoins.Add(coin);
                continue;
            }
            
            var dir = (playerPos - coin.transform.position).normalized;
            var move = dir * delta;
            coin.transform.position += move;
        }

        foreach (var deleteCoin in deleteCoins)
        {
            m_activeCoin.Remove(deleteCoin);
            deleteCoin.SetActive(false);
            m_gameObjectPool.RecycleGameObject(goldCoin, deleteCoin);
            ++m_money;
        }

        if (deleteCoins.Count > 0)
        {
            m_uiManager.ShowMoney(m_money);
        }
        deleteCoins.Clear();
    }

    #endregion

    #region Item
    
    private class ItemNumSet
    {
        public readonly int[] nums = new int[Enum.GetValues(typeof(ItemLevel)).Length];
    }

    private void InitItemConfigs()
    {
        m_level2Items.Clear();
        foreach (ItemLevel itemLevel in Enum.GetValues(typeof(ItemLevel)))
        {
            m_level2Items[itemLevel] = new List<ItemConfig>();
        }
        
        foreach (var itemConfig in itemConfigs)
        {
            m_level2Items[itemConfig.level].Add(itemConfig);
        }
    }

    private ItemNumSet RandomItemsLevel(int num, ItemNumSet set = null)
    {
        if (set == null)  set = new ItemNumSet();

        for (var t = 0; t < num; ++t)
        {
            var randNum = Random.Range(0, 100);
            var level = 0;
            var p = probabilityConfig.chest;
            for (var i = p.Length - 1; i > 0; --i)
            {
                var curLevel = p.Length - i + 1;
                var itemCount = m_level2Items[(ItemLevel)curLevel].Count;
                if (itemCount > 0 && randNum < p[i])
                {
                    level = curLevel;
                    break;
                }
            }
            ++set.nums[level];
        }

        return set;
    }
    
    private void GetRandomItemsByLevel(ItemLevel level, int num, List<ItemConfig> items)
    {
        var itemSet = m_level2Items[level];
        if (itemSet.Count < num)
        {
            throw new ArgumentException("没有那么多可以随机的道具");
        }
        for (var i = 0; i < num; ++i)
        {
            var rand = Random.Range(0, itemSet.Count);
            while (m_itemConfigHashSetTemp.Contains(itemSet[rand]))
            {
                rand = Random.Range(0, itemSet.Count);
            }

            m_itemConfigHashSetTemp.Add(itemSet[rand]);
            items.Add(itemSet[rand]);
        }
        m_itemConfigHashSetTemp.Clear();
    }

    #endregion

    #region Chest

    public void GetChestRandomItems(List<ItemConfig> items)
    {
        var result = new ItemNumSet();
        if (trick && !m_hasTrick)
        {
            result = RandomItemsLevel(2, result);
            if (result.nums[(int)ItemLevel.L4] == 0)
            {
                result.nums[(int)ItemLevel.L4] = 1;
            }
            else
            {
                ++result.nums[(int)ItemLevel.L1];
            }
            m_hasTrick = true;
        }
        else
        {
            result = RandomItemsLevel(3, result);
        }
        
        for (var level = 0; level < result.nums.Length; ++level)
        {
            var num = result.nums[level];
            if (num == 0) continue;
            var itemLevel = (ItemLevel)level;
            GetRandomItemsByLevel(itemLevel, num, items);
        }
            
        RandomUtils.Shuffle(items);
    }
    
    #endregion

    #region Shop

    // 为了方便演示游戏，先简单写
    
    public void OpenShopPanel(PlayerController playerController)
    {
        m_uiManager.SetPausePanelPropertyValue(playerController);
        m_uiManager.OpenShopPanel();
    }

    public void BuyItem1()
    {
        --m_money;
        m_uiManager.ShowMoney(m_money);
        m_locator.Get<GameManager>().player.GetComponent<PlayerAutoAttack>().sanshe = true;
        m_uiManager.HideShopSelection(0);
    }

    public void BuyItem2()
    {
        --m_money;
        m_uiManager.ShowMoney(m_money);
        m_locator.Get<GameManager>().player.GetComponent<PlayerAutoAttack>().judahua = true;
        m_uiManager.HideShopSelection(2);
    }

    public void BuyItem3()
    {
        --m_money;
        m_uiManager.ShowMoney(m_money);
        m_locator.Get<GameManager>().player.GetComponent<PlayerAutoAttack>().huimie = true;
        m_uiManager.HideShopSelection(3);
    }

    public void CloseShopPanel()
    {
        m_uiManager.CloseShopPanel();
    }

    #endregion
}
