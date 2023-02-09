using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Paralax : MonoBehaviour
{
    [Header("General Informations")]
    [SerializeField] private float parallaxEffect;

    private float length;
    private float startpos;

    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float temp = (Camera.main.transform.position.x * (1 - parallaxEffect));
        float dist = (Camera.main.transform.position.x * parallaxEffect);

        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        if (temp > startpos + length) startpos += length;
        if (temp < startpos - length) startpos -= length;
    }

}
