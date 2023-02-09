using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [SerializeField] private float force;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("Player"))
        {
            GameAudio.PlaySFX("Spring", transform.position);
            col.GetComponent<PlayerMovement>().ForceJump(force);
        }
    }
}
