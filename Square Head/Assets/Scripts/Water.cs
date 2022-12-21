using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public float speedReduction;
    public float waterLeavingForce;

    GameObject player;
    Player playerScript;
    Rigidbody2D playerRb;
    SpriteRenderer playerSr;

    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();
        playerRb = player.GetComponent<Rigidbody2D>();
        playerSr = player.GetComponent<SpriteRenderer>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":
                playerScript.IsInWater = true;
                playerRb.gravityScale = 1f;
                playerScript.SetMovementForce(playerScript.DefaultMovementForce * speedReduction);
                playerScript.SetJumpForce(playerScript.DefaultJumpForce / 3);
                playerScript.SetKnockoutForce(playerScript.DefaultKnockoutForce / 3);
                playerSr.color = new Color(0.6f, 0.6f, 1f);
                break;
            case "Enemy":
                Enemy enemyScript = collision.gameObject.GetComponent<Enemy>();
                Rigidbody2D enemyRigidBody2D = collision.gameObject.GetComponent<Rigidbody2D>();
                SpriteRenderer enemySpriteRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
                enemyScript.IsInWater = true;
                enemyRigidBody2D.gravityScale = 1f;
                enemyScript.SetMovementForce(enemyScript.DefaultMovementForce * speedReduction);
                enemyScript.SetJumpForce(enemyScript.DefaultJumpForce / 3);
                enemyScript.SetKnockoutForce(enemyScript.DefaultKnockoutForce / 3);
                enemySpriteRenderer.color = new Color(0.6f, 0.6f, 1f);
                break;
            case "Bullet":
                Bullet bulletScript = collision.gameObject.GetComponent<Bullet>();
                SpriteRenderer bulletSpriteRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
                bulletScript.SetSpeed(bulletScript.DefaultSpeed * speedReduction);
                bulletSpriteRenderer.color = new Color(0.6f, 0.6f, 1f);
                break;
        }
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    switch (collision.gameObject.tag)
    //    {
    //        case "Player":
    //            playerRb.velocity = new Vector2(playerRb.velocity.x * speedReduction, playerRb.velocity.y);
    //            break;
    //        case "Enemy":
    //            Rigidbody2D enemyRigidBody2D = collision.gameObject.GetComponent<Rigidbody2D>();
    //            enemyRigidBody2D.velocity = new Vector2(enemyRigidBody2D.velocity.x * speedReduction, enemyRigidBody2D.velocity.y);
    //            break;
    //        case "Bullet":
    //            Rigidbody2D bulletRigidBody2D = collision.gameObject.GetComponent<Rigidbody2D>();
    //            bulletRigidBody2D.velocity = new Vector2(bulletRigidBody2D.velocity.x * speedReduction, bulletRigidBody2D.velocity.y);
    //            break;
    //    }
    //}

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":
                playerScript.IsInWater = false;
                playerRb.gravityScale = playerScript.DefaultGravityScale;
                playerScript.SetMovementForce(playerScript.DefaultMovementForce);
                playerScript.SetJumpForce(playerScript.DefaultJumpForce);
                playerScript.SetKnockoutForce(playerScript.DefaultKnockoutForce);
                playerSr.color = new Color(1f, 1f, 1f);
                if (playerRb.velocity.y > 0.01)
                {
                    playerRb.velocity = new Vector2(playerRb.velocity.x, waterLeavingForce);
                }
                break;
            case "Enemy":
                Enemy enemyScript = collision.gameObject.GetComponent<Enemy>();
                Rigidbody2D enemyRigidBody2D = collision.gameObject.GetComponent<Rigidbody2D>();
                SpriteRenderer enemySpriteRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
                enemyScript.IsInWater = false;
                enemyRigidBody2D.gravityScale = enemyScript.DefaultGravityScale;
                enemyScript.SetMovementForce(enemyScript.DefaultMovementForce);
                enemyScript.SetJumpForce(enemyScript.DefaultJumpForce);
                enemyScript.SetKnockoutForce(enemyScript.DefaultKnockoutForce);
                enemySpriteRenderer.color = new Color(1f, 1f, 1f);
                if (enemyRigidBody2D.velocity.y > 0.01)
                {
                    enemyRigidBody2D.velocity = new Vector2(enemyRigidBody2D.velocity.x, waterLeavingForce);
                }
                break;
            case "Bullet":
                Bullet bulletScript = collision.gameObject.GetComponent<Bullet>();
                SpriteRenderer bulletSpriteRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
                bulletScript.SetSpeed(bulletScript.DefaultSpeed);
                bulletSpriteRenderer.color = new Color(1f, 1f, 1f);
                break;
        }
    }

    public float GetSpeedReduction()
    {
        return speedReduction;
    }
}
