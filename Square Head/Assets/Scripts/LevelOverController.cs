using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelOverController : MonoBehaviour
{
    public Text pickedStarsText, killedEnemiesText, completionTimeText;

    void Start()
    {
        pickedStarsText.text = pickedStarsText.text.Replace("{NUM}", Databank.PickedStars.ToString());
        killedEnemiesText.text = killedEnemiesText.text.Replace("{NUM}", Databank.KilledEnemies.ToString());
        completionTimeText.text = completionTimeText.text.Replace("{NUM}", ((int)Math.Round(Databank.CompletionTime, 1)).ToString());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }
}
