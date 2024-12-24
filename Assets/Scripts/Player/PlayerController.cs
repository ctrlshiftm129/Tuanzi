using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 玩家控制器
/// </summary>
public class PlayerController : MonoBehaviour
{
    public static WeaponConfig defaultWeaponConfig;
    
    public int hp;
    public GameProperty basicProp = new();
    [HideInInspector]
    public GameProperty additionalProp = new();
    public WeaponConfig weapon;

    public bool CanMove { get; set; }
    
    private Rigidbody2D m_rigidbody2D;
    private Animator m_animator;
    
    private UIManager m_uiManager;
    private EnemyManager m_enemyManager;
    private EconomicManager m_economicManager;
    private GameManager m_gameManager;
    
    public int AttAck => basicProp.attack + additionalProp.attack + weapon.weaponProp.attack;
    public float DamageRate => basicProp.damageRate + additionalProp.damageRate + weapon.weaponProp.damageRate;
    public float CriticalHitRate => Mathf.Max(basicProp.criticalHitRate + additionalProp.criticalHitRate + weapon.weaponProp.criticalHitRate, 0);
    private float FireRate => basicProp.fireRate + weapon.weaponProp.fireRate + additionalProp.fireRate;
    public float FireRateChange => basicProp.fireRateChange + weapon.weaponProp.fireRateChange + additionalProp.fireRateChange;
    public float WeaponRange => 
        Mathf.Max(basicProp.range + additionalProp.range + weapon.weaponProp.range, 10);
    private float Speed => basicProp.speed + additionalProp.speed;
    private float SpeedChange => basicProp.speedChange + additionalProp.speedChange;
    public float Lucky => 
        Mathf.Max(basicProp.lucky + additionalProp.lucky + weapon.weaponProp.lucky, 0);
    public int WeaponDamage => (int)Mathf.Max(AttAck * (1 + DamageRate * 0.01f), 1);
    public float WeaponFireRate => Mathf.Clamp(FireRate / (1 + FireRateChange * 0.01f), 0.1f, 5);
    public float PlayerSpeed => Mathf.Clamp(Speed * (1 + SpeedChange * 0.01f), 0.1f, 5);
    
    // Start is called before the first frame update
    void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        CanMove = true;
        if (defaultWeaponConfig != null) weapon = defaultWeaponConfig;
    }

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        
        var locator = ManagerLocator.Instance;
        m_uiManager = locator.Get<UIManager>();
        m_enemyManager = locator.Get<EnemyManager>();
        m_economicManager = locator.Get<EconomicManager>();
        m_gameManager = locator.Get<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        SolveMoveInput();
        UpdatePropertyUI();
    }

    private void FixedUpdate()
    {
        UpdateMove();
    }

    #region Collider Event

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var collisionGo = collision.gameObject;
        var isEnemy = m_enemyManager.IsActiveEnemy(collisionGo);
        if (isEnemy)
        {
            OnHitByEnemy();
        }
        
        var isChest = collision.GetComponent<ChestController>() != null;
        if (isChest)
        {
            OnHitByChest(collision.gameObject);
        }

        var isShop = collision.GetComponent<ShopController>() != null;
        if (isShop)
        {
            OnHitByShop();
        }
    }

    private void OnHitByEnemy()
    {
        Debug.LogError("死亡");
    }

    private void OnHitByChest(GameObject chest)
    {
        m_gameManager.TryStartOpenChest();
        Destroy(chest);
    }

    private void OnHitByShop()
    {
        m_gameManager.OpenShopPanel();
    }

    #endregion

    #region Update UI

    private void UpdatePropertyUI()
    {
        m_uiManager.ShowPlayerProperty(this);
    }

    #endregion

    #region Move Logic

    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int FaceLeft = Animator.StringToHash("FaceLeft");
    private Vector2 m_moveDirection;
    
    private void SolveMoveInput()
    {
        m_moveDirection = InputUtils.GetMoveDirection();
        if (m_moveDirection == Vector2.zero)
        {
            m_animator.SetBool(Running, false);
        }
        else
        {
            m_animator.SetBool(Running, true);
            if (m_moveDirection.x > 0)
            {
                m_animator.SetBool(FaceLeft, false);
            }
            else if (m_moveDirection.x < 0)
            {
                m_animator.SetBool(FaceLeft, true);
            }
        }
    }

    private void UpdateMove()
    {
        if (!CanMove)
        {
            m_rigidbody2D.velocity = Vector2.zero;
            return;
        }
        
        var move = m_moveDirection * PlayerSpeed;
        m_rigidbody2D.velocity = move;
    }

    #endregion
}
