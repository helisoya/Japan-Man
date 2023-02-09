using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCollider_OnWeakness : MonoBehaviour
{
    [SerializeField] private Boss boss;

    void OnTriggerEnter2D(Collider2D col)
    {
        boss.AnalyseWeaknessCollision(col);
    }
}
