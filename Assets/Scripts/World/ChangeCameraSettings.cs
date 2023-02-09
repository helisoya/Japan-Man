using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraSettings : MonoBehaviour
{
    [SerializeField] private bool followPlayerX;
    [SerializeField] private bool followPlayerY;


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("Player"))
        {
            GameManager.instance.cameraFollowPlayerX = followPlayerX;
            GameManager.instance.cameraFollowPlayerY = followPlayerY;
        }
    }
}
