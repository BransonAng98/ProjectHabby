using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public LevelManagerScriptableObject levelData;
    private int calculateCityDestruction;
    public int calculation1;
    public Slider slider;
    private GameManagerScript gameManager;
    public CutSceneManager cutsceneManager;
    public PlayerHandler playerHandler;

    [SerializeField] private bool isTriggered;

    // Start is called before the first frame update
    void Start()
    {   
        Invoke("CalculateTotalDestruction", 1f);
        gameManager = GetComponent<GameManagerScript>();
        cutsceneManager = GameObject.FindGameObjectWithTag("VictoryScreen").GetComponent<CutSceneManager>();
        playerHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        isTriggered = false;
    }

    public void CalculateTotalDestruction()
    {
        switch (levelData.cityLevel)
        {
            case 1:
                calculateCityDestruction = 1;
                break;

            case 2:
                calculateCityDestruction = 2;
                break;

            case 3:
                calculateCityDestruction = 2;
                break;

            case 4:
                calculateCityDestruction = 3;
                break;

            case 5:
                calculateCityDestruction = 3;
                break;

            case 6:
                calculateCityDestruction = 3;
                break;

            case 7:
                calculateCityDestruction = 4;
                break;

            case 8:
                calculateCityDestruction = 4;
                break;

            case 9:
                calculateCityDestruction = 5;
                break;

            case 10:
                calculateCityDestruction = 5;
                break;
        }

        calculation1 = levelData.baseScore * calculateCityDestruction;
        slider.maxValue = calculation1;
    }

    public void CalculateScore(float score)
    {
        slider.value += score;
        levelData.currentDestruction = slider.value;
        CalculateProgress();
        if(slider.value == slider.maxValue)
        {
            if (!isTriggered)
            {
                playerHandler.isEnd = true;
                playerHandler.DisableMovement(2);
                levelData.cityLevel += 1;
                levelData.destructionLevel = 0;
                gameManager.isVictory = true;
                cutsceneManager.TriggerEnd();
                isTriggered = true;
            }
        }
    }

    public void TapToLeave()
    {
        SceneManager.LoadScene("LevelSelectScene");
    }

    private void ChangeLevel()
    {
        SceneManager.LoadScene("LevelSelectScene");
    }

    public void CalculateProgress()
    {
        float destructionProgress = Mathf.Round((levelData.currentDestruction / calculation1) * 100f);
        if (destructionProgress >= 0 && destructionProgress <= 30)
        {
            levelData.destructionLevel = 0;
          
        }

        else if (destructionProgress >= 31 && destructionProgress <= 70)
        {
            levelData.destructionLevel = 1;
        }

        else { levelData.destructionLevel = 2; }
    }
}
