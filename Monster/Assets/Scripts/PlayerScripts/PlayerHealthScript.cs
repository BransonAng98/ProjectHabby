using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealthScript : MonoBehaviour
{
    public enum HealthState
    {
        normal,
        berserk,
    }

    public PlayerStatScriptableObject playerSO;
    public Slider healthSlider;
    public Slider abilitySlider;
    public Image healthFill;
    public Image abilityFill;
    public float lerpSpeed = 2f;

    private ShakeScript shakeMe;
    private float currentHealth;
    public HealthState healthState;

    private GameManagerScript gameManager;
    private PlayerHandler playerHandler;

    //Flash Effect
    private PlayerFlash flashEffect;
    private int thresholdHealth;
    public int triggerNumber;

    //Berserk mode Feedback
    private CanvasGroup berserkVignette;
    public CutSceneManager cutsceneManager;

    [SerializeField] private bool isTriggered;
    
    private void Start()
    {
        shakeMe = healthSlider.gameObject.GetComponent<ShakeScript>();
        //playerSO.health = 100;
        currentHealth = playerSO.health; // Set initial health to full
        healthSlider.maxValue = currentHealth;
        thresholdHealth = playerSO.health;
        flashEffect = GetComponent<PlayerFlash>();
        UpdateHealthBar();

        healthState = HealthState.normal;

        berserkVignette = GameObject.Find("Vignette").GetComponent<CanvasGroup>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerScript>();
        playerHandler = GetComponent<PlayerHandler>();

        cutsceneManager = GameObject.FindGameObjectWithTag("VictoryScreen").GetComponent<CutSceneManager>();
    }

    void TriggerVignette()
    {
        if(healthState != HealthState.berserk)
        {
            if (berserkVignette.alpha >= 0)
            {
                
                berserkVignette.alpha -= Time.deltaTime;
            }
        }

        else
        {
            if (!isTriggered)
            {
                playerHandler.DisableMovement(1);
                isTriggered = true;
            }

            if (berserkVignette.alpha < 1)
            {
                berserkVignette.alpha += Time.deltaTime;
            }
        }
    }

    private void Update()
    {
        if (currentHealth != playerSO.health)
        {
            // Lerp towards the target health value
            currentHealth = Mathf.Lerp(currentHealth, playerSO.health, Time.deltaTime * lerpSpeed);
            UpdateHealthBar();
        }

        TriggerVignette();
        UpdateAbilityBar();
    }

    void CheckHealthStatus(float playerhealth)
    {
        int healthPercentage = (int)(100 - ((100f / playerSO.maxhealth) * currentHealth));

        if (healthPercentage <= 45)
        {
            healthState = HealthState.normal;
        }

        else
        {
            healthState = HealthState.berserk;
        }
    }

    public void TakeDamage(int damage)
    {
        shakeMe.StartShake();
        CheckHealthStatus(currentHealth);
        if(playerSO.health > 0)
        {
            if (healthState == HealthState.normal)
            {
                playerSO.health -= damage; // Adjust the damage amount as needed
            }

            else
            {
                playerSO.health -= damage * 2; // Player takes double damage when they are in berserk mode
            }

            int healthDifference = thresholdHealth - playerSO.health;
            if (healthDifference >= triggerNumber)
            {
                //flashEffect.Flash();
                thresholdHealth = playerSO.health;
            }
        }

        else
        {
            playerHandler.isEnd = true;
            playerHandler.DisableMovement(3);
            gameManager.isVictory = false;
            cutsceneManager.TriggerEnd();

        }
    }

    public void RecoverHealth(int recover)
    {
        if (playerSO.health <= playerSO.maxhealth)
        {
            playerSO.health += recover; // Adjust the damage amount as needed
        }
        else
        {
            return;
        }
    }

    private void UpdateAbilityBar()
    {
        abilitySlider.value = playerHandler.currentUltimateCharge; // Update the slider's value
        abilityFill.fillAmount = playerHandler.currentUltimateCharge; // Update the fill amount of the health bar
    }

    private void UpdateHealthBar()
    {
        healthSlider.value = currentHealth; // Update the slider's value
        healthFill.fillAmount = currentHealth; // Update the fill amount of the health bar
    }


}
