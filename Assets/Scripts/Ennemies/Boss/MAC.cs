using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAC : Boss
{
    [Header("Boss Specific")]

    [SerializeField] private GameObject gravityBulletPrefab;
    [SerializeField] private GameObject bulletPrefab;
    private int side;
    [SerializeField] private Transform barrel;
    [SerializeField] private Transform feets;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int maxBullets;
    private int currentBullets;

    // 0 = Jump
    // 1 = Fall
    private int jumpPhase;

    [SerializeField] private float jumpForce;


    public override void Start()
    {
        base.Start();
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
        switch (currentMove)
        {
            case "idle":
                if (Time.time - lastActionTime > idleTime)
                {
                    lastActionTime = Time.time;
                    currentMove = GetRandomMove();

                    if (currentMove.Equals("shoot") || currentMove.Equals("gravityShoot"))
                    {
                        if ((side == -1 && player.position.x > transform.position.x) ||
                        (side == 1 && player.position.x < transform.position.x))
                        {
                            ReverseSide();
                        }
                        animator.SetBool("shoot", true);
                        currentBullets = currentMove.Equals("shoot") ? maxBullets : 1;
                    }
                    else if (currentMove.Equals("jump") || currentMove.Equals("reverseGravity"))
                    {
                        GameAudio.PlaySFX("Jump", transform.position);
                        rb.velocity = new Vector2(Random.Range(-speed, speed), jumpForce * rb.gravityScale);
                        jumpPhase = 0;
                        animator.SetTrigger("jump");
                        if (Random.Range(0, 2) == 0)
                        {
                            ReverseSide();
                        }

                        if ((rb.velocity.x < 0 && Physics2D.Raycast(transform.position, -transform.right, 1f, groundLayer)) ||
                        (rb.velocity.x > 0 && Physics2D.Raycast(transform.position, transform.right, 1f, groundLayer)))
                        {
                            rb.velocity = new Vector2(0, rb.velocity.y);
                        }
                    }
                }
                break;
            case "shoot":
                if (currentBullets > 0 && Time.time - lastActionTime > 0.2f)
                {
                    Instantiate(bulletPrefab, barrel.position, Quaternion.identity).GetComponent<Bullet>().Init((player.position - transform.position) / Vector2.Distance(player.position, transform.position));
                    GameAudio.PlaySFX("GunShot", transform.position);
                    currentBullets--;
                    lastActionTime = Time.time;
                }
                else if (currentBullets == 0)
                {
                    animator.SetBool("shoot", false);
                    StopMove();
                }
                break;
            case "gravityShoot":
                if (currentBullets > 0 && Time.time - lastActionTime > 0.2f)
                {
                    GameAudio.PlaySFX("GravityBullet", transform.position);
                    Instantiate(gravityBulletPrefab, barrel.position, Quaternion.identity).GetComponent<GravityBullet>().Init((player.position - transform.position) / Vector2.Distance(player.position, transform.position));
                    currentBullets--;
                    lastActionTime = Time.time;
                }
                else if (currentBullets == 0 && Time.time - lastActionTime > 0.2f)
                {
                    animator.SetBool("shoot", false);
                    StopMove();
                }
                break;
            case "reverseGravity":
                if (rb.velocity.x != 0 && (Physics2D.Raycast(transform.position, transform.right, 1f, groundLayer) || Physics2D.Raycast(transform.position, -transform.right, 1f, groundLayer)))
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }

                if (jumpPhase == 0) // Jump
                {
                    if (Time.time - lastActionTime > 0.3f)
                    {
                        GameAudio.PlaySFX("GravityShift", transform.position);
                        player.GetComponent<PlayerMovement>().ReverseGravity();
                        lastActionTime = Time.time;
                        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * -1, transform.localScale.z);
                        rb.gravityScale *= -1;
                        animator.SetTrigger("fall");
                        jumpPhase = 1;
                    }
                }
                else // Fall
                {
                    if (Physics2D.Raycast(feets.position, -feets.up * rb.gravityScale, 0.01f, groundLayer))
                    {
                        rb.velocity = Vector2.zero;
                        animator.SetTrigger("ground");
                        StopMove();
                    }
                }
                break;

            case "jump":

                if (rb.velocity.x != 0 && (Physics2D.Raycast(transform.position, transform.right, 1f, groundLayer) || Physics2D.Raycast(transform.position, -transform.right, 1f, groundLayer)))
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }

                if (jumpPhase == 0) // Jump
                {
                    if (Time.time - lastActionTime > 0.3f)
                    {
                        lastActionTime = Time.time;
                        animator.SetTrigger("fall");
                        jumpPhase = 1;
                    }
                }
                else // Fall
                {
                    if (Physics2D.Raycast(feets.position, -feets.up * rb.gravityScale, 0.01f, groundLayer))
                    {
                        rb.velocity = Vector2.zero;
                        animator.SetTrigger("ground");
                        StopMove();
                    }
                }
                break;
        }
    }

    public override void AnalyseWeaknessCollision(Collider2D col)
    {
        base.AnalyseWeaknessCollision(col);
        if (col.tag.Equals("Player") && health == maxHealth / 2 && !gaveDesperationPerk)
        {
            gaveDesperationPerk = true;
            possibleMoves.Add("gravityShoot");
        }
        if (health == 0)
        {
            StopMove();
        }
    }


}
