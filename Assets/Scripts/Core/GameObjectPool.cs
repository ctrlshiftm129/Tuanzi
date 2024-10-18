using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameObject�����
/// �����ȡ����GameObject���ᴥ��Awake����ʼ����Enable
/// һ����ҪAwake�Ĳ�Ҫ�����
/// </summary>
public class GameObjectPool
{
    //��������ش�С�Ժ�����ж��
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
            Debug.LogError("�ö����Ѿ��ڶ�����ˣ���Ҫ�ظ�����");
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