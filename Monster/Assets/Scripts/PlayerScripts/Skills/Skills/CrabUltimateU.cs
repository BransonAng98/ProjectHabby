using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrabUltimateU : UltimateBase
{
    PlayerHandler playerHandler;

    public TextMeshProUGUI dashTimer;

    [SerializeField] bool isActivated;
    public float ultimateDuration;
    public float timeReduction;
    public float currentDuration;
    [SerializeField] bool isTriggered;
    // Start is called before the first frame update
    void Start()
    {
        playerHandler = GetComponent<PlayerHandler>();
        dashTimer = GameObject.Find("DashTimer").GetComponent<TextMeshProUGUI>();
        dashTimer.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(dashTimer != null)
        {
            if (dashTimer.gameObject.activeSelf)
            {
                //Make the timer follow the player and have it be at the top right corner of the player
                dashTimer.transform.position = Camera.main.WorldToScreenPoint(playerHandler.transform.position);
                dashTimer.text = Mathf.CeilToInt(currentDuration).ToString();
            }

            else
            {
                return;
            }
        }

        else
        {
            return;
        }

        //Trigger ultimate countdown when its activated
        if (isActivated)
        {
            if(currentDuration > 0)
            {
                currentDuration -= timeReduction * Time.deltaTime;
            }

            else
            {
                currentDuration = 0f;
                EndOfUltimate();
            }
        }
    }

    public override void UseUtilityUltimate()
    {
        if (!isTriggered)
        {
            base.UseUtilityUltimate();
            playerHandler.currentUltimateCharge = 0f;
            //Put all the variables & effects that would happen during the dash
            isTriggered = true;
            currentDuration = ultimateDuration;
            dashTimer.gameObject.SetActive(true);
            playerHandler.AlterStats(true, 3, 4f);
            playerHandler.AlterStats(true, 4, 10f);
            isActivated = true;
        }

        else
        {
            return;
        }
    }

    void EndOfUltimate()
    {
        //Revert the player's stats & all changes back to normal state
        currentDuration = 0f;
        playerHandler.isDashing = false;
        playerHandler.AlterStats(false, 3, 4f);
        playerHandler.AlterStats(false, 4, 10f);
        playerHandler.RevertState();
        isActivated = false;
        playerHandler.canAttack = true;
        playerHandler.canEarnUlt = true;
        dashTimer.gameObject.SetActive(false);
        isTriggered = false;
    }
}
