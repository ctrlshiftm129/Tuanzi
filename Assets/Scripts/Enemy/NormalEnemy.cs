using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 普通怪物
/// 离得远了乱走，离得近的就追玩家，只会横着或者竖着走
/// </summary>
public class NormalEnemy : EnemyBase
{
    private const float TRACKING_DISTANCE = 8;
    private const float SPEED = 1;

    private Rigidbody2D m_rigidbody2D;

    public override void Init()
    {
        m_rigidbody2D = gameObject.GetComponent<Rigidbody2D>();        
    }

    public override void Update(Vector3 playerPos)
    {
        //var distance = Vector3.Distance(gameObject.transform.position, playerPos);
        TrackPlayer(playerPos);
    }

    private readonly int[,] AIM = new int[4, 2]
    {
        {-1, 0},
        {1, 0},
        {0, -1},
        {0, 1}
    };
    private void RandomMove()
    {
        var index = Random.Range(0, 4);
        var aimX = AIM[index, 0];
        var aimY = AIM[index, 1];
    }

    private void TrackPlayer(Vector3 playerPos)
    {
        var aim = (playerPos - gameObject.transform.position).normalized;
        m_rigidbody2D.velocity = aim * SPEED;

    }
}
