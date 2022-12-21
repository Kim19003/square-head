using UnityEngine;

public class Ladder : MonoBehaviour
{
    public float ladderLeavingForce;

    public GameObject player;

    Rigidbody2D playerRb;
    Player playerScript;

    void Start()
    {
        playerRb = player.GetComponent<Rigidbody2D>();
        playerScript = player.GetComponent<Player>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerRb.gravityScale = 0f;
            playerScript.IsOnLadder = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerRb.gravityScale = playerScript.DefaultGravityScale;
            playerScript.IsOnLadder = false;

            if (playerRb.velocity.y > 0.01)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, ladderLeavingForce);
            }
        }
    }
}
