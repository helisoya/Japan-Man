using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private int damage;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("Player"))
        {
            col.GetComponent<PlayerMovement>().UpdateHealth(damage, transform);
            GameAudio.PlaySFX("Hurt", transform.position);
        }
    }
}
