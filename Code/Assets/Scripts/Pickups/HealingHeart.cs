using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingHeart : Pickup
{
    public float healthRecover;
    public override void Collect(PlayerStats target, float speed, float lifespan = 0f)
    {
        base.Collect(target, speed, lifespan);

    }

    public void OnDestroy()
    {
        if (!target) return;
        target.RecoverHealth(healthRecover);
    }


}
