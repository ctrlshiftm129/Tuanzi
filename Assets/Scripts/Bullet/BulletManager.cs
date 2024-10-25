using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ӵ�������
/// ʹ�ö���ع������������ӵ�����Ĵ������ٺ͸���
/// </summary>
public class BulletManager : Manager
{
    private readonly Dictionary<GameObject, BulletObjectBase> m_activeGo2Bullet = new Dictionary<GameObject, BulletObjectBase>();
    private readonly HashSet<BulletObjectBase> m_activeBullets = new HashSet<BulletObjectBase>();
    private readonly List<BulletObjectBase> m_bulletUpdateTemp = new List<BulletObjectBase>();

    private void Awake()
    {
        Register2Locator();
    }

    public void PlayerShoot(Vector3 pos, Vector3 direction, float range, BulletConfig bulletConfig)
    {
        var bullet = new StraightBullet(BulletOwner.Player)
        {
            bulletPrefab = bulletConfig.bulletPrefab,
            originalPos = pos,
            direction = direction,
            range = range,
            speed = bulletConfig.speed,
            penetrate = bulletConfig.penetrate
        };
        bullet = CreateNewBulletGameObject(pos, bullet);

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

    public void SolveBulletHit(GameObject gameObject)
    {
        if (m_activeGo2Bullet.TryGetValue(gameObject, out var bullet))
        {
            // ����͸ʱ������
            if (bullet.penetrate) return;
            RecycleBullet(bullet);
            m_activeGo2Bullet.Remove(gameObject);
            m_activeBullets.Remove(bullet);
        }
    }

    public bool IsBullet(GameObject gameObject)
    {
        return m_activeGo2Bullet.ContainsKey(gameObject);
    }

    private bool IsBulletValid(BulletObjectBase bullet)
    {
        var bulletPos = bullet.gameObject.transform.position;
        var moveDis = Vector3.Distance(bulletPos, bullet.originalPos);
        if (moveDis > bullet.range) return false;

        return bulletPos.x > -1 && bulletPos.x < 21
            && bulletPos.y > -1 && bulletPos.y < 21;
    }

    #region Bullet Pool

    private Dictionary<GameObject, Transform> m_bulletCacheRoot = new Dictionary<GameObject, Transform>();
    private readonly GameObjectPool m_gameObjectPool = new GameObjectPool(1000);

    private T CreateNewBulletGameObject<T>(Vector3 pos, T bullet) where T : BulletObjectBase
    {
        var prefab = bullet.bulletPrefab;
        var bulletGameObject = m_gameObjectPool.TryGetGameObject(prefab);
        // ������û�л������
        if (bulletGameObject == null)
        {
            bulletGameObject = Instantiate(prefab);
        }

        bullet.gameObject = bulletGameObject;
        var bulletTransform = bullet.gameObject.transform;
        bulletTransform.SetParent(GetOrNewBulletCacheRoot(prefab));
        bulletTransform.position = pos;
        bulletTransform.rotation = Quaternion.identity;

        bullet.Init();
        bulletGameObject.SetActive(true);
        return bullet;
    }

    private void RecycleBullet(BulletObjectBase bullet)
    {
        if (!m_activeBullets.Contains(bullet))
        {
            Debug.LogError("����������ӵ�����������Ķ���");
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
