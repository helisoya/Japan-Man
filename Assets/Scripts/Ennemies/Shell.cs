using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [Header("General Informations")]
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    [SerializeField] private int damage;
    [SerializeField] private GameObject prefabExplosion;

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
        if (col.tag.Equals("Player") || 3 == col.gameObject.layer)
        {
            Destroy(Instantiate(prefabExplosion, transform.position, Quaternion.identity), 0.5f);
            if (col.tag.Equals("Player"))
            {
                col.GetComponent<PlayerMovement>().UpdateHealth(damage, transform);
            }

            GameAudio.PlaySFX("Explosion", transform.position);
            Destroy(gameObject);
        }
    }
}
