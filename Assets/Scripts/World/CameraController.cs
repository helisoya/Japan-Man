using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public static CameraController instance;
    [SerializeField] private Transform player;
    [SerializeField] private float interpolation;

    private Coroutine movingToSpot;

    void Start()
    {
        instance = this;
        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
    }

    public void MoveToSpot(Vector2 spot)
    {
        if (movingToSpot != null)
        {
            StopCoroutine(movingToSpot);
        }

        movingToSpot = StartCoroutine(CR_MovingToSpot(spot));
    }

    IEnumerator CR_MovingToSpot(Vector2 spot)
    {
        while (transform.position.x != spot.x && transform.position.y != spot.y)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(
                spot.x,
                spot.y,
                transform.position.z),
                10 * Time.deltaTime
                );
            yield return new WaitForEndOfFrame();
        }

        movingToSpot = null;
    }

    void Update()
    {

        if (movingToSpot != null) return;

        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(
            GameManager.instance.cameraFollowPlayerX ? player.position.x : transform.position.x,
            GameManager.instance.cameraFollowPlayerY ? player.position.y : transform.position.y,
            transform.position.z),
            interpolation
            );
    }
}
