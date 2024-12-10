using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// 暂时靠这个跑游戏逻辑
/// </summary>
public class GameManager : Manager
{
    private static readonly int Exit = Animator.StringToHash("Exit");
    private const string TITLE_SCENE_NAME = "Title";
    
    public bool debugMode;

    [Space] 
    public GameObject cameraRoot;
    public GameObject eventSystem;
    public GameObject mapBackground;
    public GameObject mapRoot;
    
    [Space]
    public GameObject gameRoot;
    public GameObject player;
    
    private ManagerLocator m_locator;
    private BulletManager m_bulletManager;
    private EnemyManager m_enemyManager;
    private UIManager m_uiManager;
    
    private void Awake()
    {
        Register2Locator();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_locator = ManagerLocator.Instance;
        m_bulletManager = m_locator.Get<BulletManager>();
        m_enemyManager = m_locator.Get<EnemyManager>();
        m_uiManager = m_locator.Get<UIManager>();

        if (debugMode)
        {
            StartCoroutine(InitScene());
        }
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

    #region Init Logic

    public void StartInitFromTitle()
    {
        StartCoroutine(PrepareInitScene());
    }
    
    /// <summary>
    /// 标题场景控制器将处理权转交给GameManager
    /// 尝试卸载标题场景，如果有的话
    /// </summary>
    private IEnumerator PrepareInitScene()
    {
        var titleScene = SceneManager.GetSceneByName(TITLE_SCENE_NAME);
        if (titleScene.IsValid())
        {
            var async = SceneManager.UnloadSceneAsync(TITLE_SCENE_NAME);
            if (async == null)
            {
                Debug.LogError($"卸载游戏场景失败, 请联系管理员 {TITLE_SCENE_NAME}");
                yield break;
            }
        
            //异步卸载场景
            while (!async.isDone)
            {
                yield return null;
            }
        }

        StartCoroutine(InitScene());
    }
    
    private IEnumerator InitScene()
    {
        mapBackground.SetActive(true);
        cameraRoot.SetActive(true);
        eventSystem.SetActive(true);
        mapRoot.SetActive(true);

        var backgroundAnimator = mapBackground.GetComponent<Animator>();
        backgroundAnimator.SetTrigger(Exit);
        while (backgroundAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }
        
        Debug.Log("初始化场景完成");
        mapBackground.SetActive(false);
        StartCoroutine(ShowStartAnimation());
    }

    private IEnumerator ShowStartAnimation()
    {
        yield return new WaitForSeconds(2);
        
        var animator = gameRoot.GetComponent<Animator>();
        animator.SetTrigger(Animator.StringToHash("Start"));
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8)
            {
                player.SetActive(true);
            }
            yield return null;
        }
        
        var playerController = player.GetComponent<PlayerController>();
        playerController.enabled = true;
        var autoAttack = player.GetComponent<PlayerAutoAttack>();
        autoAttack.enabled = true;
        Debug.Log("过场动画完成");
        InitGame();
    }

    #endregion

    #region Game Logic
    
    // 2分30秒一关
    // 1. 每2秒刷单个怪物 (normal wave)
    // 2. 每30秒一波怪群 (big wave)
    // 3. 每关到时间后出一个Boss (boss wave)
    
    private const int PER_TURN_TIME = 150;
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
        StartCoroutine(StartLevel(1));
    }

    private float m_countdownTimer;
    private IEnumerator StartLevel(int level)
    {
        m_currentLevel = level;
        m_uiManager.ShowLevel(1);
        m_countdownTimer = 4;
        while (m_countdownTimer > 0)
        {
            m_countdownTimer -= Time.deltaTime;
            var countdownInt = (int)m_countdownTimer;
            var text = countdownInt == 0 ? "开始行动" : countdownInt.ToString();
            m_uiManager.ShowCountdownText(text);
            yield return null;
        }
        
        m_isLevelRunning = true;
        m_turnTime = PER_TURN_TIME;
        m_normalWaveTimer = m_bigWaveTimer = 0;
        m_bossWave = false;
        m_uiManager.ShowLevelTime((int)m_turnTime);
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
        m_uiManager.ShowLevelTime((int)m_turnTime);

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
            var pos = RandomUtils.RandomVector2Pos(0.8f, 19.2f, 0.8f, 13.2f);
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
        var pos = RandomUtils.RandomVector2Pos(2f, 18f, 2f, 12f);
        m_enemyManager.AddEnemyDelay(1, pos, 4, true);
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
            Vector3 randPos = Vector3.one;
            if (random == 0)
            {
                randPos = RandomUtils.RandomVector2WithFixedY(2f, 2f, 18f);
            }
            else if (random == 1)
            {
                randPos = RandomUtils.RandomVector2WithFixedX(18f, 2f, 12f);
            }
            else if (random == 2)
            {
                randPos = RandomUtils.RandomVector2WithFixedY(12f, 2f, 18f);
            }
            else if (random == 3)
            {
                randPos = RandomUtils.RandomVector2WithFixedX(2f, 2f, 12f);
            }

            m_enemyManager.AddEnemyDelay(0, randPos);
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
                var randY = Random.Range(3f, 11f);
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
