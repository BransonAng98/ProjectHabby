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
   [SerializeField] private bool isLoading;

    private int currentIndex = 0;
    void Start()
    {
        
    }

    void Update()
    {
        //GoToGame();
        TurnPage();
        if(currentIndex == 1 & isLoading == true)
        {
            //GoToGame();
            Invoke("GoToGame", 2f);
            isLoading = false;
        }
    }

    public void TurnPage()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentIndex == 0)
            {
                ComicPanels[0].SetActive(false);
                ComicPanels[1].SetActive(true);
                isLoading = true;
            }

            // Activate the new current panel
            currentIndex = (currentIndex + 1) % ComicPanels.Length;
            ComicPanels[currentIndex].SetActive(true);
        }
    }


    public void GoToGame()
    {
        levelData.cityLevel = 0;
        //SceneManager.LoadScene("France_Tutorial_Level");
        SceneManager.LoadSceneAsync("France_Tutorial_Level");
    }


}

