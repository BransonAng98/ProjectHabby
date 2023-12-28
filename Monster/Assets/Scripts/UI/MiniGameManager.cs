using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MiniGameManager : MonoBehaviour
{
    public LevelManagerScriptableObject levelData;
    [SerializeField] PlayerProgressChecker progressChecker;
    public PlayerStatScriptableObject playerData;

    private void Start()
    {
        progressChecker = GetComponent<PlayerProgressChecker>();
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("Trying to change");
        levelData.worldID = 1;
        levelData.cityLevel = 1;
        playerData.levelProgress++;
        progressChecker.UpdatePlayerProgress();
        SceneManager.LoadScene("LevelSelectScene");
    }
}
