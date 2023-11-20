using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public LevelManagerScriptableObject levelData;
    public TextMeshProUGUI levelName;
    public ResourceScriptableObject resourceData;
    public TextMeshProUGUI gnaText;
    public AudioManagerScript audiomanager;
    public MenuAMScript menuaudiomanager;
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
        if(levelData.cutscenePlayed)
        {
            if (levelData.cityLevel < 10)
            {
                SceneManager.LoadScene("GameplayScene");
            }

            else if (levelData.cityLevel == 10)
            {
                SceneManager.LoadScene("LandmarkDesScene");
            }

            else { return; }
        }
        
        else
        {
            SceneManager.LoadScene("ComicScene");
            levelData.cutscenePlayed = true;
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLevelName();
    }
}
