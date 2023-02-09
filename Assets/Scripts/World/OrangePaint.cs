using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangePaint : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag.Equals("Player")) col.gameObject.GetComponent<PlayerMovement>().SetSpeedBonus(true);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag.Equals("Player")) col.gameObject.GetComponent<PlayerMovement>().SetSpeedBonus(false);
    }
}
