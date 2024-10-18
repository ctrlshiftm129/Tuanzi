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
public abstract class BulletBase
{
    public BulletOwner owner;
    public GameObject gameObject;

    public BulletBase(BulletOwner owner)
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
