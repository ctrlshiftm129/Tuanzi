using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase
{
    public GameObject gameObject;
    public bool isAlive;

    public abstract void Init();

    public abstract void Update(Vector3 playerPos);
}
