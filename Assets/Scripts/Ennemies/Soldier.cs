using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    [Header("General Informations")]
    public int corporalDamage;
    [SerializeField] private float distanceToSeePlayer;
    [SerializeField] private float speed;
    [SerializeField] private GameObject prefabBullet;
    [SerializeField] private float fireSpeed;
    [SerializeField] private float waitTime;
    [SerializeField] private float maxWalkTime;
    public int pointsReward;
    [SerializeField] private GameObject prefabHealth;

    private bool walkedToEndOfPlatform;
    private float chosenWalkTime;
    private bool waiting;
    private float startedActionAt;
    private bool playerInSight = false;
    private Transform player;
    private Vector2 vectorToPlayer;
    private float lastFireTime;
    private bool facingRight = false;


    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform gunBarrel;
    [SerializeField] private Transform feet;



    void Start()
    {
        walkedToEndOfPlatform = false;
        waiting = true;
        startedActionAt = Time.time;
        player = FindObjectOfType<PlayerMovement>().transform;
    }


    void FixedUpdate()
    {
        vectorToPlayer = (player.position - transform.position) / Vector2.Distance(player.position, transform.position);

        if ((facingRight && vectorToPlayer.x <= 0) ||
        (!facingRight && vectorToPlayer.x >= 0))
        {
            playerInSight = false;
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, vectorToPlayer, distanceToSeePlayer);
            playerInSight = hit && hit.transform.gameObject.tag.Equals("Player");
        }


        if (playerInSight)
        {
            if (Time.time - lastFireTime > fireSpeed)
            {
                lastFireTime = Time.time;
                Instantiate(prefabBullet, gunBarrel.position, Quaternion.identity).GetComponent<Bullet>().Init(vectorToPlayer);
                GameAudio.PlaySFX("GunShot", transform.position);
            }
        }
        else
        {
            if (waiting)
            {
                if (Time.time - startedActionAt > waitTime)
                {
                    waiting = false;
                    startedActionAt = Time.time;

                    bool lastFacing = facingRight;
                    facingRight = walkedToEndOfPlatform ? !facingRight : Random.Range(0, 2) == 0;
                    walkedToEndOfPlatform = false;

                    chosenWalkTime = Random.Range(0f, maxWalkTime);

                    if (facingRight != lastFacing)
                    {
                        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                    }
                    animator.SetBool("walk", true);
                }
            }
            else
            {
                walkedToEndOfPlatform = !Physics2D.Raycast(feet.position, -feet.up, 0.1f) || Physics2D.Raycast(feet.position, feet.right * (facingRight ? 1 : -1), 0.5f);
                rb.MovePosition(transform.position + transform.right * (facingRight ? 1 : -1) * speed * Time.fixedDeltaTime);
                if (walkedToEndOfPlatform ||
                Time.time - startedActionAt > chosenWalkTime)
                {
                    startedActionAt = Time.time;
                    waiting = true;
                    animator.SetBool("walk", false);
                }
            }
        }

        animator.SetBool("fire", playerInSight);
    }


    public void SetDeathAnimation()
    {
        if (Random.Range(0, 100) <= 30)
        {
            Instantiate(prefabHealth, transform.position, Quaternion.identity);
        }

        GameManager.instance.IncrementAchievement("STOMP");
        GameManager.instance.pointsInLevel += pointsReward;
        GameGUI.instance.UpdatePoints();
        animator.SetTrigger("dead");
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        rb.isKinematic = false;
        rb.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 50);
        Destroy(gameObject, 3f);
        enabled = false;
    }
}
