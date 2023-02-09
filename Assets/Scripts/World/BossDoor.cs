using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    [SerializeField] private Animator animator;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("Player"))
        {
            GameAudio.PlaySFX("BossDoor", transform.position);
            animator.SetTrigger("ChangeState");
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag.Equals("Player"))
        {
            GameAudio.PlaySFX("BossDoor", transform.position);
            animator.SetTrigger("ChangeState");
        }
    }
}
