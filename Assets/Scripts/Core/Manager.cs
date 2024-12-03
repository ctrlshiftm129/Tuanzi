using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理器基类
/// 通过定位器可以访问当前场景存在的管理器对象
/// </summary>
public abstract class Manager : MonoBehaviour
{
    protected void Register2Locator()
    {
        ManagerLocator.Instance.RegisterManager(this);
    }
}
