using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWarpRay : MonoBehaviour
{
    private float maxSize;
    private bool goToZero = true;
    [SerializeField] private float speed;

    private bool moveRight = true;

    [SerializeField] private float maxDistance;

    private float minPosY;

    void Start()
    {
        maxSize = transform.localScale.y;
        minPosY = transform.position.y;
    }

    void Update()
    {

        transform.localScale = new Vector3(
            transform.localScale.x,
        Mathf.Clamp(transform.localScale.y + Time.deltaTime * speed * (goToZero ? -1 : 1), 0, maxSize),
        transform.localScale.z);

        if (transform.localScale.y == 0 || transform.localScale.y == maxSize) goToZero = !goToZero;

        transform.position += new Vector3(0, 1, 0) * speed * Time.deltaTime * (moveRight ? 1 : -1);
        if (transform.position.y >= minPosY + maxDistance || transform.position.y <= minPosY) moveRight = !moveRight;
    }
}
