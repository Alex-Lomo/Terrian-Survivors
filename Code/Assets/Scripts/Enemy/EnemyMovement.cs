using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    EnemyStats enemy;
    Transform player;
    public float LastHoriz;
    Vector2 knockbackVelocity;
    float knockbackDuration;
    

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().transform;
        enemy = FindObjectOfType<EnemyStats>();
        
    }

    void Update()
    {   
        if(knockbackDuration>0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
            LastHoriz = knockbackVelocity.x;
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemy.currentMoveSpeed * Time.deltaTime);
            LastHoriz = (player.transform.position - transform.position).x;
        }

        
    }

    public void Knockback(Vector2 velocity, float duration)
    {
        if(knockbackDuration>0)
        {
            knockbackVelocity = velocity;
            knockbackDuration = duration;
        }
    }
}
