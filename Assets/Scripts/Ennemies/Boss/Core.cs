using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : Boss
{
    [Header("Boss Specific")]

    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject gravityBulletPrefab;

    private int currentBullets;

    [SerializeField] private int maxBullets;
    [SerializeField] private int maxMissiles;


    public override void Start()
    {
        base.Start();
        currentMove = "idle";
        currentBullets = maxBullets;
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
                    if (currentMove.Equals("bullet"))
                    {
                        currentBullets = maxBullets;
                    }
                    else if (currentMove.Equals("missile"))
                    {
                        currentBullets = maxMissiles;
                    }
                }
                break;

            case "bullet":
                if (currentBullets > 0 && Time.time - lastActionTime > 0.2f)
                {
                    Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<Bullet>().Init((player.position - transform.position) / Vector2.Distance(player.position, transform.position));
                    GameAudio.PlaySFX("GunShot", transform.position);
                    currentBullets--;
                    lastActionTime = Time.time;
                }
                else if (currentBullets == 0)
                {
                    StopMove();
                }
                break;

            case "missile":
                if (currentBullets > 0 && Time.time - lastActionTime > 0.3f)
                {
                    Instantiate(missilePrefab, transform.position, Quaternion.identity).GetComponent<Missile>().Init(new Vector3(0, 1, 0));
                    GameAudio.PlaySFX("Missile", transform.position);
                    currentBullets--;
                    lastActionTime = Time.time;
                }
                else if (currentBullets == 0)
                {
                    StopMove();
                }
                break;

            case "gravityBullet":
                GameAudio.PlaySFX("GravityBullet", transform.position);
                Instantiate(gravityBulletPrefab, transform.position, Quaternion.identity).GetComponent<GravityBullet>().Init((player.position - transform.position) / Vector2.Distance(player.position, transform.position));
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
