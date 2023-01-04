using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FirstLevelBoss : MonoBehaviour
{
    public float health = 5f;
    public float knockoutForce = 6f;
    public float bulletSpeed = 20f;
    public float bulletDamage = 2f;
    public float shootingDelay = 1f;

    public GameObject steelCrateTop;
    public GameObject steelCrateMiddle;
    public GameObject steelCrateBottom;

    public bool IsImmune { get; private set; }

    Rigidbody2D thisRigidbody2D;
    SpriteRenderer thisSpriteRenderer;
    Collider2D thisCollider2D;
    Animator thisAnimator;

    GameObject player;
    Player playerScript;
    Collider2D playerCollider2D;
    
    GameObject bullet;
    GameController gameControllerScript;

    bool canShoot = true;
    bool isDead = false;

    float nextImmunityFlashingTime = 0f;
    float immunityFlashingInterval = 0.1f;

    void Start()
    {
        thisRigidbody2D = GetComponent<Rigidbody2D>();
        thisCollider2D = GetComponent<Collider2D>();
        thisSpriteRenderer = GetComponent<SpriteRenderer>();
        thisAnimator = GetComponent<Animator>();

        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();
        playerCollider2D = player.transform.Find("Normal Body").GetComponent<Collider2D>();

        bullet = GameObject.Find("Red Bullet");
        gameControllerScript = GameObject.Find("GameController").GetComponent<GameController>();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        HandleImmunityFlashing(ref nextImmunityFlashingTime, immunityFlashingInterval, IsImmune);

        RaycastHit2D[] leftRaycastHits2D = Physics2D.RaycastAll(new Vector2(transform.position.x - 0.5f, transform.position.y), Vector2.left, 16f);
        Debug.DrawRay(new Vector2(transform.position.x - 0.5f, transform.position.y), new Vector2(-16, 0), Color.red);

        if (canShoot && leftRaycastHits2D.Any(r => r.collider.CompareTag("Player")))
        {
            StartCoroutine(Shoot(0.2f, 0.2f, shootingDelay));
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!playerScript.IsImmune)
            {
                playerScript.TakeDamage(bulletDamage);
            }
        }
    }

    IEnumerator Shoot(float beforeDelay, float bulletDelay, float afterDelay)
    {
        canShoot = false;

        yield return new WaitForSeconds(beforeDelay);

        ShootBullet();

        yield return new WaitForSeconds(bulletDelay);

        ShootBullet();

        yield return new WaitForSeconds(bulletDelay);

        ShootBullet();

        yield return new WaitForSeconds(afterDelay);

        canShoot = true;
    }

    void ShootBullet()
    {
        GameObject newBullet = Instantiate(bullet);
        Bullet newBulletScript = newBullet.GetComponent<Bullet>();
        newBulletScript.Owner = gameObject;
        newBulletScript.CurrentHorizontalLookingDirection = -1;
        newBulletScript.SetSpeed(bulletSpeed);
        newBulletScript.SetDamage(bulletDamage);
        newBulletScript.SetIsEnemyBullet(true);
        newBullet.transform.position = transform.position + new Vector3(-1 * 0.5f, 0, 0);
        newBulletScript.Launch();
    }

    public void TakeDamage(float damage)
    {
        if (!IsImmune)
        {
            health -= damage;

            if (health > 0)
            {
                thisRigidbody2D.velocity = new Vector2(thisRigidbody2D.velocity.x, knockoutForce);

                StartCoroutine(StartImmunity(2));
            }
            else
            {
                Die();
            }
        }
    }

    public void Die(bool playerClaimsEnemyKill = true)
    {
        thisCollider2D.enabled = false;
        thisSpriteRenderer.sortingLayerName = "Player Dying";
        thisRigidbody2D.velocity = new Vector2(0, knockoutForce / 2);

        thisRigidbody2D.gravityScale = 2f;
        thisRigidbody2D.freezeRotation = false;
        thisRigidbody2D.MoveRotation(-180);

        thisAnimator.SetBool("IsDead", true);

        isDead = true;
        
        if (playerClaimsEnemyKill)
        {
            int playerKilledEnemiesAmount = 1;
            int playerPreviousKilledEnemies = gameControllerScript.KilledEnemies;
            gameControllerScript.GainKilledEnemies(playerKilledEnemiesAmount);
            playerScript.SetOverPlayerText($"+{gameControllerScript.KilledEnemies - playerPreviousKilledEnemies}", Helpers.GetCustomColor(CustomColor.OrangeRed));
        }

        steelCrateTop.SetActive(false);
        steelCrateMiddle.SetActive(false);
        steelCrateBottom.SetActive(false);
    }

    IEnumerator StartImmunity(float duration)
    {
        thisSpriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        Physics2D.IgnoreCollision(thisCollider2D, playerCollider2D, true);
        IsImmune = true;

        yield return new WaitForSeconds(duration);

        thisSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        Physics2D.IgnoreCollision(thisCollider2D, playerCollider2D, false);
        IsImmune = false;
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
}
