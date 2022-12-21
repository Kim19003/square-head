using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public float vanishingAnimationInterval = 0.01f;
    public float vanishingAnimationPower = 0.2f;
    public float vanishingAnimationStartDelay = 0.5f;

    Collider2D thisCollider2D;
    SpriteRenderer thisSpriteRenderer;

    GameController gameController;

    bool pickedUp = false;

    float nextActionTime = 0f;

    void Start()
    {
        thisCollider2D = GetComponent<Collider2D>();
        thisSpriteRenderer = GetComponent<SpriteRenderer>();

        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    void Update()
    {
        if (thisSpriteRenderer.color.a < 0.01f)
        {
            Destroy(gameObject);
        }
        else if (Time.time > nextActionTime)
        {
            if (pickedUp)
            {
                thisSpriteRenderer.color = new Color(1, 1, 1, thisSpriteRenderer.color.a - vanishingAnimationPower);
            }

            nextActionTime += vanishingAnimationInterval;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameController.GainPlayerPoints(1);
            thisCollider2D.enabled = false;

            StartCoroutine(Vanish(vanishingAnimationStartDelay));
        }
    }

    IEnumerator Vanish(float delay)
    {
        yield return new WaitForSeconds(delay);

        pickedUp = true;
    }
}
