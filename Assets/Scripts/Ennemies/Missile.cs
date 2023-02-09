using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    private Vector3 moveVector;

    [Header("General Informations")]
    [SerializeField] private int speed;
    [SerializeField] private int damage;
    [SerializeField] private float timeForPhase1;
    [SerializeField] private float timeForPhase2;
    [SerializeField] private GameObject prefabExplosion;
    private float currentTime;
    private int currentPhase;

    private Transform player;

    // 1 = Move Along Initial Vector
    // 2 = Follow Player
    // 3 = Follow Last Vector

    void Update()
    {
        transform.position += Time.deltaTime * speed * moveVector;

        if (currentPhase == 1 && Time.time - currentTime > timeForPhase1)
        {
            currentPhase++;
        }
        else if (currentPhase == 2)
        {
            if (Time.time - currentTime > timeForPhase2)
            {
                currentPhase++;
                Destroy(gameObject, 10);
            }
            moveVector = (player.position - transform.position) / Vector2.Distance(player.position, transform.position);
            RefreshRotation();
        }
    }

    void RefreshRotation()
    {
        transform.up = (transform.position + moveVector) - transform.position;
    }


    public void Init(Vector3 initialVector)
    {
        player = FindObjectOfType<PlayerMovement>().transform;
        currentTime = Time.time;
        currentPhase = 1;
        moveVector = initialVector;
        RefreshRotation();
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("Player"))
        {
            GameAudio.PlaySFX("Bullet_Impact", transform.position);
            col.GetComponent<PlayerMovement>().UpdateHealth(damage, transform);
            Destroy(Instantiate(prefabExplosion, transform.position, Quaternion.identity), 0.5f);
            Destroy(gameObject);
        }
    }
}
