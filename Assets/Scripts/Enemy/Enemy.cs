using System;
using UnityEngine;

public class Enemy
{
    public readonly EnemyConfig enemyConfig;
    public GameObject gameObject;
    public int hp;
    public bool isAlive;
    
    private Rigidbody2D m_rigidbody2D;

    public Enemy(EnemyConfig enemyConfig)
    {
        this.enemyConfig = enemyConfig;
        hp = this.enemyConfig.hp;
        isAlive = true;
    }

    public void Init()
    {
        m_rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    public void Update(Vector3 playerPos)
    {
        UpdateAlive();
        if (!isAlive) return;
        
        switch (enemyConfig.behaviour)
        {
            case EnemyBehaviour.Chase:
                Chase(playerPos);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UpdateAlive()
    {
        if (hp <= 0) isAlive = false;
    }

    #region Update By Behaviour

    private void Chase(Vector3 playerPos)
    {
        var aim = (playerPos - gameObject.transform.position).normalized;
        m_rigidbody2D.velocity = aim * enemyConfig.speed;

    }

    #endregion
}