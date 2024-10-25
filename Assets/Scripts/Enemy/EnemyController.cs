using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var bulletManager = ManagerLocator.Instance.Get<BulletManager>();
        if (!bulletManager.IsBullet(collision.gameObject)) return;

        var enemyManager = ManagerLocator.Instance.Get<EnemyManager>();
        enemyManager.SetEnemyDead(gameObject);
        bulletManager.SolveBulletHit(collision.gameObject);
    }
}
