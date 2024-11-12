using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : Manager
{
    public GameObject enemyPrefab;
    public GameObject warningPrefab;
    
    private readonly Dictionary<GameObject, Enemy> m_activeGo2Enemy = new();
    private readonly HashSet<Enemy> m_activeEnemies = new();
    private readonly List<Enemy> m_enemyUpdateTemp = new();

    private void Awake()
    {
        InitPoolRoot();
        Register2Locator();
    }

    #region Create

    public void AddEnemy(EnemyConfig config, Vector3 worldPos)
    {
        var enemy = new Enemy(config);
        CreateEnemyGameObject(worldPos, enemy);
    }

    public void AddEnemyDelay(EnemyConfig config, Vector3 worldPos, float delay = 2)
    {
        var warningGo = CreateWarningGameObject(worldPos);
        StartCoroutine(AddEnemyDelayCore(warningGo, config, worldPos, delay));
    }

    private IEnumerator AddEnemyDelayCore(GameObject warningGo, EnemyConfig config, Vector3 worldPos, float delay)
    {
        yield return new WaitForSeconds(delay); // 等待一段时间
        RecycleWarningGameObject(warningGo);
        AddEnemy(config, worldPos);
    }

    #endregion

    #region Check

    public bool HasActiveEnemy()
    {
        return m_activeGo2Enemy.Count > 0;
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

    #endregion

    #region Update

    public void SetDamageToEnemy(GameObject enemyGo, int damage)
    {
        if (!m_activeGo2Enemy.TryGetValue(enemyGo, out var enemy)) return;
        enemy.hp -= damage;
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
            RecycleEnemyGameObject(enemy);
        }
        m_enemyUpdateTemp.Clear();
    }

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
        var enemyGameObject = m_gameObjectPool.TryGetGameObject(enemyPrefab);
        if (enemyGameObject == null)
        {
            enemyGameObject = Instantiate(enemyPrefab);
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
