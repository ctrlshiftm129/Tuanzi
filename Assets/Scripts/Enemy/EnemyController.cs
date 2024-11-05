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
        enemyManager.SetDamageToEnemy(gameObject, bullet.damage);
        BulletManager.SolveBulletHit(bulletGo);
    }
}
