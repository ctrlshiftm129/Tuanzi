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

    private Rigidbody2D m_rigidbody2D;
    private UIManager m_uiManager;
    private EnemyManager m_enemyManager;
    private Animator m_animator;
    
    public int WeaponDamage => 
        Mathf.Max(basicProp.attack + additionalProp.attack + weapon.weaponProp.attack, 1);
    public float DamageRate => 
        Mathf.Max(basicProp.damageRate + additionalProp.damageRate + weapon.weaponProp.damageRate, 0);
    public float CriticalHitRate => 
        Mathf.Max(basicProp.criticalHitRate + additionalProp.criticalHitRate + weapon.weaponProp.criticalHitRate, 0);
    public float WeaponFireRate => 
        Mathf.Clamp(basicProp.fireRate + additionalProp.fireRate + weapon.weaponProp.fireRate, 0.1f, 10f);
    public float WeaponRange => 
        Mathf.Max(basicProp.range + additionalProp.range + weapon.weaponProp.range, 10);
    public float Speed => 
        Mathf.Max(basicProp.speed + additionalProp.speed + weapon.weaponProp.speed, 0);
    public float Lucky => 
        Mathf.Max(basicProp.lucky + additionalProp.lucky + weapon.weaponProp.lucky, 0);
    
    // Start is called before the first frame update
    void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        if (defaultWeaponConfig != null) weapon = defaultWeaponConfig;
    }

    private void Start()
    {
        var locator = ManagerLocator.Instance;
        m_uiManager = locator.Get<UIManager>();
        m_enemyManager = locator.Get<EnemyManager>();
        m_animator = GetComponent<Animator>();
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
        if (isEnemy) Debug.LogError("死亡");
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
        var move = m_moveDirection * Speed;
        m_rigidbody2D.velocity = move;
    }

    #endregion
}
