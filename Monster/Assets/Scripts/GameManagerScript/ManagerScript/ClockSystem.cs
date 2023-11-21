using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClockSystem : MonoBehaviour
{
    public LevelManagerScriptableObject levelData;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI countDownText;
    public GameObject countDownBG;
    private GameManagerScript gameManager;

    private float timerValue;
    private float addOnTime;
    
    public bool startTime;
    private bool minuteMessageDisplayed = false;
    private bool thirtySecondsMessageDisplayed = false;
    private bool timeUpMessageDisplayed = false;

    public float timeSpeed;
    // Start is called before the first frame update
    void Start()
    {
        CalculateLevelTime();
        timerText = GetComponent<TextMeshProUGUI>();
        countDownBG.gameObject.SetActive(false);

        DisplayTime(timerValue);
        startTime = false;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startTime)
        {
            if (timerValue > 0)
            {
                timerValue -= timeSpeed * Time.deltaTime;
                DisplayTime(timerValue);
                DisplayCountdownMessages(timerValue);
            }
            else
            {
                timerValue = 0;
                gameManager.isVictory = false;
                gameManager.TriggerEndScreen();
            }
        }

        else { return; }
    }

    public void CalculateLevelTime()
    {
        switch (levelData.cityLevel)
        {
            case 1:
                addOnTime = 0;
                break;

            case 2:
                addOnTime = 30;
                break;

            case 3:
                addOnTime = 45;
                break;

            case 4:
                addOnTime = 60;
                break;

            case 5:
                addOnTime = 90;
                break;

            case 6:
                addOnTime = 105;
                break;

            case 7:
                addOnTime = 120;
                break;

            case 8:
                addOnTime = 135;
                break;

            case 9:
                addOnTime = 150;
                break;

            case 10:
                addOnTime = 160;
                break;
        }

        timerValue = levelData.baseTime + addOnTime;
    }

    void DisplayTime(float timeToDisplay)
    {
        if(timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void DisplayCountdownMessages(float remainingTime)
    {
        // Display countdown numbers for the last 10 seconds
        if (remainingTime <= 11 && remainingTime > 0)
        {
            float newTime = remainingTime - 1;
            countDownText.text = Mathf.CeilToInt(newTime).ToString();
            countDownBG.gameObject.SetActive(true);
        }

        else
        {
            if (remainingTime <= 60 && remainingTime > 50 && !minuteMessageDisplayed)
            {
                countDownBG.gameObject.SetActive(true);
                countDownText.text = "";
                countDownText.text += "1 Minute Remaining!";
                minuteMessageDisplayed = true;
                Invoke("TurnOffText", 3f);

            }
            else if (remainingTime <= 30 && remainingTime > 20 && !thirtySecondsMessageDisplayed)
            {
                countDownBG.gameObject.SetActive(true);
                countDownText.text = "";
                countDownText.text += "30 Seconds Remaining!";
                thirtySecondsMessageDisplayed = true;
                Invoke("TurnOffText", 3f);
            }

            else if (remainingTime <= 0 && !timeUpMessageDisplayed)
            {
                countDownBG.gameObject.SetActive(true);
                countDownText.text = "";
                countDownText.text = "Time's up!";
                timeUpMessageDisplayed = true;
                Invoke("TurnOffText", 3f);
            }
            
        }
    }

    void TurnOffText()
    {
        countDownBG.gameObject.SetActive(false);
    }
}
