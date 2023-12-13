using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using TMPro;

public class PlayerHandler : MonoBehaviour, ISoundable
{
    public enum PlayerStates
    {
        idle,
        attack,
        move,
        victory,
        defeat,
        rage,
        ultimate,
        land,
        exhaust,
        damage,
    }

    //Variable for State
    public SkeletonAnimation skeletonAnim;
    public AnimationReferenceAsset idling, idling2, moving, moving2, moving3, moving4, attacking, attacking2, attacking3, attacking4, attacking5, attacking6, attacking7, ultimating, ultimating2, victorying, defeating, raging, landing, exhausting, damaging;
    [SerializeField] private PlayerStates currentState;
    [SerializeField] private PlayerStates prevState;
    public string currentAnimation;

    //Variable for stat
    PlayerHealthScript playerHealth;
    [SerializeField] float damageHolder;
    [SerializeField] float movementSpeedHolder;
    [SerializeField] float attackRangeHolder;
    [SerializeField] float maxUltChargeHolder;
    public float stepDamageHolder;

    //Variable for player input
    public PlayerStatScriptableObject playerData;
    public Joystick joystick;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerVFXManager vfxManager;
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] Vector2 movementInput;
    [SerializeField] Vector2 lastKnownVector;
    public LayerMask enemyLayer;
    [SerializeField] private Collider2D selectedEnemy;
    public bool canMove;
    public bool canAttack;
    public bool enableInput;
    [SerializeField] float attackHitRange;
    [SerializeField] private bool isAttacking;
    [SerializeField] private float degreeAngle;
    [SerializeField] int moveSector;
    [SerializeField] int prevMoveSector;
    [SerializeField] int attackSector;

    //Variables for attacking 
    public List<Collider2D> listOfEnemies = new List<Collider2D>();

    public GameObject HitDetection;
    public GameObject Groundcrack;
    public List<UltimateBase> utlimates = new List<UltimateBase>();
    [SerializeField] private int selectedUltimateHolder;
    public bool canEarnUlt = true;
    public float currentUltimateCharge;
    public float ultimateRadius = 20f;
    public float aoeDmg = 10f;
    public bool isDashing;
    public bool triggerHold;
    [SerializeField] HitCircle hitCircle;
    [SerializeField] TextMeshProUGUI chargeCountdown;
    [SerializeField] float countdown;

    private bool isOnSpawned;
    private bool isOffSpawned;

    public float maxFrictionValue;
    public float lerpTime;

    public float animationSpeed;
    public float attackAnimationSpeed;

    private bool playFull;
    [SerializeField] private bool isUltimate;
    [SerializeField] private bool isRaging;
    [SerializeField] private bool extendedIdle;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isIdle;
    [SerializeField] private bool isTriggered;
    public bool isLanding;
    [SerializeField] private bool isExhausting;
    [SerializeField] private bool isDamaging;

    public bool isEnd;

    public float impactTimer;
    public float currentImpactTimer;
    public float idleTimer;
    public Collider2D[] entitycollider;

    [SerializeField] private float idleRoarTimer = 0f;
    private float minRoarThreshold = 10f;
    private float maxRoarThreshold = 20f;
    private float varTime;

    //FootSteps
    public AudioSource footstepAudioSource;
    public AudioSource attackAudioSource;
    public AudioSource jumpAudioSource;
    public AudioSource roarAudioSource;
    public AudioClip[] foostepsSFX;
    public AudioClip[] attackSFX;
    public AudioClip monsterRoarSFX;
    public AudioClip[] painRoarSFX;

    // Start is called before the first frame update
    void Start()
    {
        currentState = PlayerStates.idle;
        SetCharacterState(currentState);
        skeletonAnim = GetComponent<SkeletonAnimation>();
        skeletonAnim.AnimationState.Event += OnSpineEvent;
        entitycollider = GetComponentsInChildren<Collider2D>();

        vfxManager = GetComponent<PlayerVFXManager>();
        rb = GetComponent<Rigidbody2D>();
        footstepAudioSource = GetComponent<AudioSource>();
        cameraShake = FindObjectOfType<CameraShake>();
        playerHealth = GetComponent<PlayerHealthScript>();
        hitCircle = GameObject.Find("HitCircle").GetComponent<HitCircle>();
        chargeCountdown = GameObject.Find("ChargeDashCountdown").GetComponent<TextMeshProUGUI>();
        chargeCountdown.gameObject.SetActive(false);
        AssignStats();
        DisableColliders();

        varTime = Random.Range(minRoarThreshold, maxRoarThreshold);
    }

    // Update is called once per frame
    void Update()
    {

        Cheats();
        IdleRoar();
        if (enableInput)
        {
            if (!isEnd)
            {
                CheckJoyStickInput();

                if (!triggerHold)
                {
                    if (isDashing)
                    {
                        Dash();
                    }

                    else
                    {
                        MoveAndAttack();
                        PlayerIdle();
                        PlayerMove();
                        PlayerAttack();
                    }
                }

                else
                {
                    return;
                }
            }
            //else
            //{
            //    rb.velocity = Vector2.zero;
            //    return;
            //}
        }

        else
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (playerData.health == 0)
        {
            vfxManager.SpawnDeathVFX();
        }

        if (selectedEnemy == null)
        {
            attackSector = 0;
        }

    }

    void AssignStats()
    {
        damageHolder = playerData.attackDamage;
        movementSpeedHolder = playerData.speed;
        attackRangeHolder = playerData.attackRange;
        stepDamageHolder = playerData.stepDamage;
        selectedUltimateHolder = playerData.setUltimate;
        maxUltChargeHolder = playerData.maxUltimateCharge;

        switch (selectedUltimateHolder)
        {
            case 0:
                GetComponent<CrabUltimateU>().enabled = false;
                break;

            case 1:
                GetComponent<CrabUltimateD>().enabled = false;
                break;
        }
    }

    public void AlterStats(bool isBuff, int statID, float statChange)
    {
        //Increase stat here
        if (isBuff)
        {
            //Which stat to increase 
            switch (statID)
            {
                //Health
                case 0:
                    playerHealth.playerSO.maxhealth += (int)statChange;
                    break;

                //Attack Damage
                case 1:
                    damageHolder += statChange;
                    break;

                //Attack Speed
                case 2:
                    attackAnimationSpeed += statChange;
                    break;

                //Movement Speed
                case 3:
                    movementSpeedHolder += statChange;
                    animationSpeed += statChange;
                    break;

                case 4:
                    stepDamageHolder += statChange;
                    break;
            }
        }

        //Decrease stat
        else
        {
            //Which stat to decrease 
            switch (statID)
            {
                //Health
                case 0:
                    playerHealth.playerSO.maxhealth -= (int)statChange;
                    break;

                //Attack Damage
                case 1:
                    damageHolder -= statChange;
                    break;

                //Attack Speed
                case 2:
                    attackAnimationSpeed -= statChange;
                    break;

                //Movement Speed
                case 3:
                    movementSpeedHolder -= statChange;
                    break;

                case 4:
                    stepDamageHolder -= statChange;
                    break;
            }

        }
    }

    void MoveAndAttack()
    {
        if (attackSector == moveSector)
        {
            canMove = false;
        }

        else
        {
            canMove = true;
            selectedEnemy = null;
        }
    }

    //To allow other scripts to reset the player's state to idle or moving based on the input
    public void IdleOrMove()
    {
        enableInput = true;

        //If there is movement input
        if (movementInput != Vector2.zero)
        {
            SetCharacterState(PlayerStates.move);
        }

        else
        {
            movementInput = Vector2.zero;
            SetCharacterState(PlayerStates.idle);
        }
    }

    public void EnableColliders()
    {
        foreach (Collider2D collider in entitycollider)
        {
            collider.enabled = true;
        }
    }
    public void DisableColliders()
    {
        foreach (Collider2D collider in entitycollider)
        {
            collider.enabled = false;
        }
    }

    void Dash()
    {
        if (!currentState.Equals(PlayerStates.attack))
        {
            SetCharacterState(PlayerStates.move);
        }

        if (movementInput.x != 0 && movementInput.y != 0)
        {
            lastKnownVector = movementInput;
        }

        if (movementInput == Vector2.zero)
        {
            if (lastKnownVector.x == 0 && lastKnownVector.y == 0)
            {
                SetCharacterState(PlayerStates.move);
                moveSector = 2;
                rb.velocity = new Vector2(0.99f * movementSpeedHolder, -0.99f * movementSpeedHolder);
            }
            else
            {
                rb.velocity = new Vector2(lastKnownVector.x * movementSpeedHolder, lastKnownVector.y * movementSpeedHolder);
            }
        }

        else
        {
            rb.velocity = Vector3.RotateTowards(rb.velocity, movementInput * movementSpeedHolder, Mathf.Deg2Rad * 60f * Time.deltaTime, float.MaxValue);
        }
    }

    public static bool AreVectorsWithinRange(Vector3 vector1, Vector3 vector2, float maxDistance)
    {
        float distance = Vector3.Distance(vector1, vector2);
        return distance <= maxDistance;
    }

    private void PlayerMove()
    {
        if (movementInput != Vector2.zero)
        {
            isMoving = true;
            //cameraShake.ShakeCamera();

            if (!currentState.Equals(PlayerStates.attack))
            {
                SetCharacterState(PlayerStates.move);
            }

            if (movementInput.x != 0 && movementInput.y != 0)
            {
                lastKnownVector = movementInput;
            }
        }

        else
        {
            isMoving = false;
            cameraShake.StopShaking();

            if (!currentState.Equals(PlayerStates.attack))
            {
                if (!isDashing)
                {
                    rb.velocity = Vector2.zero;
                    SetCharacterState(PlayerStates.idle);
                }

                else
                {
                    SetCharacterState(PlayerStates.move);
                }
            }
        }

        if (canMove)
        {
            //Code to move the player
            rb.velocity = new Vector2(movementInput.x * movementSpeedHolder, movementInput.y * movementSpeedHolder);
            skeletonAnim.timeScale = animationSpeed;
        }
    }

    public void PlayerIdle()
    {
        if (currentState == PlayerStates.idle)
        {
            skeletonAnim.timeScale = 1f;
            isIdle = true;

            if (idleTimer < 4f)
            {
                idleTimer += Time.deltaTime;
            }

            if (idleTimer >= 4f)
            {
                extendedIdle = true;
                SetCharacterState(PlayerStates.idle);
            }
        }

        else
        {
            isIdle = false;
        }
    }

    public void MuteRoar()
    {
        roarAudioSource.enabled = false;
        footstepAudioSource.enabled = false;

    }

    public void TurnOffPlayer()
    {
        
    }
    //Function to trigger any spine events
    void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        string eventName = e.Data.Name;

        //Function triggers when the claw lands 
        // eventName == "Name of animation function in timeline
        if (eventName == "damage")
        {
            //Call the function that you want here
            TriggerDamage();
            PlaySFX();
        }

        if (eventName == "right01")
        {
            vfxManager.footImpact(0);
            PlaySFX();
        }
        if (eventName == "right02")
        {
            vfxManager.footImpact(1);
            PlaySFX();
        }
        if (eventName == "left01")
        {
            vfxManager.footImpact(2);
            PlaySFX();
        }
        if (eventName == "left02")
        {
            vfxManager.footImpact(3);
            PlaySFX();
        }

        if (eventName == "jump")
        {
            vfxManager.TriggerAoeTremor();
            JumpSFX();
        }

        if (eventName == "land")
        {
            PlaySFX();
            vfxManager.TriggerAoeTremor();
            TriggerUltimate1();
        }

        if (eventName == "Smash")
        {
            vfxManager.TriggerAoeTremor();
            //TriggerAOE();
            vfxManager.SpawnAoeVFX();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("BigBuilding"))
        {
            if (!listOfEnemies.Contains(collision))
            {
                listOfEnemies.Add(collision);
            }

            else
            {
                return;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("BigBuilding"))
        {
            listOfEnemies.Remove(collision);
            attackSector = 0;
        }
    }

    private void PlayerAttack()
    {
        if (canAttack && movementInput != Vector2.zero)
        { //Using the RAYCAST to detect and hit enemies
            if (listOfEnemies.Count == 0)
            {
                skeletonAnim.timeScale = animationSpeed;
                RaycastHit2D hit = Physics2D.Raycast(HitDetection.transform.position, lastKnownVector, attackHitRange, enemyLayer);
                // Check if the raycast hits an object
                if (hit.collider != null)
                {
                    selectedEnemy = hit.collider;
                    if (!currentState.Equals(PlayerStates.attack))
                    {
                        prevState = currentState;
                    }
                    SetCharacterState(PlayerStates.attack);
                    if (!isAttacking)
                    {
                        isAttacking = true;
                    }
                }
                else
                {
                    if (!listOfEnemies.Contains(selectedEnemy))
                    {
                        selectedEnemy = null;
                        if (isAttacking)
                        {
                            SetCharacterState(prevState);
                            isAttacking = false;
                        }
                    }

                    else
                    {
                        return;
                    }
                }
            }

            //Using the CYLINDER COLLIDER to detect and hit enemies
            else
            {
                // Find the closest enemy
                float closestDistance = float.MaxValue;
                Collider2D closestCollider = null;

                foreach (var enemy in listOfEnemies)
                {
                    Debug.Log(enemy);
                    float distance = Vector2.Distance(transform.position, enemy.transform.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestCollider = enemy;
                        Debug.Log(closestCollider);
                    }
                }

                // Perform the attack on the closest enemy
                if (closestCollider != null)
                {
                    selectedEnemy = closestCollider;
                    if (!currentState.Equals(PlayerStates.attack))
                    {
                        prevState = currentState;
                    }
                    SetCharacterState(PlayerStates.attack);
                    if (!isAttacking)
                    {
                        isAttacking = true;
                    }
                }
            }
        }

        else
        {
            SetCharacterState(PlayerStates.idle);
        }

        //else
        //{
        //    if (!isDashing)
        //    {
        //        SetCharacterState(PlayerStates.idle);
        //    }

        //    else
        //    {
        //        SetCharacterState(prevState);
        //    }
        //}
    }

    //All entities MUST call this script to disable the player from attacking if it detects them with the collider instead of the raycast
    public void DisableAttack(Collider2D collider)
    {
        attackSector = 0;
        listOfEnemies.Remove(collider);
        selectedEnemy = null;
        SetCharacterState(prevState);
    }

    void TriggerAttackDirAnimation()
    {
        if (selectedEnemy != null)
        {
            float playerX = this.transform.position.x;
            float objectX = selectedEnemy.gameObject.transform.position.x;
            Vector3 dir = selectedEnemy.transform.position - HitDetection.transform.position;
            float angle = Vector3.Angle(dir, Vector3.down);
            if (objectX > playerX)
            {
                if (angle >= 135f)
                {
                    attackSector = 1;
                    SetAnimation(0, attacking3, true, attackAnimationSpeed);
                }
                if (angle >= 45f && angle < 135f)
                {
                    attackSector = 2;
                    SetAnimation(0, attacking2, true, attackAnimationSpeed);
                }
                if (angle >= 0f && angle < 45f)
                {
                    attackSector = 3;
                    SetAnimation(0, attacking, true, attackAnimationSpeed);
                }
            }
            else if (objectX < playerX)
            {
                if (angle >= 135f)
                {
                    attackSector = 1;
                    SetAnimation(0, attacking6, true, attackAnimationSpeed);
                }
                if (angle >= 45f && angle < 135f)
                {
                    attackSector = 4;
                    SetAnimation(0, attacking5, true, attackAnimationSpeed);
                }
                if (angle >= 0f && angle < 45f)
                {
                    attackSector = 3;
                    SetAnimation(0, attacking4, true, attackAnimationSpeed);
                }
            }
        }

        else
        {
            SetCharacterState(prevState);
        }

    }

    //In the animation, this will deal damage to the select unit
    public void TriggerDamage()
    {
        if (selectedEnemy != null)
        {
            skeletonAnim.timeScale = animationSpeed;
            selectedEnemy.GetComponent<Targetable>().TakeDamage(damageHolder);
            //attackCount += 1;
        }

        else { return; }
    }
    public void ChargeUltimate(int amount)
    {
        if (canEarnUlt)
        {
            if (currentUltimateCharge != playerData.maxUltimateCharge)
            {
                currentUltimateCharge += amount;
            }

            if (currentUltimateCharge >= playerData.maxUltimateCharge)
            {
                currentUltimateCharge = playerData.maxUltimateCharge;
            }
        }

        else
        {
            return;
        }
    }

    void UseUltimate(int whichUlt)
    {
        canEarnUlt = false;
        switch (whichUlt)
        {
            case 0:
                if (utlimates[0] != null)
                {
                    joystick.gameObject.SetActive(false);
                    DecreaseUltimateBar(100f);
                    SetAnimation(0, ultimating, false, 1f);
                }
                break;

            case 1:
                if (utlimates[1] != null)
                {
                    SetCharacterState(PlayerStates.rage);
                    triggerHold = true;
                    if (!isOnSpawned)
                    {
                        vfxManager.SpawnRageOnText();
                        isOnSpawned = true;
                        isOffSpawned = false;
                    }

                    isDashing = true;
                    canAttack = false;
                    vfxManager.StartAppearing();
                    playerHealth.healthState = PlayerHealthScript.HealthState.berserk;
                    hitCircle.triggerHoldingDown = true;
                    StartCoroutine(HoldControlForDash());
                    playerHealth.activateAbiliityBar = false;
                }
                break;
        }
    }

    public void TriggerUltimate1()
    {
        utlimates[0].UseDamageUltimate(ultimateRadius, playerData.ultimateDamage);
        Vector2 crackPos = new Vector2(transform.position.x, transform.position.y - 1f);
        Instantiate(Groundcrack, transform.position, Quaternion.identity);
    }

    void TriggerUltimate2()
    {
        vfxManager.StartFading();
        countdown = 5f;
        canAttack = false;
        canEarnUlt = false;
        enableInput = true;
        vfxManager.isDashing = true;
        utlimates[1].UseUtilityUltimate();
        vfxManager.dashBodyVFX.SetActive(true);
    }

    IEnumerator HoldControlForDash()
    {
        Debug.Log("Trigger hold down to charge");
        while (countdown > 0)
        {
            yield return null;
            if (!chargeCountdown.gameObject.activeSelf)
            {
                chargeCountdown.gameObject.SetActive(true);
            }

            countdown -= Time.deltaTime;
            float holdDashTimer = Mathf.RoundToInt(countdown);
            chargeCountdown.text = "Get Ready to Charge in: " + holdDashTimer.ToString();

            float moveX = joystick.Horizontal;
            float moveY = joystick.Vertical;

            movementInput = new Vector2(moveX, moveY).normalized;
            lastKnownVector = movementInput;
        }

        hitCircle.triggerHoldingDown = false;
        triggerHold = false;
        TriggerUltimate2();
        chargeCountdown.gameObject.SetActive(false);
    }

    public void DecreaseUltimateBar(float decreaseRate)
    {
        Debug.Log("Bar is decreasingh");
        playerHealth.activateAbiliityBar = false;
        // Rapidly decrease the ultimate bar during the ultimate animation
        currentUltimateCharge -= Time.deltaTime * decreaseRate; // Adjust the multiplier as needed
    }
   public void DisableUltimateVFX()
    {
        vfxManager.dashBodyVFX.SetActive(false);
    }

    //Trigger ultimate, rage, victory and defeat state here
    public void DisableMovement(int state)
    {
        enableInput = false;
        switch (state)
        {
            case 0:
                SetCharacterState(PlayerStates.ultimate);
                if (!currentState.Equals(PlayerStates.ultimate))
                {
                    prevState = currentState;
                }
                SetCharacterState(PlayerStates.ultimate);
                if (!isUltimate)
                {
                    isUltimate = true;
                }
                break;

            case 1:
                SetCharacterState(PlayerStates.rage);
                if (!currentState.Equals(PlayerStates.rage))
                {
                    prevState = currentState;
                }
                SetCharacterState(PlayerStates.rage);
                if (!isRaging)
                {
                    isRaging = true;
                }
                break;

            case 2:
                SetCharacterState(PlayerStates.victory);
                isDashing = false;
                MuteRoar();
                DisableColliders();
                animationSpeed = 1.7f;
                Debug.Log("Player won");
                break;

            case 3:
                SetCharacterState(PlayerStates.defeat);
                isDashing = false;
                vfxManager.SpawnDeathVFX();
                MuteRoar();
                DisableColliders();
                 animationSpeed = 1.7f;
                Debug.Log("Player lost");
                break;

            case 4:
                SetCharacterState(PlayerStates.exhaust);

                if (!isOffSpawned)
                {
                    vfxManager.SpawnRageOffText();
                    isOffSpawned = true;
                    isOnSpawned = false;
                }

                if (!currentState.Equals(PlayerStates.exhaust))
                {
                    prevState = currentState;
                }
                SetCharacterState(PlayerStates.exhaust);
                if (!isExhausting)
                {
                    isExhausting = true;
                }
                break;

            case 5:
                SetCharacterState(PlayerStates.land);
                if (!currentState.Equals(PlayerStates.land))
                {
                    prevState = currentState;
                }
                SetCharacterState(PlayerStates.land);
                if (!isLanding)
                {
                    isLanding = true;
                }
                break;

            case 6:
                SetCharacterState(PlayerStates.damage);
                if (!currentState.Equals(PlayerStates.damage))
                {
                    prevState = currentState;
                }
                SetCharacterState(PlayerStates.damage);
                if (!isDamaging)
                {
                    isDamaging = true;
                }
                break;
        }
    }

    public void RevertState()
    {
        SetCharacterState(prevState);
    }

    public void SetAnimation(int track, AnimationReferenceAsset animation, bool loop, float timeScale)
    {
        if (animation.name.Equals(currentAnimation))
        {
            return;
        }

        if (playFull)
        {
            Spine.TrackEntry animationEntry = skeletonAnim.state.AddAnimation(track, animation, loop, 0f);
            animationEntry.TimeScale = timeScale;
            animationEntry.Complete += AnimationEntry_Complete;
            currentAnimation = animation.name;
        }
        else
        {
            Spine.TrackEntry animationEntry = skeletonAnim.state.SetAnimation(track, animation, loop);
            animationEntry.TimeScale = timeScale;
            animationEntry.Complete += AnimationEntry_Complete;
            currentAnimation = animation.name;
        }
    }

    //Triggers after the animation has played
    private void AnimationEntry_Complete(Spine.TrackEntry trackEntry)
    {
        if (playFull)
        {
            playFull = false;
        }

        if (isLanding)
        {
            isLanding = false;
            if (!joystick.gameObject.activeSelf)
            {
                joystick.gameObject.SetActive(true);
            }
        }

        if (isDamaging)
        {
            isDamaging = false;
            enableInput = true;
        }

        if (isExhausting)
        {
            isExhausting = false;
            if (!joystick.gameObject.activeSelf)
            {
                joystick.gameObject.SetActive(true);
                canAttack = true;
            }

            else { return; }
        }

        if (isAttacking)
        {
            attackSector = 0;
            isAttacking = false;
        }

        if (isUltimate)
        {
            isUltimate = false;
            if (!enableInput)
            {
                enableInput = true;
            }

            else { return; }

            if (!joystick.gameObject.activeSelf)
            {
                joystick.gameObject.SetActive(true);
            }

            else
            {
                return;
            }

            if (!canEarnUlt)
            {
                canEarnUlt = true;
            }
            else { return; }
        }

        if (isRaging)
        {
            isRaging = false;

            if (!enableInput)
            {
                enableInput = true;
            }

            else { return; }
        }

        if (extendedIdle)
        {
            extendedIdle = false;
            idleTimer = 0f;
        }

        else
        {
            return;
        }

        if (currentState != PlayerStates.victory || currentState != PlayerStates.defeat)
        {
            SetCharacterState(prevState);
        }

        else
        {
            return;
        }
    }

    void CheckJoyStickInput()
    {
        float moveX = joystick.Horizontal;
        float moveY = joystick.Vertical;

        movementInput = new Vector2(moveX, moveY).normalized;

        float angleRadians = Mathf.Atan2(movementInput.y, movementInput.x);
        // Convert the angle from radians to degrees
        degreeAngle = angleRadians * Mathf.Rad2Deg;

        // Ensure the angle is positive (0 to 360 degrees)
        degreeAngle = (degreeAngle + 360) % 360;

        //Moving Upwards
        if (degreeAngle > 45 && degreeAngle < 135)
        {
            moveSector = 1;
        }

        //Moving Leftwards
        if (degreeAngle > 135 && degreeAngle < 225)
        {
            moveSector = 4;
        }

        //Moving Downwards
        if (degreeAngle > 225 && degreeAngle < 315)
        {
            moveSector = 3;
        }

        //Moving Rightward
        if (degreeAngle > 315 && degreeAngle < 360 || degreeAngle > 0 && degreeAngle < 45)
        {
            moveSector = 2;
        }
    }

    public void SetCharacterState(PlayerStates state)
    {
        switch (state)
        {
            case PlayerStates.idle:
                playFull = false;
                if (!extendedIdle)
                {
                    SetAnimation(0, idling, true, 1f);
                }
                else
                {
                    SetAnimation(0, idling2, true, 1f);
                }
                break;

            case PlayerStates.attack:
                playFull = false;
                TriggerAttackDirAnimation();
                break;

            case PlayerStates.move:
                playFull = false;
                if (moveSector == 1)
                {
                    attackHitRange = attackRangeHolder;
                    SetAnimation(0, moving, true, animationSpeed);
                }

                //Moving Leftwards, degreeAngle > 135 && degreeAngle < 225
                if (moveSector == 4)
                {
                    attackHitRange = attackRangeHolder + 1f;
                    SetAnimation(0, moving3, true, animationSpeed);
                }

                //Moving Downwards, degreeAngle > 225 && degreeAngle < 315
                if (moveSector == 3)
                {
                    attackHitRange = attackRangeHolder;
                    SetAnimation(0, moving2, true, animationSpeed);
                }

                //Moving Rightward, degreeAngle > 315 && degreeAngle < 360 || degreeAngle > 0 && degreeAngle < 45
                if (moveSector == 2)
                {
                    attackHitRange = attackRangeHolder + 1f;
                    SetAnimation(0, moving4, true, animationSpeed);
                }
                break;

            case PlayerStates.victory:
                SetAnimation(1, victorying, true, 0.5f);
                playFull = true;
                break;

            case PlayerStates.defeat:
                SetAnimation(1, defeating, false, 0.5f);
                playFull = true;
                break;

            case PlayerStates.ultimate:
                //Trigger the different ultimates here
                UseUltimate(selectedUltimateHolder);
                break;

            case PlayerStates.land:
                SetAnimation(0, landing, false, 1f);
                playFull = false;
                break;

            case PlayerStates.rage:
                SetAnimation(0, raging, true, 1f);
                playFull = false;
                break;

            case PlayerStates.exhaust:
                SetAnimation(0, exhausting, false, 1f);
                playFull = true;
                break;

            case PlayerStates.damage:
                SetAnimation(0, damaging, false, 1f);
                playFull = true;
                break;
        }

        currentState = state;
    }

    private void Cheats()
    {
        if (Input.GetKeyUp(KeyCode.V))
        {
            CutSceneManager csManager = GameObject.FindGameObjectWithTag("VictoryScreen").GetComponent<CutSceneManager>();
            GameManagerScript gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerScript>();
            gameManager.isVictory = true;
            csManager.TriggerEnd();

        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            CutSceneManager csManager = GameObject.FindGameObjectWithTag("VictoryScreen").GetComponent<CutSceneManager>();
            GameManagerScript gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerScript>();
            gameManager.isVictory = false;
            csManager.TriggerEnd();
        }

        if (Input.GetKeyUp(KeyCode.Space)) // Triggers event randomly
        {
            EventManager eventManager = GameObject.FindGameObjectWithTag("EventManager").GetComponent<EventManager>();
            ClockSystem clockSystem = GameObject.FindGameObjectWithTag("Timer").GetComponent<ClockSystem>();
            eventManager.timer = clockSystem.eventInterval;

        }

        if (Input.GetKeyUp(KeyCode.K))
        {
            PlayerHealthScript playerhealth = GetComponent<PlayerHealthScript>();
            playerhealth.TakeDamage(100);
        }

        if (Input.GetKeyUp(KeyCode.U))
        {
            currentUltimateCharge = playerData.maxUltimateCharge;
        }
    }

    public void PlaySFX()
    {
        AudioClip SoundToPlay = foostepsSFX[Random.Range(0, foostepsSFX.Length)];
        AudioClip AttackToPlay = attackSFX[Random.Range(0, 1)];
        if (isMoving == true)
        {

            footstepAudioSource.PlayOneShot(SoundToPlay);
        }

        if (isAttacking == true)
        {

            attackAudioSource.pitch = UnityEngine.Random.Range(1f, 1.5f);
            attackAudioSource.PlayOneShot(AttackToPlay);
        }
        if (isUltimate == true)
        {
            AudioClip UltimateAttackToPlay = attackSFX[2];
            attackAudioSource.PlayOneShot(UltimateAttackToPlay);
        }

    }

    public void IdleRoar()
    {
        if (isIdle == true || isMoving == true)
        {
            idleRoarTimer += Time.deltaTime;

            if (idleRoarTimer > varTime)
            {
                roarAudioSource.PlayOneShot(monsterRoarSFX);
                varTime = Random.Range(minRoarThreshold, maxRoarThreshold);
                idleRoarTimer = 0f;
            }
        }
    }

    public void PainRoar()
    {
        AudioClip roarSoundToPlay = painRoarSFX[Random.Range(0, painRoarSFX.Length)];
        roarAudioSource.PlayOneShot(roarSoundToPlay);
    }

    public void JumpSFX()
    {
        jumpAudioSource.PlayOneShot(jumpAudioSource.clip);
    }


}

