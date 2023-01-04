using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndArea : MonoBehaviour
{
    GameController gameControllerScript;

    void Start()
    {
        gameControllerScript = GameObject.Find("GameController").GetComponent<GameController>();
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            gameControllerScript.LevelCompleted = true;
        }
    }
}
