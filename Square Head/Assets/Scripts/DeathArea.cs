using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class DeathArea : MonoBehaviour
{
    GameObject player;
    Rigidbody2D playerRb;
    Player playerScript;
    GameController gameControllerScript;

    void Start()
    {
        player = GameObject.Find("Player");
        playerRb = player.GetComponent<Rigidbody2D>();
        playerScript = player.GetComponent<Player>();
        gameControllerScript = GameObject.Find("GameController").GetComponent<GameController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameControllerScript.LosePlayerLifes(1);

            StartCoroutine(ReturnToPositionAfterTime(0.5f));
        }
        else if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }

    IEnumerator ReturnToPositionAfterTime(float seconds)
    {
        playerRb.bodyType = RigidbodyType2D.Static;

        yield return new WaitForSeconds(seconds);

        if (!gameControllerScript.PlayerIsDead())
        {
            player.transform.position = playerScript.DefaultPosition;
            playerScript.SetOverPlayerTextPosition(playerScript.GetDefaultOverPlayerTextPosition());
            playerScript.SetOverPlayerText($"-1", Color.red);
        }

        playerRb.bodyType = RigidbodyType2D.Dynamic;
    }
}
