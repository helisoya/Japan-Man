using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierCollider_TakeDamage : MonoBehaviour
{
    [SerializeField] private Soldier soldier;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag.Equals("Player"))
        {
            GameAudio.PlaySFX("Hurt", transform.position);
            soldier.SetDeathAnimation();
        }
    }
}
