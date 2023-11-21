using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventManager : MonoBehaviour
{
    [SerializeField] private int totalScore;
    public int triggerThreshold;
    public int currentScore;
    public int numberOfEvents;
    public int eventNumber;

    public TextMeshProUGUI bannerText;
    public LevelManager levelManagerScript;
    public PlayerHandler playerHandler;

    public AirStrike airStrikeScript;
    public Artillery artilleryScript;
    public MissileManager missileScript;
    [SerializeField] private bool gameStarted;

    public AudioManagerScript audiomanager;

    // Start is called before the first frame update
    void Start()
    {
        audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        airStrikeScript = GameObject.Find("AirStrikeSpawner").GetComponent<AirStrike>();
        artilleryScript = GameObject.Find("ArtySpawner").GetComponent<Artillery>();
        missileScript = GameObject.Find("MissileManager").GetComponent<MissileManager>();
        levelManagerScript = GameObject.Find("GameManager").GetComponent<LevelManager>();
        playerHandler = GameObject.Find("CrabPlayer").GetComponent<PlayerHandler>();

        Invoke("GetScore", 1.1f);
    }


    void Update()
    {

        GenerateEvents();
    }

    public void GenerateEvents()
    {
        if (gameStarted && playerHandler.isEnd != true)
        {
            if (currentScore >= triggerThreshold)
            {
                eventNumber = Random.Range(0, 2);

                switch (eventNumber)
                {
                    case 0:
                        airStrikeScript.ActivateAirStrike();
                        Invoke("PlaySFX", 6f);
                        bannerText.text = "Incoming AirStrike!";
                        currentScore = 0;
                        break;
                    case 1:
                        artilleryScript.ActivateArtillery();
                        Invoke("PlaySFX", 6f);
                        bannerText.text = "Incoming Barrage!";
                        currentScore = 0;
                        break;
                    /*case 2:
                        missileScript.StartEvent();
                        bannerText.text = "Incoming Missiles!";
                        currentScore = 0;
                        break;*/
                    default:
                        break;
                }
            }
        }

    }
    
    public void PlaySFX()
    {
        audiomanager.PlayIncomingAbility();
    }

    void GetScore()
    {
        totalScore = levelManagerScript.calculation1;
        triggerThreshold = totalScore / numberOfEvents;
        gameStarted = true;
    }

    public void AddScore()
    {
        currentScore += 1;
    }
}
