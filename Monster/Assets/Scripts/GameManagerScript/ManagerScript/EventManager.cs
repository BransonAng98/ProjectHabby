using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventManager : MonoBehaviour
{
    public int numberOfEvents;
    public int eventNumber;

    public TextMeshProUGUI bannerText;
    public LevelManager levelManagerScript;
    public PlayerHandler playerHandler;
    public GameManagerScript gameManager;
    public AirStrike airStrikeScript;
    public Artillery artilleryScript;
    public MissileManager missileScript;

    public AudioManagerScript audiomanager;
    public ClockSystem clock;

    public float timer;

    // Start is called before the first frame update
    void Start()
    {
        audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        airStrikeScript = GameObject.Find("AirStrikeSpawner").GetComponent<AirStrike>();
        artilleryScript = GameObject.Find("ArtySpawner").GetComponent<Artillery>();
        missileScript = GameObject.Find("MissileManager").GetComponent<MissileManager>();
        levelManagerScript = GameObject.Find("GameManager").GetComponent<LevelManager>();
        playerHandler = GameObject.Find("CrabPlayer").GetComponent<PlayerHandler>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        timer = 0f;
    }


    void Update()
    {
        GenerateEvents();
    }

    public void GenerateEvents()
    {
        if (gameManager.hasActivated == true)
        {
            timer += Time.deltaTime;
            
            if (timer >= clock.eventInterval)
            {
                eventNumber = Random.Range(0, 2);

                switch (eventNumber)
                {
                    case 0:
                        Debug.Log("boom");
                        airStrikeScript.ActivateAirStrike();
                        Invoke("PlaySFX", 2f);
                        bannerText.text = "Incoming AirStrike!";
                        timer = 0f;
                        break;
                    case 1:
                        artilleryScript.ActivateArtillery();
                        Invoke("PlaySFX", 6f);
                        bannerText.text = "Incoming Barrage!";
                        timer = 0f;
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

}
