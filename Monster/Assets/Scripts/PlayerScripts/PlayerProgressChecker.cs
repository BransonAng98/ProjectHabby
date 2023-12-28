using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgressChecker : MonoBehaviour
{
    public int levelID;
    public PlayerStatScriptableObject playerData;
    [SerializeField] int storedLevelProgression;
    // Start is called before the first frame update
    void Start()
    {
        storedLevelProgression = playerData.levelProgress;
    }

    public void UpdatePlayerProgress()
    {
        //switch (levelID)
        //{
        //    //Tutorial
        //    case 0:
        //        if (!PlayerPrefs.HasKey("LevelProgress"))
        //        {
        //            playerData.levelProgress++;
        //            PlayerPrefs.SetInt("LevelProgress", 1);
        //        }
        //        break;

        //    //France Easy
        //    case 1:
        //        if(storedLevelProgression == 1)
        //        {
        //            playerData.levelProgress++;
        //            PlayerPrefs.SetInt("LevelProgress", 2);
        //        }
        //        break;

        //    //France Medium
        //    case 2:
        //        if(storedLevelProgression == 3)
        //        {
        //            playerData.levelProgress++;
        //            PlayerPrefs.SetInt("LevelProgress", 4);
        //        }

        //        else if(storedLevelProgression == 2)
        //        {
        //            playerData.levelProgress++;
        //            PlayerPrefs.SetInt("LevelProgress", 3);
        //        }

        //        else
        //        {
        //            return;
        //        }
        //        break;

        //    //France Hard
        //    case 3:
        //        if(storedLevelProgression == 4)
        //        {
        //            playerData.levelProgress++;
        //            PlayerPrefs.SetInt("LevelProgress", 5);
        //        }

        //        else
        //        {
        //            return;
        //        }
        //        break;

        //    //France Landmark
        //    case 4:
        //        if(storedLevelProgression == 5)
        //        {
        //            playerData.levelProgress++;
        //            PlayerPrefs.SetInt("LevelProgress", 6);
        //            PlayerPrefs.SetInt("LandmarkDestructionCleared", 1);
        //        }

        //        else
        //        {
        //            return;
        //        }
        //        break;
        //}
    }
}
