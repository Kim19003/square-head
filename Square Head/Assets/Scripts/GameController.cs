using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
    public int maxPlayerLifes = 5;
    public int maxPlayerPoints = 999;
    public int maxPlayerAmmunation = 20;

    public Text lifesText, pointsText, ammunationText;

    public int PlayerLifes
    {
        get
        {
            return playerLifes;
        }
        private set
        {
            if (value < 0)
            {
                playerLifes = 0;
            }
            else if (value > maxPlayerLifes)
            {
                playerLifes = maxPlayerLifes;
            }
            else
            {
                playerLifes = value;
            }
        }
    }
    private int playerLifes = 0;

    public int PlayerPoints
    {
        get
        {
            return playerPoints;
        }
        private set
        {
            if (value < 0)
            {
                playerPoints = 0;
            }
            else if (value > maxPlayerPoints)
            {
                playerPoints = maxPlayerPoints;
            }
            else
            {
                playerPoints = value;
            }
        }
    }
    private int playerPoints = 0;

    public int PlayerAmmunation
    {
        get
        {
            return playerAmmunation;
        }
        private set
        {
            if (value < 0)
            {
                playerAmmunation = 0;
            }
            else if (value > maxPlayerAmmunation)
            {
                playerAmmunation = maxPlayerAmmunation;
            }
            else
            {
                playerAmmunation = value;
            }
        }
    }
    private int playerAmmunation = 0;

    void Start()
    {
        PlayerLifes = maxPlayerLifes;
        PlayerPoints = 0;
        PlayerAmmunation = maxPlayerAmmunation;

        lifesText.text = $"{PlayerLifes} X";
        pointsText.text = $"{PlayerPoints} X";
        ammunationText.text = $"{PlayerAmmunation} X";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void LosePlayerLifes(int amount, bool automaticallyEndGameIfNoHealth = true)
    {
        PlayerLifes -= amount;
        lifesText.text = $"{PlayerLifes} X";

        if (PlayerLifes < 1 && automaticallyEndGameIfNoHealth)
        {
            GameOver(false);
        }
    }

    public void GainPlayerLifes(int amount)
    {
        PlayerLifes += amount;
        lifesText.text = $"{PlayerLifes} X";
    }

    public void GainPlayerPoints(int amount)
    {
        PlayerPoints += amount;
        pointsText.text = $"{PlayerPoints} X";
    }

    public void LosePlayerPoints(int amount)
    {
        PlayerPoints -= amount;
        pointsText.text = $"{PlayerPoints} X";
    }

    public void GainPlayerAmmunation(int amount)
    {
        PlayerAmmunation += amount;
        ammunationText.text = $"{PlayerAmmunation} X";
    }

    public void LosePlayerAmmunation(int amount)
    {
        PlayerAmmunation -= amount;
        ammunationText.text = $"{PlayerAmmunation} X";
    }

    public void GameOver(bool clearEverything = true)
    {
        if (clearEverything)
        {
            LosePlayerLifes(PlayerLifes, false);
        }

        SceneManager.LoadScene(1);
    }

    public bool PlayerHasMaxLifes()
    {
        return PlayerLifes == maxPlayerLifes;
    }

    public bool PlayerHasMaxAmmunation()
    {
        return PlayerAmmunation == maxPlayerAmmunation;
    }

    public bool PlayerHasAmmunation()
    {
        return PlayerAmmunation > 0;
    }

    public bool PlayerIsDead()
    {
        return PlayerLifes < 1;
    }
}
