using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone : Boss
{
    [Header("Boss Specific")]
    private int side;
    [SerializeField] private Transform feets;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float jumpForce;


    public override void Start()
    {
        base.Start();
        animator.SetBool("ground", true);
        side = -((int)transform.localScale.x);
        currentMove = "idle";
    }

    protected void ReverseSide()
    {
        side *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    void FixedUpdate()
    {
        if (!activated) return;

        rb.velocity = new Vector2(side * speed, rb.velocity.y);
        bool grounded = Physics2D.Raycast(feets.position, -feets.up, 0.1f, groundLayer);

        if (Physics2D.Raycast(transform.position, transform.right * side, 1.5f, groundLayer))
        {
            ReverseSide();
        }

        if (Time.time - lastActionTime > idleTime && grounded)
        {
            lastActionTime = Time.time;
            animator.SetTrigger("jump");
            GameAudio.PlaySFX("Jump", transform.position);
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }

        animator.SetFloat("speed", 1);
        animator.SetBool("ground", grounded);

    }

    public override void AnalyseWeaknessCollision(Collider2D col)
    {
        base.AnalyseWeaknessCollision(col);
        if (col.tag.Equals("Player") && health == maxHealth / 2 && !gaveDesperationPerk)
        {
            gaveDesperationPerk = true;
            speed *= 2;
        }
        if (health == 0)
        {
            StopMove();
        }
    }

    public override void AnalyseBodyCollision(Collider2D col)
    {
        base.AnalyseBodyCollision(col);
    }


}
