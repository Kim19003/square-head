using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Vector2 = UnityEngine.Vector2;

public class Enemy : MonoBehaviour
{
    public float health = 1;
    public float damage = 1;
    public float jumpForce = 6;
    public float movementForce = 3;
    public float knockoutForce = 6;

    public float minMoveTime = 2;
    public float maxMoveTime = 5;
    public float idleTime = 1;
    public float leftMoveChancePercentage = 50;
    public float rightMoveChancePercentage = 50;
    public float jumpChancePercentage = 20;

    public Vector2 DefaultPosition { get; private set; }
    public float DefaultGravityScale { get; private set; }
    public float DefaultJumpForce { get; private set; }
    public float DefaultMovementForce { get; private set; }
    public float DefaultKnockoutForce { get; private set; }

    public float CurrentHorizontalLookingDirection { get; private set; }
    public float CurrentVerticalLookingDirection { get; private set; }
    public bool IsImmune { get; private set; }
    public bool IsInWater { get; set; }

    Rigidbody2D thisRigidbody2D;
    SpriteRenderer thisSpriteRenderer;
    Collider2D thisCollider2D;
    Animator thisAnimator;

    GameObject player;
    Player playerScript;
    Collider2D playerCollider2D;
    GameController gameControllerScript;

    bool isMovingLeft = false;
    bool isMovingRight = false;
    bool isJumping = false;
    bool isIdling = false;

    bool wasOverLeft = false;
    bool wasOverRight = false;

    Vector2 movementDirection = Vector2.right;

    float aiActionInterval = 2f;

    float nextImmunityFlashingTime = 0f;
    float immunityFlashingInterval = 0.1f;

    bool alertedByPlayer = false;

    bool isDead = false;

    TimedUnityAction timedAIAction = new TimedUnityAction();

    void Start()
    {
        thisRigidbody2D = GetComponent<Rigidbody2D>();
        thisSpriteRenderer = GetComponent<SpriteRenderer>();
        thisCollider2D = GetComponent<Collider2D>();
        thisAnimator = GetComponent<Animator>();

        DefaultPosition = transform.position;
        DefaultGravityScale = thisRigidbody2D.gravityScale;
        DefaultJumpForce = jumpForce;
        DefaultMovementForce = movementForce;
        DefaultKnockoutForce = knockoutForce;

        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();
        playerCollider2D = player.transform.Find("Normal Body").GetComponent<Collider2D>();
        gameControllerScript = GameObject.Find("GameController").GetComponent<GameController>();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        HandleImmunityFlashing(ref nextImmunityFlashingTime, immunityFlashingInterval, IsImmune);

        if (movementDirection.x != 0)
        {
            CurrentHorizontalLookingDirection = movementDirection.x;
            thisAnimator.SetFloat("HorizontalLookingDirection", CurrentHorizontalLookingDirection);
        }
        //if (movementDirection.y != 0)
        //{
        //    CurrentVerticalLookingDirection = movementDirection.y;
        //    thisAnimator.SetFloat("VerticalLookingDirection", CurrentVerticalLookingDirection);
        //}

        thisAnimator.SetBool("IsAlertedByPlayer", alertedByPlayer);

        RaycastHit2D[] leftRaycastHits2D = Physics2D.RaycastAll(new Vector2(transform.position.x - 0.5f, transform.position.y), Vector2.down, 2f);
        //Debug.DrawRay(new Vector2(transform.position.x - 0.5f, transform.position.y), new Vector2(0, -2), Color.red);
        RaycastHit2D[] rightRaycastHits2D = Physics2D.RaycastAll(new Vector2(transform.position.x + 0.5f, transform.position.y), Vector2.down, 2f);
        //Debug.DrawRay(new Vector2(transform.position.x + 0.5f, transform.position.y), new Vector2(0, -2), Color.red);

        bool isOverLeft = !leftRaycastHits2D.Any(r => r.collider.CompareTag("Platform"));
        bool isOverRight = !rightRaycastHits2D.Any(r => r.collider.CompareTag("Platform"));

        if (isOverLeft)
        {
            isMovingLeft = false;
            wasOverLeft = true;
        }
        if (isOverRight)
        {
            isMovingRight = false;
            wasOverRight = true;
        }

        if (playerScript.IsDead)
        {
            alertedByPlayer = false;
        }

        if (alertedByPlayer)
        {
            if (playerScript.IsImmune)
            {
                StartCoroutine(StartIdling(2));
            }
            else if (!isIdling)
            {
                switch (GetPlayerDirection())
                {
                    case -1:
                        isMovingRight = false;
                        if (!isOverLeft)
                        {
                            isMovingLeft = true;
                        }
                        break;
                    case 1:
                        isMovingLeft = false;
                        if (!isOverRight)
                        {
                            isMovingRight = true;
                        }
                        break;
                }
            }
        }

        timedAIAction.Run(() =>
        {
            //Debug.Log($"nextActionTime: {nextActionTime}");
            //Debug.Log($"Time.time: {Time.time}");
            //Debug.LogWarning($"leftRaycastHits2D distance: {(leftRaycastHits2D.Length > 0 ? leftRaycastHits2D[0].distance : -1)}");
            //Debug.LogWarning($"rightRaycastHits2D distance: {(rightRaycastHits2D.Length > 0 ? rightRaycastHits2D[0].distance : -1)}");

            if (!alertedByPlayer && !wasOverLeft && !isIdling && !isMovingLeft && !isMovingRight
                && WantToMoveLeft(leftMoveChancePercentage * (wasOverRight ? 1.5f : 1f)))
            {
                StartCoroutine(StartMovingLeft(Random.Range(minMoveTime, maxMoveTime)));
            }
            else if (!alertedByPlayer && !wasOverRight && !isIdling && !isMovingLeft && !isMovingRight
                && WantToMoveRight(rightMoveChancePercentage * (wasOverLeft ? 1.5f : 1f)))
            {
                StartCoroutine(StartMovingRight(Random.Range(minMoveTime, maxMoveTime)));
            }

            if (!wasOverLeft && !wasOverRight && !isJumping && WantToJump(jumpChancePercentage))
            {
                isJumping = true;
            }

            if (!alertedByPlayer && !isMovingLeft && !isMovingRight && !isIdling)
            {
                StartCoroutine(StartIdling(idleTime));
            }
        }, aiActionInterval, 1f);
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        if (isMovingLeft)
        {
            //Debug.Log("Is moving left");
            thisRigidbody2D.velocity = new Vector2(-movementForce, thisRigidbody2D.velocity.y);
            movementDirection = Vector2.left;

            if (wasOverRight)
            {
                wasOverRight = false;
            }
        }
        if (isMovingRight)
        {
            //Debug.Log("Is moving right");
            thisRigidbody2D.velocity = new Vector2(movementForce, thisRigidbody2D.velocity.y);
            movementDirection = Vector2.right;

            if (wasOverLeft)
            {
                wasOverLeft = false;
            }
        }
        if (isJumping)
        {
            //Debug.Log("Is jumping");
            thisRigidbody2D.velocity = new Vector2(thisRigidbody2D.velocity.x, jumpForce);

            isJumping = false;
        }
        if (isIdling)
        {
            //Debug.Log("Is idling");
            thisRigidbody2D.velocity = new Vector2(0, thisRigidbody2D.velocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":
                if (!playerScript.IsImmune)
                {
                    playerScript.TakeDamage(damage);
                }
                break;
        }
    }

    public void TakeDamage(float damage, bool causedByPlayer = true)
    {
        if (!IsImmune)
        {
            health -= damage;

            if (causedByPlayer)
            {
                playerScript.HasAttackedEnemy = true;
            }

            if (health > 0)
            {
                thisRigidbody2D.velocity = new Vector2(thisRigidbody2D.velocity.x, knockoutForce);

                StartCoroutine(StartImmunity(2));

                if (causedByPlayer)
                {
                    alertedByPlayer = true;
                }
            }
            else
            {
                Die();
            }
        }
    }

    public void Die()
    {
        thisCollider2D.enabled = false;
        thisSpriteRenderer.sortingLayerName = "Player Dying";
        thisRigidbody2D.velocity = new Vector2(0, knockoutForce / 2);

        thisRigidbody2D.gravityScale = 2f;
        thisRigidbody2D.freezeRotation = false;
        thisRigidbody2D.MoveRotation(-180);

        alertedByPlayer = false;
        thisAnimator.SetBool("IsAlertedByPlayer", alertedByPlayer);

        isDead = true;

        int playerKilledEnemiesAmount = 1;
        int playerPreviousKilledEnemies = gameControllerScript.KilledEnemies;
        gameControllerScript.GainKilledEnemies(playerKilledEnemiesAmount);
        playerScript.SetOverPlayerText($"+{gameControllerScript.KilledEnemies - playerPreviousKilledEnemies}", Color.magenta);
    }

    IEnumerator StartImmunity(float duration)
    {
        if (!IsInWater)
        {
            thisSpriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            thisSpriteRenderer.color = Helpers.GetInWaterColor(0.5f);
        }
        Physics2D.IgnoreCollision(thisCollider2D, playerCollider2D, true);
        IsImmune = true;

        yield return new WaitForSeconds(duration);

        if (!IsInWater)
        {
            thisSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            thisSpriteRenderer.color = Helpers.GetInWaterColor(1f);
        }
        Physics2D.IgnoreCollision(thisCollider2D, playerCollider2D, false);
        IsImmune = false;
    }

    bool WantToMoveLeft(float chancePercentage)
    {
        float randomNumber = Random.Range(1, 100);

        if (randomNumber.IsIn(1, chancePercentage))
        {
            return true;
        }

        return false;
    }

    bool WantToMoveRight(float chancePercentage)
    {
        float randomNumber = Random.Range(1, 100);

        if (randomNumber.IsIn(1, chancePercentage))
        {
            return true;
        }

        return false;
    }

    bool WantToJump(float chancePercentage)
    {
        float randomNumber = Random.Range(1, 100);

        if (randomNumber.IsIn(1, chancePercentage))
        {
            return true;
        }

        return false;
    }

    IEnumerator StartMovingLeft(float duration)
    {
        isMovingLeft = true;

        yield return new WaitForSeconds(duration);

        isMovingLeft = false;
    }

    IEnumerator StartMovingRight(float duration)
    {
        isMovingRight = true;

        yield return new WaitForSeconds(duration);

        isMovingRight = false;
    }

    IEnumerator StartIdling(float duration)
    {
        isIdling = true;

        yield return new WaitForSeconds(duration);

        isIdling = false;
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

    void HandleImmunityFlashing(ref float nextImmunityFlashingTime, float immunityFlashingInterval, bool isImmune)
    {
        if (Time.timeSinceLevelLoad > nextImmunityFlashingTime)
        {
            if (isImmune)
            {
                if (thisSpriteRenderer.color.a == 0.5f)
                {
                    thisSpriteRenderer.color = new Color(thisSpriteRenderer.color.r, thisSpriteRenderer.color.g, thisSpriteRenderer.color.b, 0f);
                }
                else
                {
                    thisSpriteRenderer.color = new Color(thisSpriteRenderer.color.r, thisSpriteRenderer.color.g, thisSpriteRenderer.color.b, 0.5f);
                }
            }
            else if (thisSpriteRenderer.color.a.IsAny(0.5f, 0f))
            {
                thisSpriteRenderer.color = new Color(thisSpriteRenderer.color.r, thisSpriteRenderer.color.g, thisSpriteRenderer.color.b, 1f);
            }

            nextImmunityFlashingTime += immunityFlashingInterval;
        }
    }

    float GetPlayerDirection()
    {
        if (player.transform.position.x < transform.position.x)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }
}
