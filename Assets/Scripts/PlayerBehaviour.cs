using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{

    [Header("Sprites")]
    public Sprite playerUp;
    public Sprite playerDown;
    public Sprite playerRight;
    public Sprite playerLeft;
    public SpriteRenderer spriteRenderer;

    [Header("Movement")]

    public float moveSpeed = 10f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //Movement
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        //Stop diagonal from being weird 
        moveInput.Normalize();

        // Change sprite based on movement
        if (moveInput.x > 0)        spriteRenderer.sprite = playerRight;
        else if (moveInput.x < 0)   spriteRenderer.sprite = playerLeft;
        else if (moveInput.y > 0)   spriteRenderer.sprite = playerUp;
        else if (moveInput.y < 0)   spriteRenderer.sprite = playerDown;
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput * moveSpeed;
    }
}
