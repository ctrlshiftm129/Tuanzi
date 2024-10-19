using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 暂时靠这个跑游戏逻辑
/// </summary>
public class GameManager : MonoBehaviour
{
    public GameObject player;

    private ManagerLocator m_locator;
    private BulletManager m_bulletManager;
    private EnemyManager m_enemyManager;

    // Start is called before the first frame update
    void Start()
    {
        m_locator = ManagerLocator.Instance;
        m_bulletManager = m_locator.Get<BulletManager>();
        m_enemyManager = m_locator.Get<EnemyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        m_bulletManager.UpdateAllActiveBullets();
        m_enemyManager.UpdateAllActiveEnemy(player.transform.position);
        AddNewEnemey();
    }

    private int count = 100;
    private float time = 0.5f;

    private void AddNewEnemey()
    {
        if (count == 0) return;
        time -= Time.deltaTime;
        if (time < 0)
        {
            --count;
            time += 0.5f;
            var random = Random.Range(0, 4);
            if (random == 0)
            {
                var randPos = Random.Range(2.5f, 16.5f);
                m_enemyManager.AddEnemy(new Vector3(randPos, 2.5f, 0));
            }
            else if(random == 1)
            {
                var randPos = Random.Range(2.5f, 16.5f);
                m_enemyManager.AddEnemy(new Vector3(17.5f, randPos, 0));
            }
            else if(random == 2)
            {
                var randPos = Random.Range(3.5f, 17.5f);
                m_enemyManager.AddEnemy(new Vector3(randPos, 17.5f, 0));
            }
            else if(random == 3)
            {
                var randPos = Random.Range(3.5f, 17.5f);
                m_enemyManager.AddEnemy(new Vector3(2.5f, randPos, 0));
            }
        }
    }
}
