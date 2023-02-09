using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{


    [Header("General Informations")]
    [SerializeField] private float maxSpeed = 10;
    [SerializeField] private float jumpForce = 800;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int maxJumps;
    [SerializeField] private int maxHealth;
    [SerializeField] private float maxStamina;
    [SerializeField] private float staminaDrainSpeed;
    [SerializeField] private float staminaRegainSpeed;


    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform feetPosition;
    [SerializeField] private SpriteRenderer spriteRenderer;


    private bool facingRight = false;
    private float xInput;
    private bool jumpPressed;
    private bool _grounded;
    private bool dead;
    private int currentJumps;
    private int health;
    [HideInInspector] public bool invincible;
    private Coroutine flashing;
    private float stamina;
    private bool inSlowMotion;
    private Coroutine changingTime;

    private Vector3 velocity = Vector3.zero;

    private bool canMove;

    private bool onIce;

    public bool grounded
    {
        get { return _grounded; }
    }

    private Vector3 m_Velocity = Vector3.zero;

    private int android_MoveLeft = 0;
    private int android_MoveRight = 0;

    void Awake()
    {
        canMove = true;
        currentJumps = 0;
        dead = false;
        facingRight = transform.localScale.x == -1;
        invincible = false;
        inSlowMotion = false;

    }

    void Start()
    {


        if (!GameManager.instance.inBossRush)
        {
            if (GameManager.instance.useCheckpoint)
            {
                transform.position = GameManager.instance.checkPointPosition;
                Camera.main.transform.position = transform.position + new Vector3(0, 0, -10);
            }

            if (GameManager.instance.save.difficulty == 0)
            {
                maxHealth *= 2;
            }
            else if (GameManager.instance.save.difficulty == 2)
            {
                maxHealth /= 2;
            }

            if (GameManager.instance.save.upgradeHigherJump) jumpForce += 2;
            if (GameManager.instance.save.upgradeHP) maxHealth *= 2;
            if (GameManager.instance.save.upgradeStamina) maxStamina *= 2;
            if (GameManager.instance.save.upgradeMoreJumps) maxJumps++;
            if (GameManager.instance.save.upgradeSpeed) maxSpeed += 2;
        }
        else
        {
            if (GameManager.instance.bossRushDifficulty == 0)
            {
                maxHealth *= 2;
            }
            else if (GameManager.instance.bossRushDifficulty == 2)
            {
                maxHealth /= 2;
            }
        }

        health = maxHealth;
        stamina = maxStamina;
    }

    public void SetSpeedBonus(bool value)
    {
        maxSpeed += value ? 4 : -4;
    }

    public void ReverseGravity()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * -1, transform.localScale.z);
        rb.gravityScale *= -1;
    }


    public void ForceDeath(Transform source)
    {
        invincible = false;
        UpdateHealth(health, source);
    }

    public void ForceJump(float force)
    {
        GameAudio.PlaySFX("Jump");
        animator.SetTrigger("jump");
        jumpPressed = false;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, force * rb.gravityScale), ForceMode2D.Impulse);
    }


    public void UpdateHealth(int dmg, Transform source)
    {
        if (dmg > 0 && (invincible || GameManager.instance.inCutscene)) return;

        health = Mathf.Clamp(health - dmg, 0, maxHealth);
        GameGUI.instance.UpdateHealth(health, maxHealth);


        if (dmg > 0)
        {
            animator.SetTrigger("damage");
            rb.velocity = Vector2.zero;
            canMove = false;
            rb.AddForce((transform.position - source.position) / Vector3.Distance(transform.position, source.position) * 5);

            if (health == 0)
            {
                GameManager.instance.IncrementAchievement("DEATH");
                dead = true;
                GetComponent<BoxCollider2D>().enabled = false;
                GameGUI.instance.SetDeathEffect(true);
                if (changingTime != null)
                {
                    StopCoroutine(changingTime);
                }
                changingTime = StartCoroutine(CR_ChangingTime(0, 2));
            }
            else
            {
                if (flashing != null)
                {
                    StopCoroutine(flashing);
                }
                flashing = StartCoroutine(CR_Flashing());
            }
        }
    }

    IEnumerator CR_Flashing()
    {

        invincible = true;
        for (int i = 0; i < 5; i++)
        {
            if (i == 1)
            {
                canMove = true;
                animator.SetTrigger("endDamage");
            }
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.15f);
        }
        invincible = false;
    }

    IEnumerator CR_ChangingTime(float changeTo, float speed)
    {
        bool add = Time.timeScale < changeTo;

        while (Time.timeScale != changeTo)
        {
            if (add)
            {
                Time.timeScale = Mathf.Clamp(Time.timeScale + speed * Time.deltaTime, 0, changeTo);
            }
            else
            {
                Time.timeScale = Mathf.Clamp(Time.timeScale - speed * Time.deltaTime, changeTo, 1);
            }
            yield return new WaitForEndOfFrame();
        }
    }


    public void SetSlowMotion(bool value)
    {
        inSlowMotion = value;
        GameGUI.instance.SetSlowMotionEffect(inSlowMotion);
        if (changingTime != null)
        {
            StopCoroutine(changingTime);
        }
        changingTime = StartCoroutine(CR_ChangingTime(inSlowMotion ? 0.5f : 1, 6));
    }

    public void EnableFocusMode()
    {
        if (dead || GameManager.instance.inCutscene || GameGUI.instance.paused) return;
        if ((stamina == maxStamina || inSlowMotion))
        {
            GameAudio.PlaySFX("Focus");
            SetSlowMotion(!inSlowMotion);
        }

        if (inSlowMotion && stamina != 0)
        {
            stamina = Mathf.Clamp(stamina - Time.deltaTime * staminaDrainSpeed, 0, maxStamina);
            GameGUI.instance.UpdateStamina(stamina, maxStamina, false);
            if (stamina == 0)
            {
                SetSlowMotion(false);
            }
        }
        else if (!inSlowMotion && stamina != maxStamina)
        {
            stamina = Mathf.Clamp(stamina + Time.deltaTime * staminaRegainSpeed, 0, maxStamina);
            GameGUI.instance.UpdateStamina(stamina, maxStamina, stamina != maxStamina);
        }
    }

    public void EnableJump()
    {
        if (dead || GameManager.instance.inCutscene || GameGUI.instance.paused) return;
        if (currentJumps < maxJumps)
        {
            jumpPressed = true;
        }
    }

    public void EnableMove(int value)
    {
        if (dead || GameManager.instance.inCutscene || GameGUI.instance.paused) return;
        xInput = value;
    }

    public void EnableMoveLeft(int value)
    {
        if (dead || GameManager.instance.inCutscene || GameGUI.instance.paused)
        {
            android_MoveLeft = 0;
            return;
        }
        android_MoveLeft = value;
        xInput = android_MoveRight - android_MoveLeft;
    }

    public void EnableMoveRight(int value)
    {
        if (dead || GameManager.instance.inCutscene || GameGUI.instance.paused)
        {
            android_MoveRight = 0;
            return;
        }
        android_MoveRight = value;
        xInput = android_MoveRight - android_MoveLeft;
    }

    void Update()
    {
        if (dead || GameManager.instance.inCutscene || GameGUI.instance.paused
        || Application.platform == RuntimePlatform.Android) return;

        xInput = Input.GetAxis("Horizontal");

        if (!jumpPressed && currentJumps < maxJumps)
        {
            jumpPressed = Input.GetButtonDown("Jump");
        }

        if ((stamina == maxStamina || inSlowMotion) && Input.GetButtonDown("Fire1"))
        {
            GameAudio.PlaySFX("Focus");
            SetSlowMotion(!inSlowMotion);
        }



        if (inSlowMotion && stamina != 0)
        {
            stamina = Mathf.Clamp(stamina - Time.deltaTime * staminaDrainSpeed, 0, maxStamina);
            GameGUI.instance.UpdateStamina(stamina, maxStamina, false);
            if (stamina == 0)
            {
                SetSlowMotion(false);
            }
        }
        else if (!inSlowMotion && stamina != maxStamina)
        {
            stamina = Mathf.Clamp(stamina + Time.deltaTime * staminaRegainSpeed, 0, maxStamina);
            GameGUI.instance.UpdateStamina(stamina, maxStamina, stamina != maxStamina);
        }
    }

    public void StopMoving()
    {
        xInput = 0;
        rb.velocity = Vector2.zero;
        animator.SetFloat("speed", xInput);
    }


    void FixedUpdate()
    {
        if (dead || !canMove || GameGUI.instance.paused) return;

        Collider2D collidedObj = Physics2D.OverlapArea(feetPosition.position - new Vector3(spriteRenderer.sprite.bounds.size.x / 2 - 0.3f, 0.15f),
        feetPosition.position + new Vector3(spriteRenderer.bounds.size.x / 2 - 0.4f, 0.5f),
         groundLayer.value);

        _grounded = collidedObj != null;
        onIce = _grounded && collidedObj.tag.Equals("OrangePaint");

        if (_grounded && !jumpPressed && currentJumps != 0)
        {
            currentJumps = 0;
        }

        if (!onIce)
        {
            rb.velocity = new Vector2(xInput * maxSpeed, rb.velocity.y);
        }
        else
        {
            Vector3 targetVelocity = new Vector2(xInput * maxSpeed, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, 0.5f);
        }


        if (xInput != 0 && (xInput > 0 != facingRight))
        {
            facingRight = !facingRight;

            transform.localScale = new Vector3(transform.localScale.x * -1,
            transform.localScale.y, transform.localScale.z);
        }

        if (jumpPressed)
        {
            GameAudio.PlaySFX("Jump");
            currentJumps++;
            animator.SetTrigger("jump");
            jumpPressed = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, jumpForce * rb.gravityScale), ForceMode2D.Impulse);
        }

        animator.SetBool("ground", _grounded);
        animator.SetFloat("speed", Mathf.Abs(xInput));
    }

}
