using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("General Informations")]
    [SerializeField] private int damage;
    [SerializeField] private GameObject prefabExplosion;

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
