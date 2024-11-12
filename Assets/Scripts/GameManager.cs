using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 暂时靠这个跑游戏逻辑
/// </summary>
public class GameManager : MonoBehaviour
{
    public GameObject player;
    public EnemyConfig enemyConfig;

    private ManagerLocator m_locator;
    private BulletManager m_bulletManager;
    private EnemyManager m_enemyManager;

    // Start is called before the first frame update
    void Start()
    {
        m_locator = ManagerLocator.Instance;
        m_bulletManager = m_locator.Get<BulletManager>();
        m_enemyManager = m_locator.Get<EnemyManager>();
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        m_bulletManager.UpdateAllActiveBullets();
        m_enemyManager.UpdateAllActiveEnemy(player.transform.position);
        AddNewEnemy();
    }

    private int m_enemyCount;
    private float m_timer;

    private void AddNewEnemy()
    {
        AddGroupEnemies();
    }
    
    private void AddDistributeEnemies()
    {
        const int enemyCount = 20;
        const float timeStep = 0.5f;

        if (m_enemyCount >= enemyCount) return;
        m_timer += Time.deltaTime;
        while (m_timer >= timeStep)
        {
            m_timer -= timeStep;
            var random = Random.Range(0, 4);
            if (random == 0)
            {
                var randPos = Random.Range(2.5f, 16.5f);
                m_enemyManager.AddEnemyDelay(enemyConfig, new Vector3(randPos, 2.5f, 0));
            }
            else if (random == 1)
            {
                var randPos = Random.Range(2.5f, 16.5f);
                m_enemyManager.AddEnemyDelay(enemyConfig, new Vector3(17.5f, randPos, 0));
            }
            else if (random == 2)
            {
                var randPos = Random.Range(3.5f, 17.5f);
                m_enemyManager.AddEnemyDelay(enemyConfig, new Vector3(randPos, 17.5f, 0));
            }
            else if (random == 3)
            {
                var randPos = Random.Range(3.5f, 17.5f);
                m_enemyManager.AddEnemyDelay(enemyConfig, new Vector3(2.5f, randPos, 0));
            }

            ++m_enemyCount;
        }
    }
    
    private void AddGroupEnemies()
    {
        const int enemyCount = 20;
        const float timeStep = 0.1f;
        
        if (m_enemyCount >= enemyCount) return;
        m_timer += Time.deltaTime;
        while (m_timer >= timeStep)
        {
            m_timer -= timeStep;
            var center = new Vector2(8f, 15.5f);

            var pos = center + Random.insideUnitCircle * 2f;
            m_enemyManager.AddEnemyDelay(enemyConfig, new Vector3(pos.x, pos.y, 0));
            ++m_enemyCount;
        }
    }
}
