using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetGame : MonoBehaviour
{
    public ResourceScriptableObject resourceData;
    public LevelManagerScriptableObject levelData;

    [SerializeField] StaminaSystem staminaSystem;

    public GameObject confirmationScreen;

    public void Awake()
    {
        confirmationScreen.SetActive(false);
        staminaSystem = GameObject.Find("StaminaSystem").GetComponent<StaminaSystem>();
    }

    public void OpenConfirmScreen()
    {
        confirmationScreen.SetActive(true);
    }

    public void CloseConfirmScreen()
    {
        confirmationScreen.SetActive(false);
    }

    public void ResetData()
    {
        resourceData.currentGold = 0;
        levelData.cutscenePlayed = false;
        levelData.tutorialPlayed = false;
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("StartScene");
        staminaSystem.ResetStamina();
    }
}
