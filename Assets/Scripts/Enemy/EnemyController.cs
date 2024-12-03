using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private BulletManager m_bulletManager;

    private BulletManager BulletManager
    {
        get
        {
            if (m_bulletManager == null)
            {
                m_bulletManager = ManagerLocator.Instance.Get<BulletManager>();
            }
            return m_bulletManager;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var collisionGo = collision.gameObject;
        var bullet = BulletManager.GetBullet(collisionGo);
        if (bullet != null)
        {
            SolveBulletHit(bullet, collisionGo);
        }
    }

    private void SolveBulletHit(Bullet bullet, GameObject bulletGo)
    {
        var enemyManager = ManagerLocator.Instance.Get<EnemyManager>();
        var uiManager = ManagerLocator.Instance.Get<UIManager>();
        var rand = (int)(1 + Random.value * 99);
        var critical = rand <= bullet.criticalHitRate;
        var damage = bullet.damage;
        if (critical) damage *= 2;
        enemyManager.SetDamageToEnemy(gameObject, damage);
        BulletManager.SolveBulletHit(bulletGo);
        uiManager.ShowDamageAtPos(damage, critical, gameObject.transform.position);
    }
}
