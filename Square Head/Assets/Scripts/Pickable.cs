using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pickable : MonoBehaviour
{
    public PickableType pickableType = PickableType.None;

    readonly float vanishingAnimationInterval = 0.01f; // Default: 0.01f
    readonly float vanishingAnimationPower = 0.2f; // Default: 0.2f
    readonly float vanishingAnimationStartDelay = 0.1f; // Default: 0.1f

    Collider2D thisCollider2D;
    SpriteRenderer thisSpriteRenderer;

    GameObject player;
    Player playerScript;
    GameController gameController;

    bool pickedUp = false;
    bool vanishing = false;

    TimedUnityAction timedUnityAction;

    void Start()
    {
        thisCollider2D = GetComponent<Collider2D>();
        thisSpriteRenderer = GetComponent<SpriteRenderer>();

        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        timedUnityAction = new TimedUnityAction();
    }

    void Update()
    {
        if (thisSpriteRenderer.color.a < 0.01f)
        {
            Destroy(gameObject);
        }
        else
        {
            timedUnityAction.Run(() =>
            {
                if (vanishing)
                {
                    thisSpriteRenderer.color = new Color(1, 1, 1, thisSpriteRenderer.color.a - vanishingAnimationPower);
                }
            }, vanishingAnimationInterval);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (pickedUp)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            bool shouldVanish = false;

            switch (pickableType)
            {
                case PickableType.Heart:
                    if (!gameController.PlayerHasMaxLifes())
                    {
                        int healthAmount = 2;
                        gameController.GainPlayerLifes(healthAmount);
                        playerScript.SetOverPlayerText($"+{healthAmount}", Color.red);
                        shouldVanish = true;
                    }
                    break;
                case PickableType.Star:
                    int pointAmount = 1;
                    gameController.GainPlayerPoints(pointAmount);
                    playerScript.SetOverPlayerText($"+{pointAmount}", Color.yellow);
                    shouldVanish = true;
                    break;
                case PickableType.Ammunation:
                    if (!gameController.PlayerHasMaxAmmunation())
                    {
                        int ammunationAmount = 20;
                        gameController.GainPlayerAmmunation(ammunationAmount);
                        playerScript.SetOverPlayerText($"+{ammunationAmount}", Color.gray);
                        shouldVanish = true;
                    }
                    break;
            }

            if (shouldVanish)
            {
                thisCollider2D.enabled = false;
                pickedUp = true;
                StartCoroutine(Vanish(vanishingAnimationStartDelay));
            }
        }
    }

    IEnumerator Vanish(float delay)
    {
        yield return new WaitForSeconds(delay);

        vanishing = true;
    }
}
