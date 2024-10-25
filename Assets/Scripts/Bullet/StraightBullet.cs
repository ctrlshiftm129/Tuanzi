using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightBullet : BulletObjectBase
{
    public StraightBullet(BulletOwner owner) : base(owner)
    {
        
    }

    public override void Init()
    {
        // 计算当前朝向和目标朝向之间的旋转角度
        float angle = Vector3.SignedAngle(Vector3.right, direction, Vector3.forward);
        gameObject.transform.Rotate(Vector3.forward, angle);
    }

    public override void Update()
    {
        var position = gameObject.transform.position;
        position += direction * speed * Time.deltaTime;
        gameObject.transform.position = position;
    }
}
