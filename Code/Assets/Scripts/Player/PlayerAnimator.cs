using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    Animator am;
    PlayerMovement pm;
    SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        am = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
    }

    
    void Update()
    {
        if(pm.moveDir.x!=0 || pm.moveDir.y!=0)
        {
            am.SetBool("Move", true);
            FlipSprite();
        }
        else
        {
            am.SetBool("Move", false);
        }
    }

    void FlipSprite()
    {
        if(pm.LastHoriz>0)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
    }
}
