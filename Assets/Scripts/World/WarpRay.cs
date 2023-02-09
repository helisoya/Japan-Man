using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpRay : MonoBehaviour
{
    [SerializeField] private bool isVertical = true;
    private int sideEntered;

    void SwitchAll()
    {
        GameAudio.PlaySFX("GravityShift", transform.position);
        WarpCube[] cubes = FindObjectsOfType<WarpCube>();
        foreach (WarpCube cube in cubes)
        {
            cube.SwitchCube();
        }
    }

    int DetermineEnteringPosition(Vector3 playerPos)
    {
        return isVertical ? (transform.position.x < playerPos.x ? 1 : -1) : (transform.position.y < playerPos.y ? 1 : -1);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("Player"))
        {
            sideEntered = DetermineEnteringPosition(col.transform.position);
            SwitchAll();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag.Equals("Player") &&
        DetermineEnteringPosition(col.transform.position) == sideEntered
        )
        {
            SwitchAll();
        }
    }
}
