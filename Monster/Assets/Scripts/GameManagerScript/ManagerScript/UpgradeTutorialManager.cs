using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeTutorialManager : MonoBehaviour
{
    public GameObject tutorialpage1;
    public GameObject tutorialpage2;
    public LevelManagerScriptableObject leveldata;
    public ButtonDataHandler buttonscript;

    [SerializeField] int currentPg;
    // Start is called before the first frame update
    void Start()
    {
        buttonscript = GameObject.Find("IconButton1").GetComponent<ButtonDataHandler>();
        currentPg = 0;
        tutorialpage1.SetActive(false)  ;
        tutorialpage2.SetActive(false);
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
                tutorialpage1.SetActive(true);

            }

            if (currentPg == 1)
            {
                tutorialpage1.SetActive(false);
            }
            if (currentPg == 2 & buttonscript.secondFrameOn == true)
            {
                tutorialpage2.SetActive(true);

            }

            if (currentPg == 3)
            {
                tutorialpage2.SetActive(false);
                leveldata.upgradetutorialPlayed = true;
                PlayerPrefs.SetString("UpgradeTutorialPlayed", "Upgrade Tutorial has been played");
            }

        }
    }
}
