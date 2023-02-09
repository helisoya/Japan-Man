using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackSoldier : Boss
{
    [Header("Boss Specific")]

    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private GameObject bulletPrefab;
    private int side;
    private Vector2 targetPosition;
    [SerializeField] private BoxCollider2D arenaBounds;
    [SerializeField] private Transform barrel;


    public override void Start()
    {
        base.Start();
        side = ((int)transform.localScale.x);
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
                    lastActionTime = Time.time;
                    currentMove = GetRandomMove();

                    if (currentMove.Equals("move"))
                    {
                        targetPosition = new Vector2(
                            Random.Range(arenaBounds.bounds.center.x - arenaBounds.size.x / 2, arenaBounds.bounds.center.x + arenaBounds.size.x / 2),
                            Random.Range(arenaBounds.bounds.center.y - arenaBounds.size.y / 2, arenaBounds.bounds.center.y + arenaBounds.size.y / 2)
                        );
                        if ((rb.position.x < targetPosition.x && side == 1) || (rb.position.x > targetPosition.x && side == -1))
                        {
                            side *= -1;
                            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                        }
                    }
                    else
                    {
                        if ((rb.position.x < player.position.x && side == 1) || (rb.position.x > player.position.x && side == -1))
                        {
                            side *= -1;
                            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                        }
                    }
                }
                break;
            case "move":
                if (rb.position != targetPosition)
                {
                    rb.MovePosition(Vector2.MoveTowards(rb.position, targetPosition, speed * Time.fixedDeltaTime));
                }
                else
                {
                    StopMove();
                }
                break;

            case "triShot":
                GameAudio.PlaySFX("GunShot", transform.position);
                animator.SetTrigger("shoot");
                Instantiate(bulletPrefab, barrel.position, Quaternion.identity).GetComponent<Bullet>().Init(barrel.right * side * -1);
                Instantiate(bulletPrefab, barrel.position, Quaternion.identity).GetComponent<Bullet>().Init(barrel.up + barrel.right * side * -1);
                Instantiate(bulletPrefab, barrel.position, Quaternion.identity).GetComponent<Bullet>().Init(-barrel.up + barrel.right * side * -1);
                StopMove();
                break;

            case "missile":
                GameAudio.PlaySFX("Missile", transform.position);
                animator.SetTrigger("shoot");
                Instantiate(missilePrefab, barrel.position, Quaternion.identity).GetComponent<Missile>().Init(barrel.right * side * -1);
                StopMove();
                break;
        }
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
