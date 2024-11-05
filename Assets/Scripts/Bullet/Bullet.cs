using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletOwner
{
    Player,
    Enemy
}

/// <summary>
/// 子弹基类，按逻辑分类
/// </summary>
public class Bullet
{
    public BulletOwner owner;
    public GameObject gameObject;
    public GameObject bulletPrefab;
    
    public Vector3 originalPos;
    public Vector3 direction;
    public float range;
    public float speed;
    public int damage;
    public bool penetrate;
    
    private Rigidbody2D m_rigidbody2D;

    public Bullet(BulletOwner owner)
    {
        this.owner = owner;
    }

    /// <summary>
    /// 子弹初始化执行一次
    /// 例如设置图片、参数等等
    /// </summary>
    public void Init()
    {
        m_rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        
        // 计算当前朝向和目标朝向之间的旋转角度
        var angle = Vector3.SignedAngle(Vector3.right, direction, Vector3.forward);
        gameObject.transform.Rotate(Vector3.forward, angle);
    }

    /// <summary>
    /// 每一帧子弹的更新逻辑
    /// </summary>
    public void FixedUpdate()
    {
        m_rigidbody2D.velocity = direction * speed;
    }
}
