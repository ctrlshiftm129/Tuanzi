using UnityEngine;

public class PlayerAutoAttack : MonoBehaviour
{
    private PlayerController m_player;
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
        m_player = GetComponent<PlayerController>();
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
        var attackDirection = (enemy.transform.position -playerPos).normalized;
        BulletManager.PlayerShoot(m_player, attackDirection, m_player.weapon.bulletConfig);

        return true;
    }
}
