using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightBullet : BulletBase
{
    public Vector3 direction;
    public float speed;

    public StraightBullet(BulletOwner owner, Vector2 direction, float speed) : base(owner)
    {
        this.direction = direction;
        this.speed = speed;
    }

    public override void Init()
    {
        
    }

    public override void Update()
    {
        gameObject.transform.Translate(direction * speed * Time.deltaTime);
    }
}
