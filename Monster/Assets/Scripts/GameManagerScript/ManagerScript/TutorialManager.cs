using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public Image tutorialpage1;
    public Image tutorialpage2;
    private bool pg1isActive;
    private bool pg2isActive;

    [SerializeField] int currentPg;
    public GameManagerScript gamemanager;
    // Start is called before the first frame update
    void Start()
    {
        currentPg = 0;
        gamemanager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
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
        if (gamemanager.activatePlayer == true)
        {
            if(currentPg == 1)
            {
                Time.timeScale = 0;
                tutorialpage1.enabled = true;
                
            }

            if(currentPg == 2)
            {
                tutorialpage1.enabled = false;
                tutorialpage2.enabled = true;
            }
            if(currentPg == 3)
            {
                tutorialpage2.enabled = false;
                Time.timeScale = 1;
            }
        }
    }
}
