using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameProperty
{
    public int attack;
    public int damageRate;
    public int criticalHitRate;
    public float fireRate;
    public int fireRateChange;
    public int range; 
    public float speed;
    public int speedChange;
    public int lucky;

    public void Add(GameProperty othProperty)
    {
        attack += othProperty.attack;
        damageRate += othProperty.damageRate;
        criticalHitRate += othProperty.criticalHitRate;
        fireRate += othProperty.fireRate;
        fireRateChange += othProperty.fireRateChange;
        range += othProperty.range;
        speed += othProperty.speed;
        speedChange += othProperty.speedChange;
        lucky += othProperty.lucky;
    }
}
