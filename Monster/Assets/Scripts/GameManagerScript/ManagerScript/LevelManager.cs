using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public LevelManagerScriptableObject levelData;
    public PlayerStatScriptableObject playerData;
    public ResourceScriptableObject resourceData;
    private float calculateCityDestruction;
    public float calculation1;

    public Slider objSlider;
    public Image frontSlider;

    private float lerpSpeed = 0.1f;

    private GameManagerScript gameManager;
    public CutSceneManager cutsceneManager;
    public PlayerHandler playerHandler;

    [SerializeField] private bool isTriggered;
    [SerializeField] private float targetScore;

    // Start is called before the first frame update
    private void Awake()
    {
        CalculateTotalDestruction();
    }
    void Start()
    {   
        //Invoke("CalculateTotalDestruction", 1f);
        gameManager = GetComponent<GameManagerScript>();
        cutsceneManager = GameObject.FindGameObjectWithTag("VictoryScreen").GetComponent<CutSceneManager>();
        playerHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        isTriggered = false;
    }

    private void Update()
    {
        LerpDestructionBar();
    }

    public void CalculateTotalDestruction()
    {
        switch (levelData.cityLevel)
        {
            case 1:
                calculateCityDestruction = 20;
                break;

            case 2:
                calculateCityDestruction = 450f;
                break;

            case 3:
                calculateCityDestruction = 750f;
                break;

            case 4:
                calculateCityDestruction = 1350f;
                break;
            //Loop the final level
            case 6:
                calculateCityDestruction = 1500f;
                break;
        }

        calculation1 = levelData.baseScore + calculateCityDestruction;
        objSlider.maxValue = calculation1;
        Debug.Log(calculation1);
    }

    public void CalculateScore(float score)
    {
        objSlider.value += score;
        levelData.currentDestruction = objSlider.value;
        CalculateProgress();
        if(objSlider.value == objSlider.maxValue)
        {
            if (!isTriggered)
            {
                playerHandler.isEnd = true;
                playerHandler.DisableMovement(2);

                if(levelData.cityLevel < 6)
                {
                    levelData.cityLevel += 1;
                }

                else
                {
                    levelData.cityLevel = 6;
                }
                playerData.levelProgress++;
                PlayerPrefs.SetInt("CityLevelStored", levelData.cityLevel);
                PlayerPrefs.SetInt("LevelProgress", playerData.levelProgress);
                PlayerPrefs.SetInt("PlayerMoney", resourceData.currentGold);
                PlayerPrefs.Save();
                levelData.destructionLevel = 0;
                gameManager.isVictory = true;
                isTriggered = true;
                Invoke("TriggerVictoryScreen", 3f);
            }
        }
    }

    void TriggerVictoryScreen()
    {
        cutsceneManager.TriggerEnd();
    }

    void LerpDestructionBar()
    {
        float normalizedDestruction = objSlider.value / objSlider.maxValue;
        frontSlider.fillAmount = Mathf.MoveTowards(frontSlider.fillAmount, normalizedDestruction, lerpSpeed * Time.deltaTime);
    }

    public void TapToLeave()
    {
        PlayerPrefs.Save();
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
