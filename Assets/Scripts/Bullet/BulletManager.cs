using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子弹管理器
/// 使用对象池管理场景内所有子弹对象的创建销毁和更新
/// </summary>
public class BulletManager : Manager
{
    public GameObject bulletPrefab;
    private readonly Dictionary<GameObject, BulletBase> m_activeGo2Bullet = new Dictionary<GameObject, BulletBase>();
    private readonly HashSet<BulletBase> m_activeBullets = new HashSet<BulletBase>();
    private readonly List<BulletBase> m_bulletUpdateTemp = new List<BulletBase>();

    private void Awake()
    {
        Register2Locator();
    }

    public void PlayerShoot(Vector3 pos, Vector3 direction, float speed)
    {
        var bullet = new StraightBullet(BulletOwner.Player, direction, speed);
        bullet = CreateNewBulletGameObject(bullet);
        bullet.gameObject.transform.position = pos;
        m_activeBullets.Add(bullet);
        m_activeGo2Bullet.Add(bullet.gameObject, bullet);
    }

    public void UpdateAllActiveBullets()
    {
        foreach (var bullet in m_activeBullets)
        {
            bullet.Update();
            if (!IsBulletValid(bullet))
            {
                m_bulletUpdateTemp.Add(bullet);
            }
        }

        foreach (var bullet in m_bulletUpdateTemp)
        {
            RecycleBullet(bullet);
            m_activeBullets.Remove(bullet);
            m_activeGo2Bullet.Remove(bullet.gameObject);
            bullet.gameObject.SetActive(false);
        }
        m_bulletUpdateTemp.Clear();
    }

    public bool IsBullet(GameObject gameObject)
    {
        return m_activeGo2Bullet.ContainsKey(gameObject);
    }

    private bool IsBulletValid(BulletBase bullet)
    {
        var position = bullet.gameObject.transform.position;
        return position.x > -1 && position.x < 21
            && position.y > -1 && position.y < 21;
    }

    #region Bullet Pool

    private readonly GameObjectPool m_gameObjectPool = new GameObjectPool(1000);

    private T CreateNewBulletGameObject<T>(T bullet) where T : BulletBase
    {
        var bulletGameObject = m_gameObjectPool.TryGetGameObject();
        // 池子里没有缓存的了
        if (bulletGameObject == null)
        {
            bulletGameObject = Instantiate(bulletPrefab);
        }
        bullet.gameObject = bulletGameObject;
        bullet.gameObject.transform.SetParent(transform);
        bullet.Init();
        bulletGameObject.SetActive(true);
        return bullet;
    }

    private void RecycleBullet(BulletBase bullet)
    {
        if (!m_activeBullets.Contains(bullet))
        {
            Debug.LogError("这个对象不是子弹管理器管理的对象");
            return;
        }

        bullet.gameObject.SetActive(false);
        m_gameObjectPool.RecycleGameObject(bullet.gameObject);
    }

    #endregion
}
