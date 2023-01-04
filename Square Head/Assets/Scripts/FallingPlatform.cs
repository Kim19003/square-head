using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 1f;
    public float respawnDelay = 3f;

    public Vector2 DefaultPosition { get; private set; }
    public float DefaultMass { get; private set; }
    public float DefaultGravityScale { get; private set; }

    Rigidbody2D thisRigidbody2D;
    Collider2D thisCollider2D;

    GameObject player;

    bool falling = false;
    bool startedRespawning = false;

    void Start()
    {
        thisRigidbody2D = GetComponent<Rigidbody2D>();
        thisCollider2D = GetComponent<Collider2D>();

        player = GameObject.Find("Player");

        DefaultPosition = transform.position;
        DefaultMass = thisRigidbody2D.mass;
        DefaultGravityScale = thisRigidbody2D.gravityScale;
    }

    void Update()
    {
        if (falling && !startedRespawning)
        {
            StartCoroutine(RespawnAfter(respawnDelay));

            return;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (falling)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<Player>().IsGrounded)
            {
                Fall();
            }
        }
    }

    IEnumerator FallAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        SetIsFalling(true);
    }

    IEnumerator RespawnAfter(float seconds)
    {
        startedRespawning = true;

        yield return new WaitForSeconds(seconds);

        SetIsFalling(false);

        startedRespawning = false;
    }

    void SetIsFalling(bool isFalling)
    {
        if (isFalling)
        {
            thisCollider2D.enabled = false;
            thisRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            thisRigidbody2D.mass = 0.1f;
            thisRigidbody2D.gravityScale = 2;

            falling = true;
        }
        else
        {
            thisCollider2D.enabled = true;
            thisRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            thisRigidbody2D.mass = DefaultMass;
            thisRigidbody2D.gravityScale = DefaultGravityScale;
            thisRigidbody2D.velocity = Vector2.zero;
            transform.position = DefaultPosition;

            falling = false;
        }
    }

    public void Fall()
    {
        StartCoroutine(FallAfter(fallDelay));
    }
}
