using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Haptics.Vibrations;

public class LobbyManager : MonoBehaviour
{
    public UpgradeButtonManagerScriptableObject UBSO;
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

    [SerializeField] string countryName;
    public Image countryFlag;
    public List<Sprite> countrySprite = new List<Sprite>();

    private StaminaSystem staminaSystem;

    [SerializeField] int selectedLevel;
    public Button attackButton;
    public Button leftButton;
    public Button rightButton;

    // Start is called before the first frame update
    void Start()
    {
        menuaudiomanager = GameObject.Find("MenuAudioManager").GetComponent<MenuAMScript>();
        staminaSystem = GameObject.Find("StaminaSystem").GetComponent<StaminaSystem>();
        selectedLevel = playerData.levelProgress;
        VibrateHaptics.Initialize();
    }

    private void Update()
    {
        UpdateLevelData();
        LimitLevelEnds();
    }

    void LimitLevelEnds()
    {
        if (selectedLevel == 1 && levelData.worldID == 1)
        {
            leftButton.interactable = false;
        }

        else
        {
            leftButton.interactable = true;
        }

        if (selectedLevel == 6 && levelData.worldID == 3)
        {
            rightButton.interactable = false;
        }

        else
        {
            rightButton.interactable = true;
        }
    }

    void SetLevelAtStart()
    {
        selectedLevel = playerData.levelProgress;
        if (selectedLevel > 6)
        {
            levelData.worldID++;
            selectedLevel = 1;
        }
    }

    void SetCountryName()
    {
        switch (levelData.worldID)
        {
            case 1:
                countryName = "France";
                break;

            case 2:
                countryName = "Germany";
                break;

            case 3:
                countryName = "China";
                break;
        }
    }


    void SetCountryFlag()
    {
        switch (levelData.worldID)
        {
            case 1:
                //France Flag
                countryFlag.sprite = countrySprite[0];
                break;

            case 2:
                //Germany Flag
                countryFlag.sprite = countrySprite[1];
                break;

            case 3:
                //China Flag
                countryFlag.sprite = countrySprite[2];
                break;
        }
    }
    void UpdateLevelData()
    {
        SetCountryName();
        SetCountryFlag();
        levelName.text = countryName + ": " + selectedLevel;
    }

    public void LoadLevel()
    {
        if (staminaSystem.currentEnergy >= 5)
        {
            VibrateHaptics.VibrateDoubleClick();
            menuaudiomanager.PlayTap();
            VibrateHaptics.Release();
            switch (levelData.cityLevel)
            {
                case 0:
                    SceneManager.LoadScene("France_Easy_Level");
                    break;

                case 1:
                    SceneManager.LoadScene("France_Easy_Level");
                    break;

                case 2:
                    SceneManager.LoadScene("France_Medium_Level");
                    break;

                case 3:
                    SceneManager.LoadScene("France_Medium_Level");
                    break;

                case 4:
                    SceneManager.LoadScene("France_Hard_Level");
                    break;

                case 5:
                    SceneManager.LoadScene("LandmarkDesScene");
                    break;
            }

            playerData.levelProgress++;
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

    public void ResetDemo()
    {

        //Reset the player stats
        playerData.maxhealth = 110;
        playerData.attackDamage = 3;
        playerData.speed = 5f;
        playerData.ultimateLevel = 1;

        //Reset the level stats
        levelData.cityLevel = 1;
        levelData.tutorialPlayed = false;
        levelData.cutscenePlayed = false;

        //Restart from the start screen 
        SceneManager.LoadScene("StartScene");
        resourceData.currentGold = 0;
        UBSO.buttonNames.Clear();
    }

    private void OnDestroy()
    {
        VibrateHaptics.Release();
    }





}
