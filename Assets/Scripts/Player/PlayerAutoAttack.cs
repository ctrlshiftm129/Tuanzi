using System;
using UnityEngine;

public class PlayerAutoAttack : MonoBehaviour
{
    // 方便演示
    public bool sanshe; 
    public bool judahua; 
    public bool huimie; 
    
    private PlayerController m_player;
    private AudioSource m_audioSource;
    private float m_fireRateCounter;
    
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

    private void Awake()
    {
        m_locator = ManagerLocator.Instance;
    }

    private void Start()
    {
        m_player = GetComponent<PlayerController>();
        m_audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (m_player is null) return;
        AutoAttackUpdate();
    }
    
    private void AutoAttackUpdate()
    {
        if (m_fireRateCounter > 0)
        {
            m_fireRateCounter -= Time.deltaTime;
            return;
        }

        if (!TryAttack()) return;
        m_fireRateCounter = m_player.WeaponFireRate;
    }
    
    private bool TryAttack()
    {
        var playerPos = transform.position;
        if (!EnemyManager.FindClosestEnemyByPos(playerPos, out var dis, out var enemy))
        {
            return false;
        }

        // 范围不够
        var range = m_player.WeaponRange;
        if (dis * 10 > range) return false;
        var attackDirection = (enemy.transform.position - playerPos).normalized;
        m_audioSource.Play();
        Func<Bullet, Bullet> callback = null;
        if (judahua) callback += SolveJuDaHua;
        if (huimie) callback += SolveHuiMie;
        BulletManager.PlayerShoot(m_player, attackDirection, m_player.weapon.bulletConfig, callback);
        if (sanshe)
        {
            var dir1 = Quaternion.Euler(new Vector3(0, 0, 10)) * attackDirection;
            BulletManager.PlayerShoot(m_player, dir1, m_player.weapon.bulletConfig, callback);
            var dir2 = Quaternion.Euler(new Vector3(0, 0, -10)) * attackDirection;
            BulletManager.PlayerShoot(m_player, dir2, m_player.weapon.bulletConfig, callback);
        }
        
        return true;
    }

    private Bullet SolveJuDaHua(Bullet bullet)
    {
        bullet.gameObject.transform.localScale *= 2.5f;
        return bullet;
    }
    
    private Bullet SolveHuiMie(Bullet bullet)
    {
        bullet.huimie = true;
        return bullet;
    }
}
