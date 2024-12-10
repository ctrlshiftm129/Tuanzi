using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理所有子弹
/// </summary>
public class BulletManager : Manager
{
    public Vector2 rangeX;
    public Vector2 rangeY;

    private readonly Dictionary<GameObject, Bullet> m_activeGo2Bullet = new();
    private readonly HashSet<Bullet> m_activeBullets = new();
    private readonly List<Bullet> m_bulletUpdateTemp = new();

    private void Awake()
    {
        Register2Locator();
    }

    #region Create

    public void PlayerShoot(PlayerController player, Vector3 direction, BulletConfig bulletConfig)
    {
        var bullet = new Bullet(BulletOwner.Player)
        {
            bulletPrefab = bulletConfig.bulletPrefab,
            originalPos = player.transform.position,
            direction = direction,
            range = player.WeaponRange,
            speed = bulletConfig.speed,
            damage = player.WeaponDamage,
            criticalHitRate = player.CriticalHitRate,
            penetrate = bulletConfig.penetrate
        };
        CreateNewBulletGameObject(bullet);

        m_activeBullets.Add(bullet);
        m_activeGo2Bullet.Add(bullet.gameObject, bullet);
    }
    
    #endregion

    #region Check

    public Bullet GetBullet(GameObject go)
    {
        return m_activeGo2Bullet.GetValueOrDefault(go, null);
    }
    
    private bool IsBulletInRange(Bullet bullet)
    {
        var bulletPos = bullet.gameObject.transform.position;
        var moveDis = Vector3.Distance(bulletPos, bullet.originalPos);
        if (moveDis > bullet.range) return false;

        return bulletPos.x > rangeX.x && bulletPos.x < rangeX.y
                                      && bulletPos.y > rangeY.x && bulletPos.y < rangeY.y;
    }

    #endregion

    #region Update

    public void UpdateAllActiveBullets()
    {
        foreach (var bullet in m_activeBullets)
        {
            bullet.FixedUpdate();
            if (!IsBulletInRange(bullet))
            {
                m_bulletUpdateTemp.Add(bullet);
            }
        }

        foreach (var bullet in m_bulletUpdateTemp)
        {
            KillBullet(bullet);
        }
        m_bulletUpdateTemp.Clear();
    }

    public void SolveBulletHit(GameObject go)
    {
        if (m_activeGo2Bullet.TryGetValue(go, out var bullet))
        {
            // 子弹穿透
            if (bullet.penetrate) return;
            KillBullet(bullet);
        }
    }

    private void KillBullet(Bullet bullet)
    {
        RecycleBullet(bullet);
        m_activeGo2Bullet.Remove(bullet.gameObject);
        m_activeBullets.Remove(bullet);
    }

    #endregion

    #region Bullet Pool

    private readonly Dictionary<GameObject, Transform> m_bulletCacheRoot = new();
    private readonly GameObjectPool m_gameObjectPool = new(1000);

    private void CreateNewBulletGameObject(Bullet bullet)
    {
        var prefab = bullet.bulletPrefab;
        var bulletGameObject = m_gameObjectPool.TryGetGameObject(prefab);
        if (bulletGameObject == null)
        {
            bulletGameObject = Instantiate(prefab);
            bulletGameObject.SetActive(false);
        }

        bullet.gameObject = bulletGameObject;
        var bulletTransform = bullet.gameObject.transform;
        bulletTransform.SetParent(GetOrNewBulletCacheRoot(prefab));
        bulletTransform.position = bullet.originalPos;
        bulletTransform.rotation = Quaternion.identity;

        bullet.Init();
        bullet.gameObject.SetActive(true);
    }

    private void RecycleBullet(Bullet bullet)
    {
        if (!m_activeBullets.Contains(bullet))
        {
            Debug.LogError("这个对象不是子弹管理器管理的对象");
            return;
        }
        
        bullet.gameObject.SetActive(false);
        m_gameObjectPool.RecycleGameObject(bullet.bulletPrefab, bullet.gameObject);
    }

    private Transform GetOrNewBulletCacheRoot(GameObject prefab)
    {
        if (m_bulletCacheRoot.TryGetValue(prefab, out var root)) return root;

        root = new GameObject(prefab.name).transform;
        root.SetParent(transform);
        m_bulletCacheRoot[prefab] = root;
        return root;
    }

    #endregion
}
