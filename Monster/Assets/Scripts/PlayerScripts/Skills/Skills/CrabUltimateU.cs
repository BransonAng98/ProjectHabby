using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrabUltimateU : UltimateBase
{
    PlayerHandler playerHandler;

    [SerializeField] bool isActivated;
    [SerializeField] bool ultEnd;
    public PlayerHealthScript healthScript;
    public float ultimateDuration;
    public float timeReduction;
    public float currentDuration;
    [SerializeField] bool isTriggered;
    private PlayerVFXManager vfxManager;
    [SerializeField] GameObject joystick;

    // Start is called before the first frame update
    void Start()
    {
        playerHandler = GetComponent<PlayerHandler>();
        healthScript = GetComponent<PlayerHealthScript>();
        vfxManager = GetComponent<PlayerVFXManager>();
        joystick = GameObject.Find("Floating Joystick");
    }

    // Update is called once per frame
    void Update()
    {
        //Trigger ultimate countdown when its activated
        if (isActivated)
        {
            if(currentDuration > 0)
            {
                currentDuration -= timeReduction * Time.deltaTime;
                healthScript.riseOrFall = false;
                playerHandler.DecreaseUltimateBar(28f);
            }

            else
            {
                currentDuration = 0f;
                //joystick.SetActive(false);
                ultEnd = true;
                EndOfUltimate();
            }
        }
    }

    public override void UseUtilityUltimate()
    {
        if (!isTriggered)
        {
            base.UseUtilityUltimate();
            //Put all the variables & effects that would happen during the dash
            isTriggered = true;
            currentDuration = ultimateDuration;
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
        if (ultEnd)
        {
            vfxManager.SpawnExhaustedVFX();
            //Revert the player's stats & all changes back to normal state
            playerHandler.isDashing = false;
            playerHandler.DisableMovement(4);
            playerHandler.listOfEnemies.Clear();
            vfxManager.isDashing = false;
            currentDuration = 0f;
            healthScript.healthState = PlayerHealthScript.HealthState.normal;
            playerHandler.AlterStats(false, 3, 4f);
            playerHandler.AlterStats(false, 4, 10f);
            healthScript.activateAbiliityBar = true;
            isActivated = false;
            playerHandler.canEarnUlt = true;
            vfxManager.dashBodyVFX.SetActive(false);
            isTriggered = false;
            Invoke("EnableMovement", 2f);
            ultEnd = false;

        }
        else
        {
            return;
        }
    }

    void EnableMovement()
    {
        //joystick.SetActive(true);
        playerHandler.enableInput = true;
        playerHandler.canMove = true;
        playerHandler.canAttack = true;
        //playerHandler.IdleOrMove();
        vfxManager.DeTrigger();
    }
}
