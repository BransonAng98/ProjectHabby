using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LSTutorialManager : MonoBehaviour
{
    public GameObject tutorialpage1;
    public LevelManagerScriptableObject leveldata;

    [SerializeField] int currentPg;
    
    // Start is called before the first frame update
    void Start()
    {
        currentPg = 0;
        tutorialpage1.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ShowTutorial();
        if (Input.GetMouseButtonDown(0))
        {
            currentPg++;
        }
    }
    public void ShowTutorial()
    {
        if (leveldata.levelselecttutorialPlayed == false)
        {
            if (currentPg == 0)
            {

                tutorialpage1.SetActive(true);

            }

            if (currentPg == 1)
            {
                tutorialpage1.SetActive(false);
                leveldata.levelselecttutorialPlayed = true;
                PlayerPrefs.SetString("LevelSelectTutorialPlayed", "Level Select Tutorial has been played");

            }
           
        }
    }
}
