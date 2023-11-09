using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventManager : MonoBehaviour
{
    [SerializeField] private int totalScore;
    [SerializeField] private int triggerThreshold;
    [SerializeField] private int currentScore;
    public int numberOfEvents;

    public TextMeshProUGUI bannerText;
    public LevelManager levelManagerScript;

    public AirStrike airStrikeScript;
    public Artillery artilleryScript;
    public MissileManager missileScript;
    [SerializeField] private bool gameStarted;

    // Start is called before the first frame update
    void Start()
    {
        airStrikeScript = GameObject.Find("AirStrikeSpawner").GetComponent<AirStrike>();
        artilleryScript = GameObject.Find("ArtySpawner").GetComponent<Artillery>();
        missileScript = GameObject.Find("MissileManager").GetComponent<MissileManager>();
        levelManagerScript = GameObject.Find("GameManager").GetComponent<LevelManager>();

        Invoke("GetScore", 1.1f);
    }


    void Update()
    {
        if (gameStarted)
        {
            if (currentScore > 0)
            {
                int randomEvent = Random.Range(0, 2); // 0 for bombing run, 1 for artillery
                if (randomEvent == 0)
                {
                    airStrikeScript.ActivateAirStrike();
                    bannerText.text = "Incoming AirStrike!";
                    currentScore = 0;
                }
                else
                {
                    artilleryScript.ActivateArtillery();
                    bannerText.text = "Incoming Barrage!";
                    currentScore = 0;
                }
            }
        }

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
