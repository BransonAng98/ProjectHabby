using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetGame : MonoBehaviour
{
    public ResourceScriptableObject resourceData;
    public LevelManagerScriptableObject levelData;
    public void ResetData()
    {
        resourceData.currentGold = 0;
        levelData.cutscenePlayed = false;
        levelData.tutorialPlayed = false;
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("StartScene");
    }
}
