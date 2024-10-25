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
public abstract class BulletObjectBase
{
    public BulletOwner owner;
    public GameObject gameObject;
    public GameObject bulletPrefab;
    public Vector3 originalPos;
    public Vector3 direction;
    public float range;
    public float speed;
    public bool penetrate;

    public BulletObjectBase(BulletOwner owner)
    {
        this.owner = owner;
    }

    /// <summary>
    /// 子弹初始化执行一次
    /// 例如设置图片、参数等等
    /// </summary>
    public abstract void Init();

    /// <summary>
    /// 每一帧子弹的更新逻辑
    /// 在Update更新，所以需要用Time.detalTime
    /// </summary>
    public abstract void Update();
}
