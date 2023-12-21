using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScoreDisplayScript : MonoBehaviour
{
    public TextMeshProUGUI structuresText;
    public TextMeshProUGUI civiliansText;
    public TextMeshProUGUI carsText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI goldText;
    public ResourceScriptableObject goldData;
    public AudioManagerScript audiomanager;

    [SerializeField] private int structureamt;
    [SerializeField] private int civilianamt;
    [SerializeField] private int caramt;
    [SerializeField] private float timeamt;
    [SerializeField] private int goldamt;

    public ClockSystem clock;
    private float lerpDuration = 2.0f;
    public ScoreManagerScript scoreManager;
    public GameManagerScript gamemanager;
    public bool isWin;
    //public string formattedTime;
    private void Start()
    {
     
        gamemanager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManagerScript>();
        audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        SetActiveScreen();
        StartCoroutine(LerpScores());
       

        structureamt = scoreManager.amtOfStructures;
        civilianamt = scoreManager.amtOfcivilians;
        caramt = scoreManager.amtOfCarskilled;
        timeamt = scoreManager.timeLeft;
        goldamt = scoreManager.goldearned;
        goldData.currentGold += goldamt;
        audiomanager.PlayPointCalculation();
    }

    private IEnumerator LerpScores()
    {
        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            float t = elapsedTime / lerpDuration;

            int structuresScore = Mathf.RoundToInt(Mathf.Lerp(0, scoreManager.amtOfStructures, t));
            int civiliansScore = Mathf.RoundToInt(Mathf.Lerp(0, scoreManager.amtOfcivilians, t));
            int carsScore = Mathf.RoundToInt(Mathf.Lerp(0, scoreManager.amtOfCarskilled, t));
            float timeScore = Mathf.RoundToInt(Mathf.Lerp(0, scoreManager.timeLeft, t));
            //formattedTime = clock.GetFormattedTime(timeScore);
            int gemsScore = Mathf.RoundToInt(Mathf.Lerp(0, scoreManager.goldearned, t));

           
            UpdateScoreUI(structuresScore, civiliansScore, carsScore, timeScore, gemsScore);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final scores are set correctly
        UpdateScoreUI(scoreManager.amtOfStructures, scoreManager.amtOfcivilians,
                       scoreManager.amtOfCarskilled, scoreManager.timeLeft , scoreManager.goldearned);
    }

    private void UpdateScoreUI(int structures, int civilians, int cars, float time, int gems)
    {
        structuresText.text = "" + structures;
        civiliansText.text = "" + civilians;
        carsText.text = "" + cars;
        float minutes = Mathf.FloorToInt(timeamt / 60);
        float seconds = Mathf.FloorToInt(timeamt % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        goldText.text = "" + gems;
        
    }

    public void SetActiveScreen()
    {
        // Example: Assuming you have two screens for victory and loss
        // Assign the appropriate TextMeshProUGUI components based on the active screen

        if( gamemanager.isVictory)
        {
            isWin = true;
            structuresText = GameObject.Find("winText_structure").GetComponent<TextMeshProUGUI>();
            civiliansText = GameObject.Find("winText_civilians").GetComponent<TextMeshProUGUI>();
            carsText = GameObject.Find("winText_cars").GetComponent<TextMeshProUGUI>();
            timeText = GameObject.Find("winText_time").GetComponent<TextMeshProUGUI>();
            goldText = GameObject.Find("winText_gems").GetComponent<TextMeshProUGUI>();
        }
        else
        {
            isWin = false;
            structuresText = GameObject.Find("loseText_structure").GetComponent<TextMeshProUGUI>();
            civiliansText = GameObject.Find("loseText_civilians").GetComponent<TextMeshProUGUI>();
            carsText = GameObject.Find("loseText_cars").GetComponent<TextMeshProUGUI>();
            timeText = GameObject.Find("loseText_time").GetComponent<TextMeshProUGUI>();
            goldText = GameObject.Find("loseText_gems").GetComponent<TextMeshProUGUI>();
        }
    }
}
