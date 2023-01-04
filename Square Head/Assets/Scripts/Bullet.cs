using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20;
    public float damage = 1;
    public bool isEnemyBullet = false;

    public GameObject Owner { get; set; }

    public float DefaultSpeed { get; private set; }

    public float CurrentHorizontalLookingDirection
    {
        get
        {
            return currentHorizontalLookingDirection;
        }
        set
        {
            if (value == 1 || value == -1)
            {
                currentHorizontalLookingDirection = value;
            }
        }
    }
    private float currentHorizontalLookingDirection = 1;

    Rigidbody2D thisRb;

    GameObject player;
    Player playerScript;
    Rigidbody2D playerRb;

    GameController gameControllerScript;

    bool launched = false;

    void Start()
    {
        thisRb = GetComponent<Rigidbody2D>();

        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();
        playerRb = player.GetComponent<Rigidbody2D>();

        gameControllerScript = GameObject.Find("GameController").GetComponent<GameController>();

        DefaultSpeed = speed;
    }

    void FixedUpdate()
    {
        if (!launched)
        {
            return;
        }

        switch (CurrentHorizontalLookingDirection)
        {
            case -1:
                thisRb.velocity = Vector2.left * speed;
                break;
            case 1:
                thisRb.velocity = Vector2.right * speed;
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!launched || other.gameObject == Owner)
        {
            return;
        }

        bool destroyBullet = false;

        switch (other.gameObject.tag)
        {
            case "Enemy":
                if (!isEnemyBullet)
                {
                    if (!other.gameObject.name.Contains("Boss"))
                    {
                        other.gameObject.GetComponent<Enemy>().TakeDamage(damage);
                    }
                    else
                    {
                        other.gameObject.GetComponent<FirstLevelBoss>().TakeDamage(damage);
                    }
                    destroyBullet = true;
                }
                break;
            case "Player":
                if (isEnemyBullet)
                {
                    if (!playerScript.IsImmune)
                    {
                        playerScript.TakeDamage(damage);
                    }
                    destroyBullet = true;
                }
                break;
            case "FallingPlatform":
                other.gameObject.GetComponent<FallingPlatform>().Fall();
                destroyBullet = true;
                break;
            case "Platform":
            case "Crate":
            case "Elevator":
                destroyBullet = true;
                break;
        }

        if (destroyBullet)
        {
            Destroy(gameObject);
        }
    }

    public void Launch()
    {
        launched = true;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    public void SetIsEnemyBullet(bool isEnemyBullet)
    {
        this.isEnemyBullet = isEnemyBullet;
    }
}
