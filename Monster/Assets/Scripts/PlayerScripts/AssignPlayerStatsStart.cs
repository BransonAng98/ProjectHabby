using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignPlayerStatsStart : MonoBehaviour
{
    public PlayerStatScriptableObject playerData;
    public LevelManagerScriptableObject levelData;
    // Start is called before the first frame update
    void Start()
    {
        AssignStats();
    }

    void AssignStats()
    {
        //Assign saved player health
        if (PlayerPrefs.HasKey("PlayerHealth"))
        {
            playerData.maxhealth = PlayerPrefs.GetInt("PlayerHealth");
        }

        else
        {
            playerData.maxhealth = 110;
        }

        //Assign saved player movement speed
        if (PlayerPrefs.HasKey("PlayerMovement"))
        {
            playerData.speed = PlayerPrefs.GetFloat("PlayerMovement");
        }

        else
        {
            playerData.speed = 5;
        }

        //Assign saved player attack damage
        if (PlayerPrefs.HasKey("PlayerAttackDamage"))
        {
            playerData.attackDamage = PlayerPrefs.GetFloat("PlayerAttackDamage");
        }

        else
        {
            playerData.attackDamage = 3;
        }

        //Assign saved player ultimate level
        if (PlayerPrefs.HasKey("PlayerUltimateLevel"))
        {
            playerData.ultimateLevel = PlayerPrefs.GetInt("PlayerUltimateLevel");
        }

        else
        {
            playerData.ultimateLevel = 1;
        }

        //Assign saved player upgrade level
        if (PlayerPrefs.HasKey("PlayerUpgradeLevel"))
        {
            playerData.upgradeLevel = PlayerPrefs.GetInt("PlayerUpgradeLevel");
        }

        else
        {
            playerData.upgradeLevel = 1;
        }

        //Assign saved player progress level
        if (PlayerPrefs.HasKey("LevelProgress"))
        {
            playerData.levelProgress = PlayerPrefs.GetInt("LevelProgress");
        }

        else
        {
            playerData.levelProgress = 0;
        }

        //Assign comic variables
        if (PlayerPrefs.HasKey("CutscenePlayed"))
        {
            levelData.cutscenePlayed = true;
        }

        else
        {
            levelData.cutscenePlayed = false;
        }

        //Assign tutorial variables
        if (PlayerPrefs.HasKey("TutorialPlayed"))
        {
            levelData.tutorialPlayed = true;
        }

        else
        {
            levelData.tutorialPlayed = false;
        }

        if (PlayerPrefs.HasKey("LevelSelectTutorialPlayed"))
        {
            levelData.levelselecttutorialPlayed = true;
        }

        else
        {
            levelData.levelselecttutorialPlayed = false;
        }

        if (PlayerPrefs.HasKey("UpgradeTutorialPlayed"))
        {
            levelData.upgradetutorialPlayed = true;
        }

        else
        {
            levelData.upgradetutorialPlayed = false;
        }
    }
}
