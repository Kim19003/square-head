using Assets.Scripts;
using System.Collections;
using System.Drawing;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{
    public float jumpForce;
    public float movementForce;
    public float knockoutForce;

    public Text overPlayerText;

    public Vector3 DefaultPosition { get; private set; }
    public float DefaultGravityScale { get; private set; }
    public float DefaultDrag { get; private set; }
    public float DefaultJumpForce { get; private set; }
    public float DefaultMovementForce { get; private set; }
    public float DefaultKnockoutForce { get; private set; }

    public float CurrentHorizontalLookingDirection { get; private set; }
    public float CurrentVerticalLookingDirection { get; private set; }
    public float PreviousOtherThanZeroXVelocity { get; private set; }
    public float PreviousOtherThanZeroYVelocity { get; private set; }

    public int Lifes { get { return gameControllerScript.PlayerLifes; } }
    public int Points { get { return gameControllerScript.PlayerPoints; } }
    public int Ammunation { get { return gameControllerScript.PlayerAmmunation; } }

    public bool IsGrounded { get { return groundCheck.IsGrounded; } }
    public bool IsOnLadder { get; set; }
    public bool IsInWater { get; set; }
    public bool IsOnIce { get; set; }
    public bool IsAttacking { get; private set; }
    public bool IsImmune { get; private set; }
    public bool IsDead { get; private set; }

    Rigidbody2D rb;
    Collider2D col;
    SpriteRenderer sr;
    GroundCheck groundCheck;
    Animator animator;
    GameController gameControllerScript;

    GameObject bullet;

    bool canAttack = true;

    float nextImmunityFlashingTime = 0f;
    float immunityFlashingInterval = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = transform.Find("Normal Body").GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        groundCheck = transform.Find("GroundCheck").GetComponent<GroundCheck>();
        animator = GetComponent<Animator>();
        gameControllerScript = GameObject.Find("GameController").GetComponent<GameController>();

        bullet = GameObject.Find("Bullet");

        DefaultPosition = transform.position;
        DefaultGravityScale = rb.gravityScale;
        DefaultDrag = rb.drag;
        DefaultJumpForce = jumpForce;
        DefaultMovementForce = movementForce;
        DefaultKnockoutForce = knockoutForce;
    }

    void Update()
    {
        if (IsDead)
        {
            StartCoroutine(GameOverInTime(3f));

            return;
        }

        if (!overPlayerTextCoroutineStarted)
        {
            overPlayerText.transform.position = GetDefaultOverPlayerTextPosition();
        }

        HandleImmunityFlashing(ref nextImmunityFlashingTime, immunityFlashingInterval, IsImmune);

        Vector2 movementDirection = GetWantToMoveDirection();

        animator.SetFloat("Horizontal", movementDirection.x);
        animator.SetFloat("Vertical", movementDirection.y);
        if (movementDirection.x != 0)
        {
            CurrentHorizontalLookingDirection = movementDirection.x;
            animator.SetFloat("HorizontalLookingDirection", CurrentHorizontalLookingDirection);
        }
        if (movementDirection.y != 0)
        {
            CurrentVerticalLookingDirection = movementDirection.y;
            animator.SetFloat("VerticalLookingDirection", CurrentVerticalLookingDirection);
        }
        animator.SetBool("IsOnLadder", IsOnLadder);
        animator.SetBool("IsGrounded", IsGrounded);
        animator.SetBool("IsAttacking", IsAttacking);
        animator.SetBool("IsOnIce", IsOnIce);
        animator.SetFloat("HorizontalSpeed", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("VerticalSpeed", Mathf.Abs(rb.velocity.y));

        if (rb.velocity.x != 0)
        {
            PreviousOtherThanZeroXVelocity = rb.velocity.x;
        }
        if (rb.velocity.y != 0)
        {
            PreviousOtherThanZeroYVelocity = rb.velocity.y;
        }

        SlowFasterWhenHasDrag(0.2f);

        if (IsOnLadder)
        {
            StopAttack();
        }
        else if (IsGrounded)
        {
            if (WantToMoveUp()) // Want to jump
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if (WantToAttack())
            {
                if (!IsAttacking && canAttack)
                {
                    StartCoroutine(Attack(0.2f, 0.3f));
                }
            }
        }
        else // Is on air
        {
            if (WantToAttack())
            {
                if (!IsAttacking && canAttack)
                {
                    StartCoroutine(Attack(0.2f, 0.3f));
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (IsDead)
        {
            return;
        }

        if (IsOnLadder)
        {
            HandleLadderMovement();
        }
        else
        {
            if (WantToMoveLeft())
            {
                rb.velocity = new Vector2(-movementForce, rb.velocity.y);
            }
            else if (WantToMoveRight())
            {
                rb.velocity = new Vector2(movementForce, rb.velocity.y);
            }
            else
            {
                if (!IsOnIce)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
            }
        }
    }

    void HandleLadderMovement()
    {
        if (WantToMoveUp())
        {
            if (WantToMoveLeft())
            {
                rb.velocity = new Vector2(-movementForce, movementForce);
            }
            else if (WantToMoveRight())
            {
                rb.velocity = new Vector2(movementForce, movementForce);
            }
            else
            {
                rb.velocity = new Vector2(0f, movementForce);
            }
        }
        else if (WantToMoveDown())
        {
            if (WantToMoveLeft())
            {
                rb.velocity = new Vector2(-movementForce, -movementForce);
            }
            else if (WantToMoveRight())
            {
                rb.velocity = new Vector2(movementForce, -movementForce);
            }
            else
            {
                rb.velocity = new Vector2(0f, -movementForce);
            }
        }
        else if (WantToMoveLeft())
        {
            if (WantToMoveUp())
            {
                rb.velocity = new Vector2(-movementForce, movementForce);
            }
            else if (WantToMoveDown())
            {
                rb.velocity = new Vector2(-movementForce, -movementForce);
            }
            else
            {
                rb.velocity = new Vector2(-movementForce, 0f);
            }
        }
        else if (WantToMoveRight())
        {
            if (WantToMoveUp())
            {
                rb.velocity = new Vector2(movementForce, movementForce);
            }
            else if (WantToMoveDown())
            {
                rb.velocity = new Vector2(movementForce, -movementForce);
            }
            else
            {
                rb.velocity = new Vector2(movementForce, 0f);
            }
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
        }
    }

    Vector2 GetWantToMoveDirection()
    {
        Vector2 direction = Vector2.zero;

        if (WantToMoveLeft())
        {
            direction += Vector2.left;
        }
        else if (WantToMoveRight())
        {
            direction += Vector2.right;
        }
        else if (WantToMoveUp())
        {
            direction += Vector2.up;
        }
        else if (WantToMoveDown())
        {
            direction += Vector2.down;
        }

        return direction;
    }

    public void Die()
    {
        col.enabled = false;
        sr.sortingLayerName = "Player Dying";
        rb.velocity = new Vector2(0, knockoutForce / 2);

        rb.gravityScale = 2f;
        rb.freezeRotation = false;
        rb.MoveRotation(-180);

        IsDead = true;
    }

    IEnumerator GameOverInTime(float time)
    {
        yield return new WaitForSeconds(time);

        gameControllerScript.GameOver(false);
    }

    IEnumerator Attack(float attackDuration, float attackDelayDuration)
    {
        IsAttacking = true;
        canAttack = false;

        if (gameControllerScript.PlayerHasAmmunation())
        {
            GameObject newBullet = Instantiate(bullet);
            Bullet newBulletScript = newBullet.GetComponent<Bullet>();
            newBulletScript.Owner = gameObject;
            newBulletScript.CurrentHorizontalLookingDirection = CurrentHorizontalLookingDirection;
            newBullet.transform.position = transform.position + new Vector3(CurrentHorizontalLookingDirection * 0.5f, 0, 0);
            newBulletScript.Launch();
            gameControllerScript.LosePlayerAmmunation(1);
        }

        yield return new WaitForSeconds(attackDuration);

        IsAttacking = false;

        yield return new WaitForSeconds(attackDelayDuration);

        canAttack = true;
    }

    void StopAttack()
    {
        IsAttacking = false;
    }

    public IEnumerator StartImmunity(float duration)
    {
        sr.color = new Color(1f, 1f, 1f, 0.5f);
        GameObject[] foundEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in foundEnemies)
        {
            Physics2D.IgnoreCollision(col, enemy.GetComponent<Collider2D>(), true);
        }
        IsImmune = true;

        yield return new WaitForSeconds(duration);

        sr.color = new Color(1f, 1f, 1f, 1f);
        foundEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in foundEnemies)
        {
            Physics2D.IgnoreCollision(col, enemy.GetComponent<Collider2D>(), false);
        }
        IsImmune = false;
    }

    bool WantToMoveLeft()
    {
        return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
    }

    bool WantToMoveRight()
    {
        return Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
    }

    bool WantToMoveUp()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
    }

    bool WantToMoveDown()
    {
        return Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
    }

    bool WantToAttack()
    {
        return Input.GetKey(KeyCode.Space);
    }

    public void SetJumpForce(float jumpForce)
    {
        this.jumpForce = jumpForce;
    }

    public void SetMovementForce(float movementForce)
    {
        this.movementForce = movementForce;
    }
    
    public void SetKnockoutForce(float knockoutForce)
    {
        this.knockoutForce = knockoutForce;
    }

    void SlowFasterWhenHasDrag(float minSpeed)
    {
        if (rb.drag > 0 && Mathf.Abs(rb.velocity.x) < minSpeed)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }
    
    void HandleImmunityFlashing(ref float nextImmunityFlashingTime, float immunityFlashingInterval, bool isImmune)
    {
        if (Time.time > nextImmunityFlashingTime)
        {
            if (isImmune)
            {
                if (sr.color.a == 0.5f)
                {
                    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
                }
                else
                {
                    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.5f);
                }
            }
            else if (sr.color.a.IsAny(0.5f, 0f))
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            }

            nextImmunityFlashingTime += immunityFlashingInterval;
        }
    }

    Coroutine overPlayerTextCoroutine = null;
    bool overPlayerTextCoroutineStarted = false;

    public void SetOverPlayerText(string text, Color color)
    {
        overPlayerText.color = color;
        overPlayerText.text = text;
        overPlayerText.transform.position = new Vector2(overPlayerText.transform.position.x, overPlayerText.transform.position.y - 0.5f);

        if (overPlayerTextCoroutine != null && overPlayerTextCoroutineStarted)
        {
            StopCoroutine(overPlayerTextCoroutine);

            overPlayerText.transform.position = GetDefaultOverPlayerTextPosition();

            overPlayerTextCoroutineStarted = false;
        }

        LeanTween.moveY(overPlayerText.gameObject, overPlayerText.transform.position.y + 0.5f, 0.5f).setEase(LeanTweenType.easeInOutSine);

        overPlayerTextCoroutine = StartCoroutine(ClearOverPlayerTextAfter(2));
    }

    IEnumerator ClearOverPlayerTextAfter(float seconds)
    {
        overPlayerTextCoroutineStarted = true;

        yield return new WaitForSeconds(seconds);

        overPlayerText.color = Color.black;
        overPlayerText.text = string.Empty;
        overPlayerText.transform.position = GetDefaultOverPlayerTextPosition();

        overPlayerTextCoroutineStarted = false;
    }

    public void SetOverPlayerTextPosition(Vector2 position)
    {
        overPlayerText.transform.position = position;
    }

    public Vector2 GetDefaultOverPlayerTextPosition()
    {
        return new Vector2(transform.position.x, transform.position.y + 1);
    }
}
