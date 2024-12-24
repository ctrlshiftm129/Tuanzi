using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : Manager
{
    private static readonly int Dead = Animator.StringToHash("Dead");
    
    public List<EnemyConfig> enemyConfigs;
    public GameObject warningPrefab;
    public event Action<Enemy> OnNormalEnemyDead;
    public event Action<Enemy> OnBossDead;

    private Dictionary<string, EnemyConfig> m_enemyConfigs;
    
    private readonly Dictionary<GameObject, Enemy> m_activeGo2Enemy = new();
    private readonly HashSet<Enemy> m_activeEnemies = new();
    private readonly List<Enemy> m_enemyUpdateTemp = new();
    private Enemy m_boss;

    private int m_delayCount;

    private void Awake()
    {
        InitPoolRoot();
        Register2Locator();
    }

    #region EnemyInfo

    public EnemyConfig GetEnemyConfigById(int id)
    {
        return enemyConfigs[id];
    }

    #endregion

    #region Create

    public void AddBossDelay(int enemyId, Vector3 worldPos, float delay = 4)
    {
        var warningGo = CreateWarningGameObject(worldPos);
        warningGo.transform.localScale = new Vector3(2, 2, 2);
        ++m_delayCount;
        StartCoroutine(AddEnemyDelayCore(warningGo, enemyId, worldPos, delay, true));
    }

    public void AddEnemyDelay(int enemyId, Vector3 worldPos, float delay = 1)
    {
        var warningGo = CreateWarningGameObject(worldPos);
        warningGo.transform.localScale = Vector3.one;
        ++m_delayCount;
        StartCoroutine(AddEnemyDelayCore(warningGo, enemyId, worldPos, delay, false));
    }

    private IEnumerator AddEnemyDelayCore(GameObject warningGo, int enemyId, Vector3 worldPos, float delay, bool isBoss)
    {
        yield return new WaitForSeconds(delay); // 等待一段时间
        RecycleWarningGameObject(warningGo);
        var enemy = AddEnemy(enemyId, worldPos);
        if (isBoss) m_boss = enemy;
        --m_delayCount;
    }
    
    private Enemy AddEnemy(int enemyId, Vector3 worldPos)
    {
        var enemy = new Enemy(enemyConfigs[enemyId]);
        CreateEnemyGameObject(worldPos, enemy);
        return enemy;
    }

    #endregion

    #region Check

    public bool HasActiveEnemy()
    {
        return m_activeGo2Enemy.Count > 0 || m_delayCount > 0;
    }
    
    public bool IsActiveEnemy(GameObject go)
    {
        return m_activeGo2Enemy.ContainsKey(go);
    }
    
    public bool FindClosestEnemyByPos(Vector3 position, out float distance, out GameObject closestEnemy)
    {
        distance = 9999f;
        closestEnemy = null;
        foreach (var enemy in m_activeEnemies)
        {
            var currentDis = Vector3.Distance(position, enemy.gameObject.transform.position);
            if (currentDis < distance)
            {
                distance = currentDis;
                closestEnemy = enemy.gameObject;
            }
        }

        return closestEnemy is not null;
    }

    public bool TryGetBossHpRatio(out float hpRatio)
    {
        hpRatio = 0;
        if (m_boss is null) return false;
        hpRatio = m_boss.GetHpRatio();
        return true;
    }

    #endregion

    #region Update

    public void SetDamageToEnemy(GameObject enemyGo, int damage)
    {
        if (!m_activeGo2Enemy.TryGetValue(enemyGo, out var enemy)) return;
        enemy.hp -= damage;
        StartCoroutine(enemy.ShowAffectFXCore());
    }
    
    public void UpdateAllActiveEnemy(Vector3 playerPos)
    {
        foreach (var enemy in m_activeEnemies)
        {
            enemy.Update(playerPos);
            if (!enemy.isAlive)
            {
                m_enemyUpdateTemp.Add(enemy);
            }
        }

        foreach (var enemy in m_enemyUpdateTemp)
        {
            if (m_boss is not null && enemy == m_boss)
            {
                OnBossDead?.Invoke(m_boss);
                m_boss = null;
            }
            else
            {
                OnNormalEnemyDead?.Invoke(enemy);
            }
            RecycleEnemyGameObject(enemy);
            //StartCoroutine(ShowEnemyDeadFX(enemy));
        }
        m_enemyUpdateTemp.Clear();
    }

    // private IEnumerator ShowEnemyDeadFX(Enemy enemy)
    // {
    //     enemy.animator.SetTrigger(Dead);
    //     yield return new WaitForSeconds(1);
    //     RecycleEnemyGameObject(enemy);
    // }

    #endregion
    
    #region Enemy Pool

    private readonly GameObjectPool m_gameObjectPool = new(1000);
    private GameObject m_enemyRoot;
    private GameObject m_warningRoot;

    private void InitPoolRoot()
    {
        m_enemyRoot = new GameObject("EnemyRoot");
        m_warningRoot = new GameObject("WarningRoot");
        
        m_enemyRoot.transform.SetParent(transform);
        m_warningRoot.transform.SetParent(transform);
    }

    private void CreateEnemyGameObject(Vector3 pos, Enemy enemy)
    {
        var enemyPrefab = enemy.enemyConfig.enemyPrefab;
        var enemyGameObject = m_gameObjectPool.TryGetGameObject(enemyPrefab);
        if (enemyGameObject == null)
        {
            enemyGameObject = Instantiate(enemyPrefab);
            enemyGameObject.SetActive(false);
        }
        enemy.gameObject = enemyGameObject;
        enemy.gameObject.transform.SetParent(m_enemyRoot.transform);
        enemy.gameObject.transform.position = pos;
        
        m_activeEnemies.Add(enemy);
        m_activeGo2Enemy.Add(enemy.gameObject, enemy);
        
        enemy.Init();
        enemy.gameObject.SetActive(true);
    }

    private void RecycleEnemyGameObject(Enemy enemy)
    {
        if (!m_activeEnemies.Contains(enemy))
        {
            Debug.LogError("这个对象不是怪物管理器管理的对象");
            return;
        }
        
        enemy.gameObject.SetActive(false);
        m_activeEnemies.Remove(enemy);
        m_activeGo2Enemy.Remove(enemy.gameObject);
        
        var enemyPrefab = enemy.enemyConfig.enemyPrefab;
        m_gameObjectPool.RecycleGameObject(enemyPrefab, enemy.gameObject);
    }

    private GameObject CreateWarningGameObject(Vector3 pos)
    {
        var warningGo = m_gameObjectPool.TryGetGameObject(warningPrefab);
        if (warningGo == null)
        {
            warningGo = Instantiate(warningPrefab);
        }
        warningGo.transform.SetParent(m_warningRoot.transform);
        warningGo.transform.position = pos;
        warningGo.SetActive(true);
        return warningGo;
    }

    private void RecycleWarningGameObject(GameObject warningGo)
    {
        warningGo.SetActive(false);
        m_gameObjectPool.RecycleGameObject(warningPrefab, warningGo);
    }

    #endregion
}
