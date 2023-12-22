using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MiniGameManager : MonoBehaviour
{
    public LevelManagerScriptableObject levelData;

    public void ReturnToMainMenu()
    {
        Debug.Log("Trying to change");
        levelData.worldID++;
        levelData.cityLevel = 1;
        SceneManager.LoadScene("LevelSelectScene");
    }
}
