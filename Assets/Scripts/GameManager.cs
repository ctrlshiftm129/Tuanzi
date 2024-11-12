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
    private UIManager m_uiManager;

    // Start is called before the first frame update
    void Start()
    {
        m_locator = ManagerLocator.Instance;
        m_bulletManager = m_locator.Get<BulletManager>();
        m_enemyManager = m_locator.Get<EnemyManager>();
        m_uiManager = m_locator.Get<UIManager>();

        InitGame();
    }

    private void Update()
    {
        if (m_isLevelRunning)
        {
            LevelLoop();
        }
    }

    private void FixedUpdate()
    {
        m_bulletManager.UpdateAllActiveBullets();
        m_enemyManager.UpdateAllActiveEnemy(player.transform.position);
    }

    #region Game Logic
    
    private const int PER_LEVEL_TURN_NUM = 3;

    private int m_currentLevel;

    // level
    private bool m_isLevelRunning;
    // turn
    private bool m_isTurnRunning;
    private int m_turnCount;
    
    private float m_timer;
    private int m_enemyCount;
    private Action m_enemyGenerator;

    #region Level
    
    private void InitGame()
    {
        StartLevel(1);
    }

    private void StartLevel(int level)
    {
        m_currentLevel = level;
        m_uiManager.ShowLevel(1);
        m_isLevelRunning = true;
        
        m_turnCount = 0;
        m_isTurnRunning = false;
    }
    
    private void LevelLoop()
    {
        if (!m_isLevelRunning) return;
        if (!m_isTurnRunning)
        {
            if (m_turnCount == PER_LEVEL_TURN_NUM)
            {
                CompleteLevel();
            }

            CreateNewTurn();
            ++m_turnCount;
            m_isTurnRunning = true;
        }
        
        m_enemyGenerator.Invoke();
        if (m_enemyCount == 0 && !m_enemyManager.HasActiveEnemy())
        {
            m_isTurnRunning = false;
        }
    }

    private void CompleteLevel()
    {
        m_isLevelRunning = false;
    }

    #endregion

    #region Turn

    private void CreateNewTurn()
    {
        m_enemyCount = 20;
        m_timer = 0;
        
        var createType = Random.Range(0, 2);
        m_enemyGenerator = createType switch
        {
            0 => AddDistributeEnemies,
            1 => AddGroupEnemies,
            _ => m_enemyGenerator
        };
    }

    #endregion

    #endregion

    #region Enemy Generator

    private void AddDistributeEnemies()
    {
        const float timeStep = 0.5f;
        if (m_enemyCount == 0) return;
        
        m_timer += Time.deltaTime;
        while (m_timer >= timeStep)
        {
            if (m_enemyCount == 0) return;
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

            --m_enemyCount;
        }
    }
    
    private void AddGroupEnemies()
    {
        const float timeStep = 0.2f;
        if (m_enemyCount == 0) return;
        
        m_timer += Time.deltaTime;
        while (m_timer >= timeStep)
        {
            if (m_enemyCount == 0) return;
            m_timer -= timeStep;
            var center = new Vector2(8f, 15.5f);

            var pos = center + Random.insideUnitCircle * 2f;
            m_enemyManager.AddEnemyDelay(enemyConfig, new Vector3(pos.x, pos.y, 0));
            --m_enemyCount;
        }
    }

    #endregion
}
