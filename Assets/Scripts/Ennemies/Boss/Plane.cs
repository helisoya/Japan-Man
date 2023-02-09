using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : Boss
{
    [Header("Boss Specific")]

    [SerializeField] private float timeBetweenBomb;
    [SerializeField] private GameObject bombPrefab;
    private int side;
    [SerializeField] private Transform bombSlot;
    [SerializeField] private BoxCollider2D availableSpace;

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
                    GameAudio.PlaySFX("Plane", transform.position);
                }
                break;
            case "move":
                rb.MovePosition(transform.position + transform.right * side * speed * Time.fixedDeltaTime);
                break;
            case "moveBomb":
                if (Time.time - lastActionTime > timeBetweenBomb)
                {
                    GameAudio.PlaySFX("Plane_DropBomb", transform.position);
                    lastActionTime = Time.time;
                    Instantiate(bombPrefab, bombSlot.position, Quaternion.identity);
                }
                rb.MovePosition(transform.position + transform.right * side * speed * Time.fixedDeltaTime);
                break;
        }
    }

    public override void AnalyseWeaknessCollision(Collider2D col)
    {
        base.AnalyseWeaknessCollision(col);
        if (col.tag.Equals("Player") && health == maxHealth / 2 && !gaveDesperationPerk)
        {
            gaveDesperationPerk = true;
            speed += speed / 3;
        }
        if (health == 0)
        {
            StopMove();
        }
    }

    public override void AnalyseBodyCollision(Collider2D col)
    {
        base.AnalyseBodyCollision(col);
        if ((col.gameObject.name.Equals("Bounds_Right") && side == 1) ||
        (col.gameObject.name.Equals("Bounds_Left") && side == -1))
        {
            ReverseSide();
            StopMove();
            rb.MovePosition(new Vector3(transform.position.x, Random.Range(availableSpace.bounds.min.y, availableSpace.bounds.max.y), transform.position.z));
        }
    }
}
