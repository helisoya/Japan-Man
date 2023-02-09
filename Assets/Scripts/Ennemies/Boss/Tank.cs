using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Boss
{
    [Header("Boss Specific")]

    private bool weaknessExposed;
    [SerializeField] private LayerMask groundLayer;
    private int side;
    [SerializeField] private int maxBullets;
    private int currentBullet;
    [SerializeField] private Transform bulletBarrel;
    [SerializeField] private Transform shellBarrel;
    [SerializeField] private GameObject shellPrefab;
    [SerializeField] private GameObject bulletPrefab;

    private int numberMovesNoWeakness;

    public override void Start()
    {
        numberMovesNoWeakness = 0;
        currentBullet = maxBullets;
        side = -((int)transform.localScale.x);
        base.Start();
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
                    int lastSide = side;
                    side = player.position.x >= transform.position.x ? 1 : -1;
                    if (side != lastSide)
                    {
                        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                    }

                    lastActionTime = Time.time;

                    currentMove = numberMovesNoWeakness <= 3 ? GetRandomMove() : "openWeakness";

                    if (currentMove.Equals("openWeakness"))
                    {
                        numberMovesNoWeakness = 0;
                        GameAudio.PlaySFX("Tank_Hatch", transform.position);
                        animator.SetTrigger("opening");
                        weaknessExposed = true;
                    }
                    else
                    {
                        numberMovesNoWeakness++;
                    }
                    if (currentMove.Equals("move"))
                    {
                        GameAudio.PlaySFX("Tank_Move", transform.position);
                        animator.SetBool("move", true);
                    }
                }
                break;
            case "bullet":
                if (currentBullet > 0 && Time.time - lastActionTime > 0.2f)
                {
                    Instantiate(bulletPrefab, bulletBarrel.position, Quaternion.identity).GetComponent<Bullet>().Init((player.position - transform.position) / Vector2.Distance(player.position, transform.position));
                    GameAudio.PlaySFX("GunShot", transform.position);
                    currentBullet--;
                    lastActionTime = Time.time;
                }
                else if (currentBullet == 0)
                {
                    currentBullet = maxBullets;
                    StopMove();
                }
                break;
            case "shell":
                GameAudio.PlaySFX("Tank_Fire", transform.position);
                Instantiate(shellPrefab, shellBarrel.position, Quaternion.identity).GetComponent<Shell>().Init(transform.right * side);
                StopMove();
                break;
            case "move":
                rb.velocity = new Vector2(speed * side, 0);
                if (Physics2D.Raycast(transform.position, transform.right * side, 4f, groundLayer))
                {
                    StopMove();
                    ReverseSide();
                }
                break;
            case "closeWeakness":
                if (Time.time - lastActionTime > idleTime)
                {

                    StopMove();
                    weaknessExposed = false;
                }
                break;
            case "openWeakness":
                if (Time.time - lastActionTime > idleTime * 2)
                {
                    currentMove = "closeWeakness";
                    animator.SetTrigger("opening");
                    GameAudio.PlaySFX("Tank_Hatch", transform.position);
                }
                break;
        }
    }

    public override void StopMove()
    {
        base.StopMove();
        animator.SetBool("move", false);
    }

    public override void AnalyseBodyCollision(Collider2D col)
    {
        base.AnalyseBodyCollision(col);
    }

    public override void AnalyseWeaknessCollision(Collider2D col)
    {
        if (!weaknessExposed) return;
        base.AnalyseWeaknessCollision(col);
        if (col.tag.Equals("Player") && health == maxHealth / 2 && !gaveDesperationPerk)
        {
            gaveDesperationPerk = true;
        }
        if (health == 0)
        {
            StopMove();
        }
    }

}
