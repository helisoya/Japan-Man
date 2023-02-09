using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBullet : MonoBehaviour
{
    [Header("General Informations")]
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    [SerializeField] private int damage;

    private Vector3 movementVector;

    private float timeOfCreation;


    public void Init(Vector3 vector)
    {
        timeOfCreation = Time.time;
        movementVector = vector;
        transform.right = (transform.position + vector) - transform.position;
    }



    void Update()
    {
        if (Time.time - timeOfCreation > lifeTime)
        {
            Destroy(gameObject);
        }

        transform.position += movementVector * Time.deltaTime * speed;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("Player"))
        {
            col.GetComponent<PlayerMovement>().UpdateHealth(damage, transform);
            Destroy(gameObject);
        }
    }
}
