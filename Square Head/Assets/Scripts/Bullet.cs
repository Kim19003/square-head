using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20;
    public float damage = 1;

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

    bool launched = false;

    void Start()
    {
        thisRb = GetComponent<Rigidbody2D>();

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

        switch (other.gameObject.tag)
        {
            case "Enemy":
                other.gameObject.GetComponent<Enemy>().TakeDamage(damage);
                Destroy(gameObject);
                break;
            case "Platform":
            case "Crate":
                Destroy(gameObject);
                break;
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
}
