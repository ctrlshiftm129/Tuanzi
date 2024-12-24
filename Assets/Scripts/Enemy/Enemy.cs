using System;
using System.Collections;
using UnityEngine;

public class Enemy
{
    private static readonly int SetFixedColor = Shader.PropertyToID("_SetFixedColor");
    
    public readonly EnemyConfig enemyConfig;
    public GameObject gameObject;
    public int hp;
    public bool isAlive;
    
    public Animator animator;
    private Rigidbody2D m_rigidbody2D;
    private Material m_material;

    public Enemy(EnemyConfig enemyConfig)
    {
        this.enemyConfig = enemyConfig;
        hp = this.enemyConfig.hp;
        isAlive = true;
    }

    public void Init()
    {
        m_rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        m_material = gameObject.GetComponent<SpriteRenderer>().material;
        animator = gameObject.GetComponent<Animator>();
    }

    public float GetHpRatio()
    {
        return 1f * hp / enemyConfig.hp;
    }

    public IEnumerator ShowAffectFXCore()
    {
        m_material.SetInt(SetFixedColor, 1);
        yield return new WaitForSeconds(0.1f);
        m_material.SetInt(SetFixedColor, 0);
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
            case EnemyBehaviour.ChaseAndRest:
                ChaseAndRest(playerPos);
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

    private bool m_isRest;
    private float m_restTimer;
    /// <summary>
    /// 休息2s 冲刺2s
    /// </summary>
    private void ChaseAndRest(Vector3 playerPos)
    {
        m_restTimer += Time.deltaTime;

        var aim = (playerPos - gameObject.transform.position).normalized;
        if (m_isRest)
        {
            m_rigidbody2D.velocity = aim * 1;
        }
        else
        {
            m_rigidbody2D.velocity = aim * enemyConfig.speed;
        }
        
        if (m_restTimer >= 2)
        {
            m_isRest = !m_isRest;
            m_restTimer = 0;
        }
    }

    #endregion
}