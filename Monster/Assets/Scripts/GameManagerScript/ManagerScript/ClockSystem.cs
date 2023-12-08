using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClockSystem : MonoBehaviour
{
    public LevelManagerScriptableObject levelData;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI timerWarningText;
    public GameObject countDownBG;
    public Image vignette;
    private GameManagerScript gameManager;
    [SerializeField] private PlayerHandler playerHandler;
    [SerializeField] private EventManager eventManager;
    private float timerValue;
    private float addOnTime;
    
    public bool startTime;
    private bool thirtySecondsMessageDisplayed = false;
    private bool timeUpMessageDisplayed = false;
    private bool isfinalSecondsLeft = false;

    private Color normalColor = Color.white;
    private float enlargedFontSize = 60f;
    private Color enlargedColor = Color.red;

    public float flashSpeed;
    public float timeSpeed;

    public float eventInterval;

    // Start is called before the first frame update
    void Start()
    {
        CalculateLevelTime();
        CalculateEventInterval();

        timerText = GetComponent<TextMeshProUGUI>();
        countDownBG.gameObject.SetActive(false);

        DisplayTime(timerValue);
        startTime = false;
        vignette.enabled = false;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerScript>();
        playerHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        eventManager = GameObject.FindGameObjectWithTag("EventManager").GetComponent<EventManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startTime && playerHandler.enableInput == true)
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
                playerHandler.isEnd = true;
                playerHandler.DisableMovement(3);
                Invoke("DelayEndScreen", 5f);
            }
        }

        else { return; }

        if (isfinalSecondsLeft)
        {
            timerText.color = Color.Lerp(normalColor, enlargedColor, Mathf.PingPong(Time.time * flashSpeed, 1f));
            vignette.enabled = true;

            float alpha = Mathf.PingPong(Time.time * flashSpeed, 1f);

            alpha = Mathf.Lerp(0f, 1f, alpha);

            Color vignetteColor = vignette.color;
            vignetteColor.a = alpha;
            vignette.color = vignetteColor;
        }
        
    }
        

    void DelayEndScreen()
    {
        gameManager.isVictory = false;
        gameManager.TriggerEndScreen();
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
            isfinalSecondsLeft = true;
            float newTime = remainingTime - 1;
            timerWarningText.text = Mathf.CeilToInt(newTime).ToString();
            timerText.fontSize = enlargedFontSize;
        }

        else
        {
            if (remainingTime <= 30 && remainingTime > 20 && !thirtySecondsMessageDisplayed)
            {
                countDownBG.gameObject.SetActive(true);
                timerWarningText.text = "";
                timerWarningText.text += "30 Seconds!";
                thirtySecondsMessageDisplayed = true;
                Invoke("TurnOffText", 3f);
            }
           

            else if (remainingTime <= 0 && !timeUpMessageDisplayed)
            {
                countDownBG.gameObject.SetActive(true);
                timerWarningText.text = "";
                timerWarningText.text = "Time's up!";
                timeUpMessageDisplayed = true;
                Invoke("TurnOffText", 3f);
            }
            
        }
    }
    void CalculateEventInterval()
    {
        // Calculate the event interval based on the level duration and number of events
        eventInterval = timerValue / eventManager.numberOfEvents;
    }

    void TurnOffText()
    {
        countDownBG.gameObject.SetActive(false);
    }
}
