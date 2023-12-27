using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeTutorialManager : MonoBehaviour
{
    public Image tutorialpage1;
    public Image tutorialpage2;
    public LevelManagerScriptableObject leveldata;
    public ButtonDataHandler buttonscript;

    [SerializeField] int currentPg;
    // Start is called before the first frame update
    void Start()
    {
        buttonscript = GameObject.Find("IconButton1").GetComponent<ButtonDataHandler>();
        currentPg = 0;
        tutorialpage1.enabled = false;
        tutorialpage2.enabled = false;
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
        if (leveldata.upgradetutorialPlayed == false)
        {
            if (currentPg == 0)
            {
                tutorialpage1.enabled = true;

            }

            if (currentPg == 1)
            {
                tutorialpage1.enabled = false;
            }
            if (currentPg == 2 & buttonscript.secondFrameOn == true)
            {
                tutorialpage2.enabled = true;
               
            }

            if (currentPg == 3)
            {
                tutorialpage2.enabled = false;
                leveldata.upgradetutorialPlayed = true;
                PlayerPrefs.SetString("UpgradeTutorialPlayed", "Upgrade Tutorial has been played");
            }

        }
    }
}
