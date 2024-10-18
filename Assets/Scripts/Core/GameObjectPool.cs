using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameObject对象池
/// 对象池取出的GameObject不会触发Awake，初始化用Enable
/// 一定需要Awake的不要用这个
/// </summary>
public class GameObjectPool
{
    //超过对象池大小以后正常卸载
    public int poolSize;
    private readonly HashSet<GameObject> m_checkSet = new HashSet<GameObject>();
    private readonly Stack<GameObject> m_gameObjects = new Stack<GameObject>();

    public GameObjectPool(int poolSize)
    {
        this.poolSize = poolSize;
    }

    public GameObject TryGetGameObject()
    {
        GameObject gameObject = null;
        if (m_gameObjects.Count > 0)
        {
            gameObject = m_gameObjects.Pop();
            m_checkSet.Remove(gameObject);
        }

        return gameObject;
    }

    public void RecycleGameObject(GameObject gameObject)
    {
        if (m_checkSet.Contains(gameObject))
        {
            Debug.LogError("该对象已经在对象池了，不要重复回收");
            return;
        }

        if (m_gameObjects.Count < poolSize)
        {
            m_checkSet.Add(gameObject);
            m_gameObjects.Push(gameObject);
        }
        else
        {
            Object.Destroy(gameObject);
        }
    }
}