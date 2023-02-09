using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("General Informations")]
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int corporalDamage;
    [SerializeField] protected int speed;
    public int pointsReward;
    [SerializeField] protected string takeDamageSound;
    [SerializeField] protected string dealDamageSound;
    [SerializeField] protected string deathSound;
    [SerializeField] protected float idleTime;
    [SerializeField] protected List<string> possibleMoves;



    [Header("Components")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected GameObject prefabExplosion;


    protected Transform player;
    protected int health;
    protected bool activated;
    protected bool invincible;
    protected Coroutine flashing;
    protected string currentMove;
    protected float lastActionTime;
    protected Color defaultColor;
    protected bool gaveDesperationPerk = false;

    public string GetRandomMove()
    {
        return possibleMoves[Random.Range(0, possibleMoves.Count)];
    }


    public virtual void Start()
    {
        defaultColor = spriteRenderer.color;
        invincible = false;
        activated = false;
        player = FindObjectOfType<PlayerMovement>().transform;
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>());



        if (GameManager.instance.actualDifficulty == 0)
        {
            maxHealth /= 2;
        }
        else if (GameManager.instance.actualDifficulty == 2)
        {
            maxHealth *= 2;
        }
    }

    public void StartBattle()
    {
        health = maxHealth;
        GameGUI.instance.SetBossBarActive(true);
        StartCoroutine(CR_StartCutscene());
    }

    IEnumerator CR_StartCutscene()
    {
        GameAudio.PlayBGM("Demo");
        GameManager.instance.inCutscene = true;
        float fakeHealth = 0;

        while (fakeHealth != (float)maxHealth)
        {
            fakeHealth = Mathf.Clamp(fakeHealth + Time.deltaTime * 5, 0, maxHealth);
            GameGUI.instance.UpdateBossHealth(fakeHealth, maxHealth);
            yield return new WaitForEndOfFrame();
        }

        activated = true;
        GameManager.instance.inCutscene = false;
        GameAudio.PlayBGM("Boss");
    }

    IEnumerator CR_Flashing()
    {

        invincible = true;
        for (int i = 0; i < 4; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = defaultColor;
            yield return new WaitForSeconds(0.15f);
        }
        invincible = false;
    }

    IEnumerator CR_Death()
    {
        GameAudio.PlayBGM(null);
        GameManager.instance.pointsInLevel += pointsReward;
        GameGUI.instance.UpdatePoints();
        FindObjectOfType<PlayerMovement>().StopMoving();
        GameManager.instance.inCutscene = true;

        for (int i = 0; i < 4; i++)
        {
            spriteRenderer.color = defaultColor;
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = Color.clear;
            yield return new WaitForSeconds(0.15f);
        }
        GameGUI.instance.FinishLevel();
    }

    public virtual void StopMove()
    {
        currentMove = "idle";
        lastActionTime = Time.time;
        rb.velocity = Vector2.zero;
    }

    public virtual void StopAllActions()
    {
        activated = false;
    }

    public virtual void AnalyseWeaknessCollision(Collider2D col)
    {
        if (!col.tag.Equals("Player")) return;

        player.GetComponent<PlayerMovement>().ForceJump(6);

        if (health == 0 || invincible) return;



        if (!invincible && !col.transform.GetComponent<PlayerMovement>().invincible)
        {
            health--;
            GameGUI.instance.UpdateBossHealth(health, maxHealth);
            // Update Health On GUI
            if (health == 0)
            {
                GameAudio.PlaySFX(deathSound, transform.position);
                StopAllActions();
                rb.isKinematic = true;
                foreach (Collider2D childCollider in GetComponentsInChildren<Collider2D>())
                {
                    childCollider.enabled = false;
                }
                Destroy(Instantiate(prefabExplosion, transform.position, Quaternion.identity), 0.5f);
                StartCoroutine(CR_Death());
            }
            else
            {
                GameAudio.PlaySFX(takeDamageSound, transform.position);
                if (flashing != null)
                {
                    StopCoroutine(flashing);
                }
                flashing = StartCoroutine(CR_Flashing());
            }
        }
    }

    public virtual void AnalyseBodyCollision(Collider2D col)
    {
        if (col.tag.Equals("Player"))
        {
            GameAudio.PlaySFX(dealDamageSound, transform.position);
            col.transform.GetComponent<PlayerMovement>().UpdateHealth(corporalDamage, transform);
        }
    }
}
