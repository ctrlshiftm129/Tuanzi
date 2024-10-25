using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Íæ¼Ò¿ØÖÆÆ÷
/// </summary>
public class PlayerController : MonoBehaviour
{
    public int hp;
    public GameProperty basicProp = new GameProperty();
    [HideInInspector]
    public GameProperty additionalProp = new GameProperty();
    public WeaponConfig weapon;

    private Vector2 m_moveDirection;
    private Rigidbody2D m_rigidbody2D;
    private ManagerLocator m_locator;

    private EnemyManager m_enemyManager;
    private EnemyManager EnemyManager
    {
        get
        {
            if (m_enemyManager != null) return m_enemyManager;
            return m_enemyManager = m_locator.Get<EnemyManager>();
        }
    }

    private BulletManager m_bulletManager;
    private BulletManager BulletManager
    {
        get
        {
            if (m_bulletManager != null) return m_bulletManager;
            return m_bulletManager = m_locator.Get<BulletManager>();
        }
    }

    public float Speed => Mathf.Max(basicProp.speed + additionalProp.speed + weapon.weaponProp.speed, 0);

    public float Range => Mathf.Max(basicProp.range + additionalProp.range + weapon.weaponProp.range, 10);

    public float FireRate => Mathf.Clamp(basicProp.fireRate + additionalProp.fireRate + weapon.weaponProp.fireRate, 0.1f, 10f);

    // Start is called before the first frame update
    void Start()
    {
        m_locator = ManagerLocator.Instance;
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        SolveMoveInput();
        AutoAttack();
    }

    private void SolveMoveInput()
    {
        m_moveDirection = InputUtils.GetMoveDirection();
        m_rigidbody2D.velocity = m_moveDirection * Speed;
    }

    #region Auto Attack Logic

    private float m_fireRateCounter = 0;
    private void AutoAttack()
    {
        if (m_fireRateCounter > 0)
        {
            m_fireRateCounter -= Time.deltaTime;
            return;
        }

        if (!TryAttack()) return;
        m_fireRateCounter = FireRate;
    }

    private bool TryAttack()
    {
        var playerPos = transform.position;
        if (!EnemyManager.FindClosestEnemyByPos(playerPos, out var dis, out var enemy))
        {
            return false;
        }

        // ·¶Î§²»¹»
        var range = Range;
        if (dis * 10 > range) return false;
        var attackDirection = (enemy.transform.position -playerPos).normalized;
        BulletManager.PlayerShoot(playerPos, attackDirection, range, weapon.bulletConfig);

        return true;
    }

    public void Switch1()
    {
        weapon = AssetDatabase.LoadAssetAtPath<WeaponConfig>("Assets/Configs/Weapons/Pistol.asset");
    }

    public void Switch2()
    {
        weapon = AssetDatabase.LoadAssetAtPath<WeaponConfig>("Assets/Configs/Weapons/SniperRifle.asset");
    }

    public void Switch3()
    {
        weapon = AssetDatabase.LoadAssetAtPath<WeaponConfig>("Assets/Configs/Weapons/Submachine.asset");
    }

    #endregion

}
