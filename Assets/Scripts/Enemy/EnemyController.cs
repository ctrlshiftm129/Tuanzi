using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    private static readonly int Running = Animator.StringToHash("Running");
    
    private BulletManager m_bulletManager;
    private Animator m_animator;
    private Rigidbody2D m_rigidbody2D;
    private SpriteRenderer m_renderer;

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

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        m_renderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        m_animator.SetBool(Running, m_rigidbody2D.velocity != Vector2.zero);
        if (m_rigidbody2D.velocity.x < 0)
        {
            m_renderer.flipX = true;
        }
        else if (m_rigidbody2D.velocity.x > 0)
        {
            m_renderer.flipX = false;
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
