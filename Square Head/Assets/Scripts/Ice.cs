using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice : MonoBehaviour
{
    GameObject player;
    Player playerScript;
    Rigidbody2D playerRb;

    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            playerRb.drag = 2;
            playerRb.velocity = new Vector2(playerRb.velocity.x, playerRb.velocity.y);
            playerScript.IsOnIce = true;
        }
    }
        
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            playerRb.drag = playerScript.DefaultDrag;
            playerScript.IsOnIce = false;
        }
    }
}
