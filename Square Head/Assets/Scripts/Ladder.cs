using Assets.Scripts;
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

    void OnTriggerStay2D(Collider2D other)
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
            if (!playerScript.IsInWater)
            {
                playerRb.gravityScale = playerScript.DefaultGravityScale;

                if (playerRb.velocity.y > 0.01)
                {
                    playerRb.velocity = new Vector2(playerRb.velocity.x, ladderLeavingForce);
                }
            }
            else
            {
                playerRb.gravityScale = Helpers.GetWaterGravityScale();
            }

            playerScript.IsOnLadder = false;
        }
    }
}
