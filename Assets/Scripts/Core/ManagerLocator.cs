using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 获取场景内指定管理器
/// </summary>
public class ManagerLocator
{
    public static ManagerLocator Instance { get; } = new ManagerLocator();

    private readonly Dictionary<Type, Manager> m_managers = new Dictionary<Type, Manager>();

    public void RegisterManager(Manager manager)
    {
        Debug.Log(manager.GetType());
        m_managers.Add(manager.GetType(), manager);
    }

    public T Get<T>() where T : Manager
    {
        return (T)m_managers.GetValueOrDefault(typeof(T), null);
    }

    private ManagerLocator()
    {

    }
}
