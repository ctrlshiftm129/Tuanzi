using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����������
/// ͨ����λ�����Է��ʵ�ǰ�������ڵĹ���������
/// </summary>
public abstract class Manager : MonoBehaviour
{
    protected void Register2Locator()
    {
        ManagerLocator.Instance.RegisterManager(this);
    }
}
