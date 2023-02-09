using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCollider_StartBattle : MonoBehaviour
{
    [SerializeField] private Boss boss;

    [SerializeField] private Transform cameraPosition;

    [SerializeField] private bool lockX = true;
    [SerializeField] private bool lockY = true;


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("Player"))
        {
            FindObjectOfType<PlayerMovement>().StopMoving();
            GameManager.instance.cameraFollowPlayerX = !lockX;
            GameManager.instance.cameraFollowPlayerY = !lockY;
            CameraController.instance.MoveToSpot(cameraPosition.position);
            boss.StartBattle();
            Destroy(gameObject);
        }
    }
}
