using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Spine;
using Spine.Unity;
using Haptics.Vibrations;
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
    private float lastPainRoarTime; // Variable to store the time of the last pain roar
    private float painRoarCooldown = 5f; // Adjust the cooldown duration as needed

    private ObjectShakeScript shakeScript;
    private float currentHealth;
    public HealthState healthState;
    [SerializeField] HealthState currentState;

    private GameManagerScript gameManager;
    private PlayerHandler playerHandler;
    private PlayerVFXManager vfxManager;

    //Flash Effect
    private PlayerFlash flashEffect;
    private int thresholdHealth;
    public int triggerNumber;

    //Berserk mode Feedback
    public CutSceneManager cutsceneManager;
    [SerializeField] private float ogValues;
    [SerializeField] private float ogAtkSpeed;
    [SerializeField] private bool isTriggered;

    [SerializeField] private SkeletonAnimation meshRenderer;
    public Material originalMat;
    public Material rageMat;

    //AbilityBar Variables
    [SerializeField] float abilityBarPercentage;
    [SerializeField] int abilityPhase;
    public bool riseOrFall;
    public float abilityRetainThreshold;
    public float timeRetainThreshold;
    private float stagnateTimer;
    [SerializeField] float previousBarValue;
    public float barDecrease;
    public bool activateAbiliityBar;
    [SerializeField] bool canDecrease;
    float lastChangeTime;
    [SerializeField] bool buffed1;
    [SerializeField] bool buffed2;
    [SerializeField] bool buffed3;
    [SerializeField] bool buffed4;
    //AbilityBar Feedback
    [SerializeField] Color flashColor = Color.white;  // Color to flash to
    [SerializeField] Color originalColor = Color.yellow;
    [SerializeField] float flashSpeed;  // Speed of the color flash
    [SerializeField] bool isFlashing;




    private void Start()
    {
        shakeScript = healthSlider.gameObject.GetComponent<ObjectShakeScript>();
        //playerSO.health = 100;
        playerSO.health = playerSO.maxhealth;
        currentHealth = playerSO.health; // Set initial health to full
        healthSlider.maxValue = currentHealth;
        thresholdHealth = playerSO.health;
        flashEffect = GetComponent<PlayerFlash>();
        UpdateHealthBar();

        healthState = HealthState.normal;
        currentState = healthState;

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerScript>();
        playerHandler = GetComponent<PlayerHandler>();
        ogValues = playerHandler.animationSpeed;
        ogAtkSpeed = playerHandler.attackAnimationSpeed;
        cutsceneManager = GameObject.FindGameObjectWithTag("VictoryScreen").GetComponent<CutSceneManager>();
        meshRenderer = GetComponent<SkeletonAnimation>();
        vfxManager = GetComponent<PlayerVFXManager>();

        lastChangeTime = Time.time;
        previousBarValue = playerHandler.currentUltimateCharge;

        abilityFill.color = originalColor;
        isFlashing = false;
    }

    void CheckHealthState()
    {
        if(healthState != currentState)
        {
            if(healthState == HealthState.berserk)
            {
                if (!isTriggered)
                {
                    playerHandler.DisableMovement(1);
                    isTriggered = true;
                }

                meshRenderer.CustomMaterialOverride[originalMat] = rageMat;
                playerHandler.animationSpeed = 2f;
                playerHandler.attackAnimationSpeed = 2f;
                playerHandler.aoeDmg = 20;
                playerHandler.AlterStats(true, 3, 1.5f);
                currentState = healthState;
            }

            if(healthState == HealthState.normal)
            {
                meshRenderer.CustomMaterialOverride[originalMat] = originalMat;
                playerHandler.attackAnimationSpeed = ogAtkSpeed;
                playerHandler.animationSpeed = ogValues;
                playerHandler.aoeDmg = 10;
                playerHandler.AlterStats(false, 3, 1.5f);
                currentState = healthState;
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

        CheckHealthState();
        UpdateAbilityBar();
    }

    //void CheckHealthStatus(float playerhealth)
    //{
    //    int healthPercentage = (int)(100 - ((100f / playerSO.maxhealth) * currentHealth));

    //    if (healthPercentage <= 45)
    //    {
    //        if(healthState == HealthState.berserk)
    //        {
    //            healthState = HealthState.normal;
    //        }
    //    }

    //    else
    //    {
    //        if(healthState != HealthState.berserk)
    //        {
    //            healthState = HealthState.berserk;
    //        }
    //        else
    //        {
    //            return;
    //        }
    //    }
    //}

    public void TakeDamage(int damage)
    {
        if (Time.time - lastPainRoarTime >= painRoarCooldown)
        {
            playerHandler.PainRoar(); // Play the pain roar
            lastPainRoarTime = Time.time; // Update the last pain roar time
        }
        //shakeScript.StartShake();
        if (playerSO.health >= 1)
        {
            if (healthState == HealthState.normal)
            {
                playerSO.health -= damage; // Adjust the damage amount as needed

                if(playerSO.health < 1)
                {
                    TriggerDeath();
                }
            }

            else
            {
                playerSO.health -= damage * 2; // Player takes double damage when they are in berserk mode
                
                if (playerSO.health < 1)
                {
                    TriggerDeath();
                }
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
            Invoke("TriggerDeath", 3f);
        }
    }

    void TriggerDeath()
    {
        cutsceneManager.TriggerEnd();
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
        float tempPercentage = (playerHandler.currentUltimateCharge / playerSO.maxUltimateCharge) * 100f;
        abilityBarPercentage = tempPercentage;
        float prevPercentage = tempPercentage;

        abilitySlider.value = abilityBarPercentage; // Update the slider's value
        abilityFill.fillAmount = abilityBarPercentage; // Update the fill amount of the health bar
        AlterPlayer();
        if (activateAbiliityBar)
        {
            if (HasValueChanged())
            {
                // Value has changed, update lastChangeTime
                previousBarValue = playerHandler.currentUltimateCharge;
                lastChangeTime = Time.time;
                riseOrFall = true;
                canDecrease = false;
            }
            else
            {
                // Value hasn't changed for a certain amount of time
                if (Time.time - lastChangeTime > timeRetainThreshold)
                {
                    DecreaseBarValue();
                    riseOrFall = false;
                    canDecrease = true;
                }
            }

            if (abilityBarPercentage >= 75)
            {
                if (!isFlashing)
                {
                    StartCoroutine(FlashColor(flashColor, originalColor));
                }
            }

            else
            {
                if (isFlashing)
                {
                    StopCoroutine("FlashColor");
                    abilityFill.color = originalColor;
                    isFlashing = false;
                }
            }

            //Use ultimate
            if (abilityBarPercentage > 95)
            {
                if (playerHandler.currentUltimateCharge == playerHandler.playerData.maxUltimateCharge)
                {
                    playerHandler.DisableMovement(0);
                }
            }
        }
    }

    void AlterPlayer()
    {
        switch (riseOrFall)
        {
            case true:
                BuffPlayer();
                break;

            case false:
                DebuffPlayer();
                break;
        }
    }

    private IEnumerator FlashColor(Color fromColor, Color toColor)
    {
        isFlashing = true;

        while (isFlashing == true)
        {
            abilityFill.color = Color.Lerp(fromColor, toColor, Mathf.PingPong(Time.time * flashSpeed, 1f));

            yield return null;
        }
    }

    bool HasValueChanged()
    {
        // Replace this condition with your actual condition to check for value change
        return Mathf.Abs(playerHandler.currentUltimateCharge - previousBarValue) > abilityRetainThreshold;
    }

    void DecreaseBarValue()
    {
        if (playerHandler.currentUltimateCharge > 0)
        {
            //Start to decrease the ultimate bar
            playerHandler.currentUltimateCharge -= barDecrease * Time.deltaTime;
            previousBarValue -= barDecrease * Time.deltaTime;
        }

        else
        {
            playerHandler.currentUltimateCharge = 0f;
            previousBarValue = 0f;
        }
    }

    private void UpdateHealthBar()
    {
        healthSlider.value = currentHealth; // Update the slider's value
        healthFill.fillAmount = currentHealth; // Update the fill amount of the health bar
    }

    void BuffPlayer()
    {
        if(abilityBarPercentage < 15)
        {
            abilityPhase = 0;
        }

        else
        {
            //Update Abilites accordingly
            if (abilityBarPercentage > 15 && abilityBarPercentage < 24)
            {
                if (!buffed1)
                {
                    abilityPhase = 1;
                    TriggerAbilities(1);
                    vfxManager.SpawnUpgradeVFX();
                    buffed1 = true;
                }
            }

            //Update Abilites accordingly
            if (abilityBarPercentage > 25 && abilityBarPercentage < 34)
            {
                if (!buffed2)
                {
                    abilityPhase = 2;
                    TriggerAbilities(2);
                    vfxManager.SpawnUpgradeVFX();
                    buffed2 = true;
                }
            }

            if (abilityBarPercentage > 35 && abilityBarPercentage < 44)
            {
                if (!buffed3)
                {
                    abilityPhase = 3;
                    TriggerAbilities(3);
                    vfxManager.SpawnUpgradeVFX();
                    buffed3 = true;
                }
            }

            if (abilityBarPercentage > 45 && abilityBarPercentage < 95)
            {
                if (!buffed4)
                {
                    abilityPhase = 4;
                    TriggerAbilities(4);
                    vfxManager.SpawnUpgradeVFX();
                    buffed4 = true;
                }
            }
        }
    }

    void DebuffPlayer()
    {
        //Update Abilites accordingly
        if (abilityBarPercentage >= 46 && abilityBarPercentage <= 95)
        {
            if (buffed4)
            {
                RemoveAbilities(4);
                buffed4 = false;
                abilityPhase = 4;
            }
        }
        
        //Update Abilites accordingly
        if (abilityBarPercentage >= 36 && abilityBarPercentage <= 45)
        {
            if (buffed3)
            {
                RemoveAbilities(3);
                buffed3 = false;
                abilityPhase = 2;
            }
        }

        //Update Abilites accordingly
        if (abilityBarPercentage >= 26 && abilityBarPercentage <= 35)
        {
            if (buffed2)
            {
                RemoveAbilities(2);
                buffed2 = false;
                abilityPhase = 1;
            }
        }

        if (abilityBarPercentage < 25)
        {
            if (buffed1)
            {
                RemoveAbilities(1);
                buffed1 = false;
                abilityPhase = 0;
            }
        }
    }

    void TriggerAbilities(int ability)
    {
        VibrateHaptics.VibrateTick();
        switch (ability)
        {
            case 1:
                playerHandler.AlterStats(true, 2, 1.6f);
                break;

            case 2:
                playerHandler.AlterStats(true, 2, 1.8f);
                break;

            case 3:
                playerHandler.AlterStats(true, 2, 2.0f);
                break;

            case 4:
                playerHandler.AlterStats(true, 1, 5f);
                break;
        }
    }

    void RemoveAbilities(int ability)
    {
        VibrateHaptics.VibrateTick();
        switch (ability)
        {
            case 1:
                playerHandler.AlterStats(false, 2, 1.4f);
                break;

            case 2:
                playerHandler.AlterStats(false, 2, 1.6f);
                break;

            case 3:
                playerHandler.AlterStats(false, 2, 1.8f);
                break;

            case 4:
                playerHandler.AlterStats(false, 1, 5f);
                break;

            case 5:
                playerHandler.AlterStats(false, 5, 0);
                break;
        }
    }
}
