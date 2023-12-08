using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComicManager : MonoBehaviour
{
    public GameObject[] ComicPanels;
    public GameObject cover;
    public Animator coveranim;
    public LevelManagerScriptableObject levelData;

    private int currentIndex = 0;
    void Start()
    {
       
        // Activate only the first comic panel at the start
        for (int i = 0; i < ComicPanels.Length; i++)
        {
            ComicPanels[i].SetActive(i == 0);
        }
    }

    void Update()
    {
        TurnPage();
    }

    public void TurnPage()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentIndex == 0)
            {
             
                coveranim.SetBool("Covered", true);

            }
            // Check if the current panel is the 3rd one in the list
            if (currentIndex == 1)
            {
             
                ComicPanels[0].SetActive(false);
                coveranim.SetBool("Covered", false);

            }
            if (currentIndex == 2)
            {
                // Deactivate the 1st and 2nd panels
                ComicPanels[1].SetActive(false);
                coveranim.SetBool("Covered", true);
                //cover.SetActive(false);


            }

            if (currentIndex == 3)
            {
                GoToGame();
            }

            // Activate the new current panel
            currentIndex = (currentIndex + 1) % ComicPanels.Length;
            ComicPanels[currentIndex].SetActive(true);
        }
    }

    public void TurnAuto()
    {
        if (currentIndex == 0)
        {

            coveranim.SetBool("Covered", true);

        }
        // Check if the current panel is the 3rd one in the list
        if (currentIndex == 1)
        {

            ComicPanels[0].SetActive(false);
            coveranim.SetBool("Covered", false);

        }
        if (currentIndex == 2)
        {
            // Deactivate the 1st and 2nd panels
            ComicPanels[1].SetActive(false);
            coveranim.SetBool("Covered", true);
            //cover.SetActive(false);


        }

        if (currentIndex == 3)
        {
            GoToGame();
        }

        // Activate the new current panel
        currentIndex = (currentIndex + 1) % ComicPanels.Length;
        ComicPanels[currentIndex].SetActive(true);
    }

    void GoToGame()
    {
        if (levelData.cityLevel < 10)
        {
            SceneManager.LoadScene("France_Hard_Level");
        }
        else if(levelData.cityLevel == 10)
        {
            SceneManager.LoadScene("LandmarkDesScene");
        }
    }
}
