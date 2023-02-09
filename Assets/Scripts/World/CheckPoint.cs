using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!this.enabled) return;

        if (col.tag.Equals("Player"))
        {
            GameAudio.PlaySFX("CheckPoint", transform.position);
            GameManager.instance.EnableCheckpoint(transform.position);
            this.enabled = false;
        }
    }
}
