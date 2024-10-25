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
        // ���㵱ǰ�����Ŀ�곯��֮�����ת�Ƕ�
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
