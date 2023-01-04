using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float speed = 1f;

    public Vector2 DefaultStartingPosition { get; private set; }
    public Vector2 DefaultDestinationPosition { get; private set; }

    public bool ReachedStartPosition { get; private set; } = true;
    public bool ReachedDestinationPosition { get; private set; } = false;

    Vector2 startPosition;
    Vector2 destinationPosition;

    GameObject player;

    bool isMoving = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        startPosition = transform.position;
        destinationPosition = transform.Find("Destination").position;

        DefaultStartingPosition = startPosition;
        DefaultDestinationPosition = destinationPosition;
    }

    void Update()
    {
        if (ReachedStartPosition && !ReachedDestinationPosition && Vector2.Distance(transform.position, destinationPosition) < 0.01f)
        {
            ReachedDestinationPosition = true;
            ReachedStartPosition = false;
        }
        else if (ReachedDestinationPosition && !ReachedStartPosition && Vector2.Distance(transform.position, startPosition) < 0.01f)
        {
            ReachedStartPosition = true;
            ReachedDestinationPosition = false;
        }

        if (isMoving)
        {
            if (ReachedStartPosition && !ReachedDestinationPosition)
            {
                transform.position = Vector2.MoveTowards(transform.position, destinationPosition, speed * Time.deltaTime);
                //thisRb.velocity = new Vector2(-speed, 0);
            }
            else if (ReachedDestinationPosition && !ReachedStartPosition)
            {
                transform.position = Vector2.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
                //thisRb.velocity = new Vector2(speed, 0);
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":
                collision.transform.SetParent(transform);
                Rigidbody2D playerRigidBody2D = collision.gameObject.GetComponent<Rigidbody2D>();
                playerRigidBody2D.interpolation = RigidbodyInterpolation2D.None;
                break;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":
                collision.transform.SetParent(null);
                Rigidbody2D playerRigidBody2D = collision.gameObject.GetComponent<Rigidbody2D>();
                playerRigidBody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
                break;
        }
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetIsMoving(bool isMoving)
    {
        this.isMoving = isMoving;
    }
}
