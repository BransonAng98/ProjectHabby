using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MiniGameManager : MonoBehaviour
{
    public LevelManagerScriptableObject levelData;
    private GameObject landMark;
 
    // Start is called before the first frame update
    void Start()
    {
        landMark = GameObject.FindGameObjectWithTag("Landmark");
    }

    void ReturnToMainMenu()
    {
        levelData.cityLevel = 1;
        SceneManager.LoadScene("LevelSelectScene");
    }

    // Update is called once per frame
    void Update()
    {
        if(landMark != null)
        {
            return;
        }

        else
        {
            Invoke("ReturnToMainMenu", 2f);
        }
    }
}
