using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletOwner
{
    Player,
    Enemy
}

/// <summary>
/// �ӵ����࣬���߼�����
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
    /// �ӵ���ʼ��ִ��һ��
    /// ��������ͼƬ�������ȵ�
    /// </summary>
    public abstract void Init();

    /// <summary>
    /// ÿһ֡�ӵ��ĸ����߼�
    /// ��Update���£�������Ҫ��Time.detalTime
    /// </summary>
    public abstract void Update();
}
