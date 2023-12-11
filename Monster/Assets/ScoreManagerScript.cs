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
    public int gemsEarned;

    public ClockSystem clock;

    void Start()
    {
        clock = GameObject.Find("Timer").GetComponent<ClockSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft = clock.timerValue;
    }
}
