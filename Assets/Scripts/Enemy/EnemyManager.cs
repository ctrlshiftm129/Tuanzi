using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Manager
{
    public GameObject enemyPrefab;
    private readonly Dictionary<GameObject, EnemyBase> m_activeGo2Enemy = new Dictionary<GameObject, EnemyBase>();
    private readonly HashSet<EnemyBase> m_activeEnemies = new HashSet<EnemyBase>();
    private readonly List<EnemyBase> m_enemyUpdateTemp = new List<EnemyBase>();

    private void Awake()
    {
        Register2Locator();
    }

    public void AddEnemy(Vector3 worldPos)
    {
        var enemy = new NormalEnemy();
        enemy = CreateEnemyGameObject(enemy);
        enemy.gameObject.transform.position = worldPos;
        enemy.isAlive = true;
        m_activeEnemies.Add(enemy);
        m_activeGo2Enemy.Add(enemy.gameObject, enemy);
    }

    public void SetEnemyDead(GameObject enemyGo)
    {
        if (!m_activeGo2Enemy.TryGetValue(enemyGo, out var enemy)) return;
        enemy.isAlive = false;
    }

    public void UpdateAllActiveEnemy(Vector3 playerPos)
    {
        foreach (var enemy in m_activeEnemies)
        {
            if (!enemy.isAlive)
            {
                m_enemyUpdateTemp.Add(enemy);
            }
            enemy.Update(playerPos);
        }

        foreach (var enemy in m_enemyUpdateTemp)
        {
            RecycleEnemyGameObject(enemy);
            m_activeEnemies.Remove(enemy);
            m_activeGo2Enemy.Remove(enemy.gameObject);
            enemy.gameObject.SetActive(false);
        }
        m_enemyUpdateTemp.Clear();
    }

    public bool IsActiveEnemy(GameObject gameObject)
    {
        return m_activeGo2Enemy.ContainsKey(gameObject);
    }

    #region Enemy Pool

    private readonly GameObjectPool m_gameObjectPool = new GameObjectPool(1000);

    private T CreateEnemyGameObject<T>(T enemy) where T : EnemyBase
    {
        var enemyGameObject = m_gameObjectPool.TryGetGameObject();
        if (enemyGameObject == null)
        {
            enemyGameObject = Instantiate(enemyPrefab);
        }
        enemy.gameObject = enemyGameObject;
        enemy.gameObject.transform.SetParent(transform);
        enemy.Init();
        enemyGameObject.SetActive(true);
        return enemy;
    }

    private void RecycleEnemyGameObject(EnemyBase enemy)
    {
        if (!m_activeEnemies.Contains(enemy))
        {
            Debug.LogError("��������ǹ������������Ķ���");
            return;
        }

        enemy.gameObject.SetActive(false);
        m_gameObjectPool.RecycleGameObject(enemy.gameObject);
    }

    #endregion
}
