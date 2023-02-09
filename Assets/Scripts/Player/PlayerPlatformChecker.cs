using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformChecker : MonoBehaviour
{
    [SerializeField] private Transform player;

    [SerializeField] private PlayerMovement move;
    private bool grounded;
    private bool check;

    void Update()
    {
        grounded = move.grounded;

        if (!grounded)
        {
            check = false;
            if (Input.GetAxis("Horizontal") != 0)
            {
                player.SetParent(null);
            }
        }

        if (!check)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 0.25f);
            if (hit.collider != null && hit.collider.CompareTag("MovingPlatform"))
            {
                player.SetParent(hit.transform);

                check = true;
            }
        }
    }
}
