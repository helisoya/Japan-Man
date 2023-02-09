using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmoredSoldier : Boss
{
    [Header("Boss Specific")]
    [SerializeField] private Transform feets;
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private float missileTime;
    [SerializeField] private LayerMask groundLayer;
    private int side;

    private int remainingMissiles;

    private bool lastWasMissile;

    public override void Start()
    {
        lastWasMissile = false;
        side = ((int)transform.localScale.x);
        base.Start();
        currentMove = "idle";
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
                    currentMove = lastWasMissile ? "dash" : GetRandomMove();
                    if (currentMove.Equals("dash"))
                    {
                        lastWasMissile = false;
                        animator.SetBool("dash", true);
                        GameAudio.PlaySFX("ArmoredSoldier_Dash", transform.position);
                    }
                    else if (currentMove.Equals("missile"))
                    {
                        lastWasMissile = true;
                        remainingMissiles = GameManager.instance.actualDifficulty + 1;
                    }

                }
                break;
            case "dash":
                rb.velocity = new Vector2(speed * side, 0);
                if (Physics2D.Raycast(feets.position + feets.up * 0.5f, feets.right * side, 1f, groundLayer))
                {
                    StopMove();
                }
                break;
            case "missile":
                if (remainingMissiles == 0)
                {
                    StopMove();
                }
                else if (Time.time - lastActionTime > missileTime)
                {
                    GameAudio.PlaySFX("Missile", transform.position);
                    Instantiate(missilePrefab, transform.position, Quaternion.identity).GetComponent<Missile>().Init(transform.up);
                    lastActionTime = Time.time;
                    remainingMissiles--;
                }
                break;
        }
    }

    public override void StopMove()
    {
        base.StopMove();
        animator.SetBool("dash", false);
    }

    public override void AnalyseBodyCollision(Collider2D col)
    {
        base.AnalyseBodyCollision(col);
    }

    public override void AnalyseWeaknessCollision(Collider2D col)
    {
        base.AnalyseWeaknessCollision(col);
        if (col.tag.Equals("Player") && health == maxHealth / 2 && !gaveDesperationPerk)
        {
            gaveDesperationPerk = true;
            possibleMoves.Add("missile");
        }
        if (health == 0)
        {
            StopMove();
        }
    }

}
