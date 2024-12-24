using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int FlipX = Shader.PropertyToID("_FlipX");

    private BulletManager m_bulletManager;
    private Animator m_animator;
    private Rigidbody2D m_rigidbody2D;
    private Material m_enemyMat;

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
        m_enemyMat = gameObject.GetComponent<SpriteRenderer>().material;
    }

    private void Update()
    {
        m_animator.SetBool(Running, m_rigidbody2D.velocity != Vector2.zero);
        if (m_rigidbody2D.velocity.x < 0)
        {
            m_enemyMat.SetInt(FlipX, 1);
        }
        else if (m_rigidbody2D.velocity.x > 0)
        {
            m_enemyMat.SetInt(FlipX, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var bulletController = collision.GetComponentInParent<BulletController>();
        if (bulletController is null) return;
        var collisionGo = bulletController.gameObject;
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
        if (critical)
        {
            var factor = 2;
            if (bullet.huimie && Random.value > 0.6f)
            {
                factor = 3;
            }
            damage *= factor;
        }
        enemyManager.SetDamageToEnemy(gameObject, damage);
        BulletManager.SolveBulletHit(bulletGo);
        uiManager.ShowDamageAtPos(damage, critical, gameObject.transform.position);
    }
}
