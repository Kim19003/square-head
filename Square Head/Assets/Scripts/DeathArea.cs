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

    Vector2 previousPlayerPosition;

    bool playerIsInDeathArea = false;

    void Start()
    {
        player = GameObject.Find("Player");
        playerRb = player.GetComponent<Rigidbody2D>();
        playerScript = player.GetComponent<Player>();
        gameControllerScript = GameObject.Find("GameController").GetComponent<GameController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameControllerScript.LevelCompleted)
        {
            return;
        }

        switch (other.gameObject.tag)
        {
            case "Player":
                if (!playerIsInDeathArea)
                {
                    previousPlayerPosition = playerScript.PreviousSafePosition;

                    gameControllerScript.LosePlayerLifes(1);

                    StartCoroutine(ReturnToPositionAfterTime(0.5f));
                }
                break;
            case "Enemy":
                other.transform.position = other.transform.gameObject.GetComponent<Enemy>().DefaultPosition;
                break;
            //case "FallingPlatform":
            //    other.gameObject.SetActive(false);
            //    break;
        }
    }

    IEnumerator ReturnToPositionAfterTime(float seconds)
    {
        playerIsInDeathArea = true;

        playerRb.bodyType = RigidbodyType2D.Static;

        yield return new WaitForSeconds(seconds);

        if (!gameControllerScript.PlayerIsDead())
        {
            player.transform.position = previousPlayerPosition;
            playerRb.bodyType = RigidbodyType2D.Dynamic;
            playerRb.velocity = new Vector2(0, 0.01f);
            StartCoroutine(playerScript.StartImmunity(2));

            playerScript.SetOverPlayerTextPosition(playerScript.GetDefaultOverPlayerTextPosition());
            playerScript.SetOverPlayerText($"-1", Color.red);

            playerIsInDeathArea = false;
        }
    }
}
