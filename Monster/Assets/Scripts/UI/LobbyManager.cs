using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public LevelManagerScriptableObject levelData;
    public PlayerStatScriptableObject playerData;
    public TextMeshProUGUI levelName;
    public ResourceScriptableObject resourceData;
    public TextMeshProUGUI gnaText;
    public AudioManagerScript audiomanager;
    public MenuAMScript menuaudiomanager;
    public Button ultimate1;
    public Button ultimate2;

    // Start is called before the first frame update
    void Start()
    {
        menuaudiomanager = GameObject.Find("MenuAudioManager").GetComponent<MenuAMScript>();
    }

    void UpdateLevelName()
    {
        levelName.text = "Europe: France " + levelData.cityLevel;
        gnaText.text = "" + resourceData.currentGNA;
    }

    public void LoadLevel()
    {
        menuaudiomanager.PlayTap();
        if (levelData.cutscenePlayed)
        {
            if (!levelData.loopGame)
            {
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
                        levelData.loopGame = true;
                        break;
                }
            }

            else
            {
                levelData.cityLevel = 5;
                SceneManager.LoadScene("France_Hard_Level");
            }
        }
        
        else
        {
            SceneManager.LoadScene("ComicScene");
            levelData.cutscenePlayed = true;
        }
       
    }

    public void SelectUltimate(int whichUlt)
    {
        playerData.setUltimate = whichUlt;
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

    // Update is called once per frame
    void Update()
    {
        UpdateLevelName();
    }
}
