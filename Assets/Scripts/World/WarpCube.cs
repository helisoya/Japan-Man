using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpCube : MonoBehaviour
{

    [SerializeField] private Color greenColor;
    [SerializeField] private Color redColor;

    private bool isGreen = true;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D col;

    [SerializeField] private bool switchOnStart = false;

    void Start()
    {
        spriteRenderer.color = greenColor;
        if (switchOnStart) SwitchCube();
    }

    public void SwitchCube(bool value)
    {
        isGreen = value;
        spriteRenderer.color = isGreen ? greenColor : redColor;
        col.enabled = !isGreen;
    }

    public void SwitchCube()
    {
        SwitchCube(!isGreen);
    }
}
