using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private int currentTarget;

    [SerializeField] private Transform[] waypoints;

    [SerializeField] private int speed;

    [SerializeField] private Transform platform;

    void Start()
    {
        currentTarget = 0;
    }

    void Update()
    {
        platform.position = Vector2.MoveTowards(platform.position, waypoints[currentTarget].position, speed * Time.deltaTime);
        if (platform.position.Equals(waypoints[currentTarget].position))
        {
            currentTarget = (currentTarget + 1 + waypoints.Length) % waypoints.Length;
        }
    }
}
