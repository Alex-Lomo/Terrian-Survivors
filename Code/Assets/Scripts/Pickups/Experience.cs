using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experience : Pickup
{
    public float xpGranted;
    public override void Collect(PlayerStats target, float speed, float lifespan = 0f)
    {
        base.Collect(target,speed,lifespan);
        
    }

    public void OnDestroy()
    {
        if (!target) return;
        target.GainXp(xpGranted);
    }



}
