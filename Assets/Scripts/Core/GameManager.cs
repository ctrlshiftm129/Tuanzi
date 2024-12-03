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
        LevelLoop();
    }

    private void FixedUpdate()
    {
        m_bulletManager.UpdateAllActiveBullets();
        m_enemyManager.UpdateAllActiveEnemy(player.transform.position);
    }

    #region Game Logic
    
    // 2分30秒一关
    // 1. 每2秒刷单个怪物 (normal wave)
    // 2. 每30秒一波怪群 (big wave)
    // 3. 每关到时间后出一个Boss (boss wave)
    
    private const int PER_TURN_TIME = 30;
    private const int PER_NORMAL_WAVE_TIME = 2;
    private const int PER_BIG_WAVE_TIME = 30;
    
    // level
    private int m_currentLevel;
    private bool m_isLevelRunning;
    // turn
    private float m_turnTime;
    private float m_normalWaveTimer;
    private float m_bigWaveTimer;
    private bool m_bossWave;
    
    private float m_bigWaveGenTimer;
    private int m_enemyCount;
    private Action m_enemyGenerator;

    #region Level & Turn
    
    private void InitGame()
    {
        StartLevel(1);
    }

    private void StartLevel(int level)
    {
        m_currentLevel = level;
        m_uiManager.ShowLevel(1);
        m_isLevelRunning = true;
        
        m_turnTime = PER_TURN_TIME;
        m_normalWaveTimer = m_bigWaveTimer = 0;
        m_bossWave = false;
        m_uiManager.ShowTime((int)m_turnTime);
    }
    
    private void LevelLoop()
    {
        if (!m_isLevelRunning) return;
        if (m_turnTime < 0)
        {
            if (!m_enemyManager.HasActiveEnemy())
            {
                if (!m_bossWave)
                {
                    UpdateBossWave();
                    return;
                }
                CompleteLevel();
            }
            return;
        }

        // 时间流逝
        m_turnTime -= Time.deltaTime;
        m_normalWaveTimer += Time.deltaTime;
        m_bigWaveTimer -= Time.deltaTime;
        m_uiManager.ShowTime((int)m_turnTime);

        UpdateNormalWave();
        UpdateBigWave();
    }

    private void CompleteLevel()
    {
        m_isLevelRunning = false;
    }

    #endregion

    #region Wave

    private const int PER_WAVE_ENEMY_NUM = 20;

    private void UpdateNormalWave()
    {
        if (m_normalWaveTimer >= PER_NORMAL_WAVE_TIME)
        {
            m_normalWaveTimer -= PER_NORMAL_WAVE_TIME;
            var pos = RandomUtils.RandomVector2Pos(2.5f, 17.5f);
            m_enemyManager.AddEnemyDelay(0, pos);
        }
    }

    private void UpdateBigWave()
    {
        if (m_bigWaveTimer <= 0)
        {
            m_bigWaveTimer += PER_BIG_WAVE_TIME;
            m_enemyCount = PER_WAVE_ENEMY_NUM;
            m_bigWaveGenTimer = 0;
            var createType = Random.Range(0, 2);
            m_enemyGenerator = createType switch
            {
                0 => AddDistributeEnemies,
                1 => AddGroupEnemies,
                _ => m_enemyGenerator
            };
        }
        
        // 刷怪
        m_enemyGenerator?.Invoke();
    }

    private void UpdateBossWave()
    {
        if (m_bossWave) return;

        m_bossWave = true;
        var pos = RandomUtils.RandomVector2Pos(2f, 18f);
        m_enemyManager.AddEnemyDelay(1, pos, 4);
    }

    #endregion

    #endregion

    #region Enemy Generator

    private void AddDistributeEnemies()
    {
        const float timeStep = 1f;
        if (m_enemyCount == 0) return;
        
        m_bigWaveGenTimer += Time.deltaTime;
        while (m_bigWaveGenTimer >= timeStep)
        {
            if (m_enemyCount == 0) return;
            m_bigWaveGenTimer -= timeStep;
            var random = Random.Range(0, 4);
            if (random == 0)
            {
                var randPos = Random.Range(2.5f, 16.5f);
                m_enemyManager.AddEnemyDelay(0, new Vector3(randPos, 2.5f, 0));
            }
            else if (random == 1)
            {
                var randPos = Random.Range(2.5f, 16.5f);
                m_enemyManager.AddEnemyDelay(0, new Vector3(17.5f, randPos, 0));
            }
            else if (random == 2)
            {
                var randPos = Random.Range(3.5f, 17.5f);
                m_enemyManager.AddEnemyDelay(0, new Vector3(randPos, 17.5f, 0));
            }
            else if (random == 3)
            {
                var randPos = Random.Range(3.5f, 17.5f);
                m_enemyManager.AddEnemyDelay(0, new Vector3(2.5f, randPos, 0));
            }

            --m_enemyCount;
        }
    }

    private Vector2 m_enemyGroupCenter;
    private void AddGroupEnemies()
    {
        const float timeStep = 0.5f;
        if (m_enemyCount == 0) return;
        
        m_bigWaveGenTimer += Time.deltaTime;
        while (m_bigWaveGenTimer >= timeStep)
        {
            if (m_enemyCount == 0) return;
            if (m_enemyCount % 10 == 0)
            {
                var randX = Random.Range(3f, 17f);
                var randY = Random.Range(3f, 17f);
                m_enemyGroupCenter = new Vector2(randX, randY);
            }
            m_bigWaveGenTimer -= timeStep;

            var pos = m_enemyGroupCenter + Random.insideUnitCircle * 2f;
            m_enemyManager.AddEnemyDelay(0, new Vector3(pos.x, pos.y, 0));
            --m_enemyCount;
        }
    }

    #endregion
}
