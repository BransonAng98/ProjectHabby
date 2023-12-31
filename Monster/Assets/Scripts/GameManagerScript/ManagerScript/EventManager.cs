using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventManager : MonoBehaviour
{
    public int numberOfEvents;
    public float eventTiming;
    public int eventNumber;

    public ResourceScriptableObject resourceData;
    public TextMeshProUGUI bannerText;
    public TextMeshProUGUI endStatusText;
    public LevelManager levelManagerScript;
    public PlayerHandler playerHandler;
    public GameManagerScript gameManager;
    public AirStrike airStrikeScript;
    public Artillery artilleryScript;
    public MissileManager missileScript;
    public PlayerHealthScript playerHealth;
    public AudioManagerScript audiomanager;
    public ClockSystem clock;
    public GameObject bannerObj;
    public HitCircle hitcircleScript;

    public GameObject endStatus;

    public float timer;

    private bool isEndStatusTriggered = false;
    // Start is called before the first frame update
    void Start()
    {
        audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        airStrikeScript = GameObject.Find("AirStrikeSpawner").GetComponent<AirStrike>();
        artilleryScript = GameObject.Find("ArtySpawner").GetComponent<Artillery>();
        levelManagerScript = GameObject.Find("GameManager").GetComponent<LevelManager>();
        playerHandler = GameObject.Find("CrabPlayer").GetComponent<PlayerHandler>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        playerHealth = GameObject.Find("CrabPlayer").GetComponent<PlayerHealthScript>();
        hitcircleScript = GameObject.Find("HitCircle").GetComponent<HitCircle>();


        timer = 0f;
    }


    void Update()
    {

        if (!isEndStatusTriggered)
        {
            GenerateEvents();
        }

        TriggerEndStatus();
        eventTiming = clock.eventInterval;
    }

    public void GenerateEvents()
    {
        if (gameManager.hasActivated == true && gameManager.gameEnded == false)
        {
            timer += Time.deltaTime;

            if (timer >= clock.eventInterval)
            {
                eventNumber = Random.Range(0, 2);

                switch (eventNumber)
                {
                    case 0:
                        PerformBannerFade(0.5f, 0.5f, 3f);
                        airStrikeScript.ActivateAirStrike();
                        Invoke("PlaySFX", 2f);
                        bannerText.text = "Incoming AirStrike!";
                        timer = 0f;
                        break;

                    case 1:
                        PerformBannerFade(0.5f, 0.5f, 3f);
                        artilleryScript.ActivateArtillery();
                        Invoke("PlaySFX", 1.5f);
                        bannerText.text = "Incoming Barrage!";
                        timer = 0f;
                        break;

                    default:
                        break;
                }

            }

        }

    }

    public void TriggerEndStatus()
    {
        endStatus.SetActive(false); // Ensure it starts deactivated
        PlayerPrefs.SetInt("PlayerMoney", resourceData.currentGold);
        PlayerPrefs.Save();
        if (gameManager.isVictory == false && playerHealth.healthSlider.value <= 0)
        {
            gameManager.gameEnded = true;
            isEndStatusTriggered = true;
            playerHandler.DisableMovement(3);
            playerHandler.DisableUltimateVFX();
            hitcircleScript.DisableHitCircleVFX();
            endStatus.SetActive(true);
            endStatusText.text = "";

            endStatusText.color = Color.red;
            endStatusText.text = "Monster Slain!";
        }
        else if (gameManager.isVictory == true)
        {
            gameManager.gameEnded = true;
            isEndStatusTriggered = true;
            playerHandler.DisableMovement(2);
            playerHandler.DisableUltimateVFX();
            hitcircleScript.DisableHitCircleVFX();
            endStatus.SetActive(true);
            endStatusText.text = "";

            endStatusText.color = Color.green;
            endStatusText.text = "Mission Complete!";
        }
        else if (clock.timerValue == 0 && gameManager.isVictory == false)
        {
            gameManager.gameEnded = true;
            isEndStatusTriggered = true;
            playerHandler.DisableMovement(3);
            playerHandler.DisableUltimateVFX();
            hitcircleScript.DisableHitCircleVFX();
            endStatus.SetActive(true);
            endStatusText.text = "";

            endStatusText.color = Color.red;
            endStatusText.text = "Time's Up!";
        }
    }
    public void PlaySFX()
    {
        audiomanager.PlayIncomingAbility();
    }


    public void PerformBannerFade(float fadeInDuration, float fadeOutDuration, float waitDuration)
    {
        StartCoroutine(FadeBannerInOut(fadeInDuration, fadeOutDuration, waitDuration));
    }

    IEnumerator FadeBannerInOut(float fadeInDuration, float fadeOutDuration, float waitDuration)
    {
        
        yield return StartCoroutine(FadeInBanner(fadeInDuration));

        // Wait for a short duration
        yield return new WaitForSeconds(waitDuration);

        yield return StartCoroutine(FadeOutBanner(fadeOutDuration));
        // Reset the flag after fading
        isEndStatusTriggered = false;
    }


    IEnumerator FadeInBanner(float duration)
    {
        audiomanager.PlayWoosh1SFX();
        CanvasGroup canvasGroup = bannerObj.GetComponent<CanvasGroup>();

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // Ensure it reaches the target alpha exactly
        canvasGroup.alpha = 1f;
        audiomanager.PlayWarningSFX();
    }

    IEnumerator FadeOutBanner(float duration)
    {
        audiomanager.PlayWoosh2SFX();
        CanvasGroup canvasGroup = bannerObj.GetComponent<CanvasGroup>();

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        // Ensure it reaches the target alpha exactly
        canvasGroup.alpha = 0f;
    }


}

