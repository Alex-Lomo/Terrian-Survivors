using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float lifespan = 5f;
    protected PlayerStats target;
    protected float speed;

    protected virtual void Update()
    {
        if(target)
        {
            Vector2 distance = target.transform.position - transform.position;
            if (distance.sqrMagnitude > speed * speed * Time.deltaTime)
            {
                transform.position += (Vector3)distance.normalized * speed * speed * Time.deltaTime;
            }
            else
                Destroy(gameObject);
        }
    }

    public virtual void Collect(PlayerStats target, float speed, float lifespan = 0f)
    {
        if(!this.target)
        {
            this.target = target;
            this.speed = speed;
            if (lifespan > 0) this.lifespan = lifespan;
            Destroy(gameObject, Mathf.Max(0.01f, this.lifespan));
        }

    }

}
