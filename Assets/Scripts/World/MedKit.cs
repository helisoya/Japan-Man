using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedKit : MonoBehaviour
{

    [SerializeField] private int healthAmount;

    void Awake()
    {
        if (GameManager.instance.save.difficulty == 0)
        {
            healthAmount *= 2;
        }
        else if (GameManager.instance.save.difficulty == 0)
        {
            healthAmount /= 2;
        }
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("Player"))
        {
            GameAudio.PlaySFX("Heal", transform.position);
            col.GetComponent<PlayerMovement>().UpdateHealth(-healthAmount, transform);
            Destroy(gameObject);
        }
    }
}
