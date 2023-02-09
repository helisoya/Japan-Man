using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityShifter : MonoBehaviour
{

    private float lastActiveTime;
    [SerializeField] private float waitTime = 2;

    void Start()
    {
        lastActiveTime = Time.time;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("Player") && Time.time - lastActiveTime >= waitTime)
        {
            lastActiveTime = Time.time;
            GameAudio.PlaySFX("GravityShift", transform.position);
            col.GetComponent<PlayerMovement>().ReverseGravity();
        }
    }

}
