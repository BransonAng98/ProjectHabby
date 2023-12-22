using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScoreManagerScript : MonoBehaviour
{
    public int amtOfStructures;
    public int amtOfcivilians;
    public int amtOfCarskilled;
    public float timeLeft;
    public int goldearned;
    public TextMeshProUGUI goldDisplay;
  
    public int bigbuildingKilled;
    public int smallbuildingKilled;
    public ClockSystem clock;
    

    void Start()
    {
        clock = GameObject.Find("Timer").GetComponent<ClockSystem>();
        goldDisplay = GameObject.Find("CoinText").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft = clock.timerValue;
        //GoldCalculation();
        goldDisplay.text = "" + goldearned;

    }

    void GoldCalculation()
    {
        //goldearned = (amtOfcivilians * 1) + (amtOfCarskilled * 3) + (smallbuildingKilled * 5) + (bigbuildingKilled * 10);
    }
}
