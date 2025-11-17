using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public const float DEFAULT_MOVESPEED = 6f;
    
    Rigidbody2D rb;
    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public float LastHoriz;
    [HideInInspector]
    public float LastVert;
    [HideInInspector]
    public Vector2 lastMovedVector;
    PlayerStats player;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(-1, 0f);
        player = FindObjectOfType<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        InputManagement();
    }

    void FixedUpdate()
    {
        Move();
    }

    void InputManagement()
    {
        if (GameManager.instance.isGameOver == true) return;

        float MoveX = Input.GetAxisRaw("Horizontal");
        float MoveY = Input.GetAxisRaw("Vertical");
        moveDir = new Vector2(MoveX, MoveY).normalized;
        if (moveDir.x != 0)
        {
            LastHoriz = moveDir.x;
            lastMovedVector = new Vector2(LastHoriz, 0f);    
        }

        if (moveDir.y != 0)
        {
            LastVert = moveDir.y;
            lastMovedVector = new Vector2(0f, LastVert);  
        }

        if (moveDir.x != 0 && moveDir.y != 0)
        {
            lastMovedVector = new Vector2(LastHoriz, LastVert);    
        }
    }

    void Move()
    {
        if (GameManager.instance.isGameOver == true) return;

        rb.velocity = moveDir * DEFAULT_MOVESPEED * player.Stats.moveSpeed;
    }
}
