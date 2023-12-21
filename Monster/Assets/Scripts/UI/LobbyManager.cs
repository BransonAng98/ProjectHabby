using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Haptics.Vibrations;

public class LobbyManager : MonoBehaviour
{
    public LevelManagerScriptableObject levelData;
    public PlayerStatScriptableObject playerData;
    public PlayerLevelSelectScript playerSprite;
    public TextMeshProUGUI levelName;
    public ResourceScriptableObject resourceData;
    public TextMeshProUGUI gnaText;
    public AudioManagerScript audiomanager;
    public MenuAMScript menuaudiomanager;
    public Button ultimate1;
    public Button ultimate2;

    private StaminaSystem staminaSystem;

    // Start is called before the first frame update
    void Start()
    {
        menuaudiomanager = GameObject.Find("MenuAudioManager").GetComponent<MenuAMScript>();
        staminaSystem = GameObject.Find("StaminaSystem").GetComponent<StaminaSystem>();
        VibrateHaptics.Initialize();
    }

    void UpdateLevelName()
    {
        levelName.text = "Europe: France " + levelData.cityLevel;
    }

    public void LoadLevel()
    {
        if(staminaSystem.currentEnergy >= 5)
        {
            VibrateHaptics.VibrateDoubleClick();
            menuaudiomanager.PlayTap();

            VibrateHaptics.Release();
            switch (levelData.cityLevel)
            {
                case 1:
                    SceneManager.LoadScene("France_Easy_Level");
                    break;

                case 2:
                    SceneManager.LoadScene("France_Easy_Level");
                    break;

                case 3:
                    SceneManager.LoadScene("France_Medium_Level");
                    break;

                case 4:
                    SceneManager.LoadScene("France_Medium_Level");
                    break;

                case 5:
                    SceneManager.LoadScene("France_Hard_Level");
                    break;

                case 6:
                    SceneManager.LoadScene("LandmarkDesScene");
                    break;
            }
        }
        else
        {
            Debug.Log("Insufficient Energy You Fool!");
        }
        
       
    }

    public void SelectUltimate(int whichUlt)
    {
        VibrateHaptics.VibrateClick();
        playerData.setUltimate = whichUlt;
        playerSprite.TriggerSelectedAnimation();
        ShowSelectedButton(whichUlt);
    }

    void ShowSelectedButton(int whichUlt)
    {
        switch (whichUlt)
        {
            case 0:
                //Apply VFX for button 1
                Debug.Log("Ultimate 1 selected");
                //Remove VFX for button 2
                break;

            case 1:
                //Remove VFX for button 1
                Debug.Log("Ultimate 2 selected");
                //Apply VFX for button 2
                break;
        }
    }

    private void OnDestroy()
    {
        VibrateHaptics.Release();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLevelName();
    }
}
