using Assets.Scripts;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
    public int maxPlayerLifes = 5;
    public int maxPlayerPoints = 999;
    public int maxPlayerAmmunation = 12;
    public int maxKilledEnemies = 999;

    public Text lifesText, pointsText, ammunationText, killedEnemiesText;

    public Transform blackbars;

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

    public int KilledEnemies
    {
        get
        {
            return killedEnemies;
        }
        private set
        {
            if (value < 0)
            {
                killedEnemies = 0;
            }
            else if (value > maxKilledEnemies)
            {
                killedEnemies = maxKilledEnemies;
            }
            else
            {
                killedEnemies = value;
            }
        }
    }
    private int killedEnemies = 0;

    public float RunTime { get; set; }
    public bool LevelCompleted { get; set; } = false;

    bool stopRunTime = false;

    bool isFullScreen = false;

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
        else if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isFullScreen)
            {
                Resolution maxResolution = Helpers.GetMaxResolutionInAspectRatio(16, 9);
                Screen.SetResolution(maxResolution.width, maxResolution.height, true);
                isFullScreen = true;
            }
            else
            {
                Screen.SetResolution(640, 360, false);
                isFullScreen = false;
            }
        }

        if (!stopRunTime)
        {
            RunTime += Time.deltaTime;
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

    public void GainKilledEnemies(int amount)
    {
        KilledEnemies += amount;
        killedEnemiesText.text = $"{KilledEnemies} X";
    }

    public void LoseKilledEnemies(int amount)
    {
        KilledEnemies -= amount;
        killedEnemiesText.text = $"{KilledEnemies} X";
    }

    public void GameOver(bool clearEverything = true)
    {
        if (clearEverything)
        {
            LosePlayerLifes(PlayerLifes, false);
        }

        SceneManager.LoadScene(1);
    }

    public void LevelOver()
    {
        Databank.PickedStars = PlayerPoints;
        Databank.KilledEnemies = KilledEnemies;
        Databank.CompletionTime = RunTime;

        SceneManager.LoadScene(2);
    }

    public void HideAllUIElements()
    {
        lifesText.transform.parent.transform.gameObject.SetActive(false);
        pointsText.transform.parent.transform.gameObject.SetActive(false);
        ammunationText.transform.parent.transform.gameObject.SetActive(false);
        killedEnemiesText.transform.parent.transform.gameObject.SetActive(false);
    }

    public void ShowBlackbars(bool isTrue)
    {
        blackbars.gameObject.SetActive(isTrue);
    }

    public void StopRunTime()
    {
        stopRunTime = true;
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

    public void PlayerHasBossWeapon(bool isTrue)
    {
        if (isTrue)
        {
            PlayerAmmunation = maxPlayerAmmunation;
            ammunationText.color = Helpers.GetCustomColor(CustomColor.OrangeRed);
            ammunationText.text = "UNLIMITED";
        }
        else
        {
            ammunationText.color = Color.white;
            ammunationText.text = $"{PlayerAmmunation} X";
        }
    }
}
