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
    private readonly Dictionary<GameObject, HashSet<GameObject>> m_checkSets = new Dictionary<GameObject, HashSet<GameObject>>();
    private readonly Dictionary<GameObject, Stack<GameObject>> m_pools = new Dictionary<GameObject, Stack<GameObject>>();

    public GameObjectPool(int poolSize)
    {
        this.poolSize = poolSize;
    }

    public GameObject TryGetGameObject(GameObject prefab)
    {
        var (checkSet, pool) = GetOrNewPrefabPool(prefab);

        GameObject gameObject = null;
        if (pool.Count > 0)
        {
            gameObject = pool.Pop();
            checkSet.Remove(gameObject);
        }

        return gameObject;
    }

    public void RecycleGameObject(GameObject prefab, GameObject gameObject)
    {
        var (checkSet, pool) = GetOrNewPrefabPool(prefab);

        if (checkSet.Contains(gameObject))
        {
            Debug.LogError("�ö����Ѿ��ڶ�����ˣ���Ҫ�ظ�����");
            return;
        }

        if (pool.Count < poolSize)
        {
            checkSet.Add(gameObject);
            pool.Push(gameObject);
        }
        else
        {
            Object.Destroy(gameObject);
        }
    }

    private (HashSet<GameObject>, Stack<GameObject>) GetOrNewPrefabPool(GameObject prefab)
    {
        if (!m_checkSets.TryGetValue(prefab, out var checkSet))
        {
            m_checkSets[prefab] = checkSet = new HashSet<GameObject>();
        }

        if (!m_pools.TryGetValue(prefab, out var pool))
        {
            m_pools[prefab] = pool = new Stack<GameObject>();
        }

        return (checkSet, pool);
    }
}