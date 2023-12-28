using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialpage1;
    public LevelManagerScriptableObject leveldata;

    [SerializeField] int currentPg;
    public GameManagerScript gamemanager;
    // Start is called before the first frame update
    void Start()
    {
        currentPg = 0;
        gamemanager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
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
        if (gamemanager.activatePlayer == true)
        {
            if(currentPg == 1)
            {
                Time.timeScale = 0;
                tutorialpage1.SetActive(true);

            }

            if(currentPg == 2)
            {
                tutorialpage1.SetActive(false);
                Time.timeScale = 1;
                leveldata.tutorialPlayed = true;
                PlayerPrefs.SetString("TutorialPlayed", "Tutorial has been played");
            }
           
        }
    }
}
