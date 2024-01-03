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
    public TextMeshProUGUI timeOutText;
    public Image clockSprite;
    public Image vignette;

    [SerializeField] private GameManagerScript gameManager;
    [SerializeField] private PlayerHandler playerHandler;
    [SerializeField] private EventManager eventManager;

    public float timerValue;
    private float addOnTime;
    
    public bool startTime;
    private bool thirtySecondsMessageDisplayed = false;
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

        DisplayTime(timerValue);
        startTime = false;
        vignette.enabled = false;
        clockSprite.enabled = false;
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
                timeOutText.text = "";
                timeOutText.text = "OUT OF TIME!";
                Invoke("DelayEndScreen", 3f);
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

    public void ActivateTimeWarning()
    {
        clockSprite.enabled = true;
    }

    public void DeactivateTimeWarning()
    {
        clockSprite.enabled = false;
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
            case 0:
                addOnTime = 0;
                break;

            case 1:
                addOnTime = 20;
                break;

            case 2:
                //Prev was 30
                addOnTime = 30;
                break;

            case 3:
                //Prev was 45
                addOnTime = 50;
                break;

            case 4:
                //Prev was 60
                addOnTime = 70;
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

    public string GetFormattedTime(float timeToFormat)
    {
        if (timeToFormat < 0)
        {
            timeToFormat = 0;
        }

        float minutes = Mathf.FloorToInt(timeToFormat / 60);
        float seconds = Mathf.FloorToInt(timeToFormat % 60);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void DisplayCountdownMessages(float remainingTime)
    {
        // Display countdown numbers for the last 10 seconds
        if (remainingTime <= 11 && remainingTime > 0)
        {
            isfinalSecondsLeft = true;
            float newTime = remainingTime - 1;
            timerText.fontSize = enlargedFontSize;
        }

        else
        {
            if (remainingTime <= 32 && remainingTime > 20 && !thirtySecondsMessageDisplayed)
            {
                eventManager.PerformBannerFade(0.5f, 0.5f, 3f);
                ActivateTimeWarning();
                timerWarningText.text = "";
                timerWarningText.text += "30 SECONDS!";
                thirtySecondsMessageDisplayed = true;
                Invoke("DeactivateTimeWarning", 2.5f);
            }
          
        }
    }
    void CalculateEventInterval()
    {
        // Calculate the event interval based on the level duration and number of events
        eventInterval = timerValue / eventManager.numberOfEvents;
    }


}
