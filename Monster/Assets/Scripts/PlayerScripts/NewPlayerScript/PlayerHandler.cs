using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

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
    }

    //Variable for State
    public SkeletonAnimation skeletonAnim;
    public AnimationReferenceAsset idling, idling2, moving, moving2, moving3, moving4, attacking, attacking2, attacking3, attacking4, attacking5, attacking6, attacking7, ultimating, victorying, defeating, raging;
    [SerializeField] private PlayerStates currentState;
    [SerializeField] private PlayerStates prevState;
    public string currentAnimation;

    //Variable for player input
    public PlayerStatScriptableObject playerData;
    public Joystick joystick;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerVFXManager vfxManager;
    [SerializeField] private CameraShake cameraShake;
    private Vector2 movementInput;
    private Vector2 lastKnownVector;
    public LayerMask enemyLayer;
    [SerializeField] private Collider2D selectedEnemy;
    public bool canMove;
    [SerializeField] float attackHitRange;
    [SerializeField] private bool isAttacking;
    [SerializeField] bool nearTarget;
    [SerializeField] private int attackCount;
    [SerializeField] private float degreeAngle;

    
    public GameObject HitDetection;
    public GameObject Groundcrack;
    public List<UltimateBase> utlimates = new List<UltimateBase>();
    public float currentUltimateCharge;
    public float ultimateRadius = 20f;
    public float aoeDmg = 10f;
    public float aoeTremor;
    public float animationSpeed;
    public float attackAnimationSpeed;

    [SerializeField] private bool isUltimate;
    [SerializeField] private bool isRaging;
    [SerializeField] private bool extendedIdle;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isIdle;
    [SerializeField] private bool isTriggered;

    public bool isEnd;

    public float impactTimer;
    public float currentImpactTimer;
    public float idleTimer;
    public Collider2D entitycollider;

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
        entitycollider = GetComponent<Collider2D>();

        vfxManager = GetComponent<PlayerVFXManager>();
        rb = GetComponent<Rigidbody2D>();
        footstepAudioSource = GetComponent<AudioSource>();
        cameraShake = FindObjectOfType<CameraShake>();
        entitycollider.enabled = false;

        varTime = Random.Range(minRoarThreshold, maxRoarThreshold);
    }
                
    // Update is called once per frame
    void Update()
    {
        Cheats();
        IdleRoar();
        if (canMove)
        {
            if (!isEnd)
            {
                PlayerIdle();
                PlayerMove();
                PlayerAttack();
            }
            else
            {
                rb.velocity = Vector2.zero;
                return;
            }
        }

        else
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if(playerData.health == 0)
        {
            vfxManager.SpawnDeathVFX();
        }
    }

    private void PlayerMove()
    {
        float moveX = joystick.Horizontal;
        float moveY = joystick.Vertical;

        movementInput = new Vector2(moveX, moveY).normalized;
        rb.velocity = new Vector2(movementInput.x * playerData.speed, movementInput.y * playerData.speed);
        skeletonAnim.timeScale = animationSpeed;

        if (movementInput != Vector2.zero)
        {
            isMoving = true;
            cameraShake.ShakeCamera();
            float angleRadians = Mathf.Atan2(movementInput.y, movementInput.x);

            // Convert the angle from radians to degrees
            degreeAngle = angleRadians * Mathf.Rad2Deg;

            // Ensure the angle is positive (0 to 360 degrees)
            degreeAngle = (degreeAngle + 360) % 360;


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
                rb.velocity = Vector2.zero;
                SetCharacterState(PlayerStates.idle);
            }
        }
    }

    public void PlayerIdle()
    {
        if(currentState == PlayerStates.idle)
        {
            skeletonAnim.timeScale = 1f;
            isIdle = true;
            
            if(idleTimer < 4f)
            {
                idleTimer += Time.deltaTime;
            }

            if(idleTimer >= 4f)
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

        if(eventName == "right01")
        {
            vfxManager.footImpact(0);
            TriggerTremor();
            PlaySFX();
        }
        if (eventName == "right02")
        {
            vfxManager.footImpact(1);
            TriggerTremor();
            PlaySFX();
        }
        if (eventName == "left01")
        {
            vfxManager.footImpact(2);
            TriggerTremor();
            PlaySFX();
        }
        if (eventName == "left02")
        {
            vfxManager.footImpact(3);
            TriggerTremor();
            PlaySFX();
        }

        if(eventName == "jump")
        {
            TriggerTremor();
            JumpSFX();
        }

        if(eventName == "land")
        {
            PlaySFX();
            TriggerTremor();
            UseUltimate1();
        }

        if(eventName == "Smash")
        {
            TriggerTremor();
            TriggerAOE();
            vfxManager.SpawnAoeVFX();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("BigBuilding"))
        {
            if(selectedEnemy != null)
            {
                return;
            }

            else
            {
                nearTarget = true;
                selectedEnemy = collision;
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("BigBuilding"))
        {
            nearTarget = true;
            selectedEnemy = null;
            SetCharacterState(prevState);
        }
    }

    private void AttackNearestEnemy()
    {

    }

    private void PlayerAttack()
    {
        if (!nearTarget)
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
                selectedEnemy = null;
                if (isAttacking)
                {
                    SetCharacterState(prevState);
                    isAttacking = false;
                }
            }
        }

        else
        {
            Debug.Log("There's a closer target " + selectedEnemy);
        }
    }

    //All entities MUST call this script to disable the player from attacking if it detects them with the collider instead of the raycast
    public void DisableAttack()
    {
        nearTarget = false;
        SetCharacterState(prevState);
    }

    void TriggerAttackDirAnimation()
    {
        float playerX = this.transform.position.x;
        float objectX = selectedEnemy.gameObject.transform.position.x;
        Vector3 dir = selectedEnemy.transform.position - HitDetection.transform.position;
        float angle = Vector3.Angle(dir, Vector3.down);
        if (objectX > playerX)
        {
            if (angle >= 135f)
            {
                SetAnimation(0, attacking3, true, attackAnimationSpeed);
            }
            if (angle >= 45f && angle < 135f)
            {
                SetAnimation(0, attacking2, true, attackAnimationSpeed);
            }
            if (angle >= 0f && angle < 45f)
            {
                SetAnimation(0, attacking, true, attackAnimationSpeed);
            }
        }
        else if (objectX < playerX)
        {
            if (angle >= 135f)
            {
                SetAnimation(0, attacking6, true, attackAnimationSpeed);
            }
            if (angle >= 45f && angle < 135f)
            {
                SetAnimation(0, attacking5, true, attackAnimationSpeed);
            }
            if (angle >= 0f && angle < 45f)
            {
                SetAnimation(0, attacking4, true, attackAnimationSpeed);
            }
        }

        //if (attackCount <= 5)
        //{
            
        //}

        //else
        //{
        //    SetAnimation(0, attacking7, false, 1f);
        //}
    }
    
    public void TriggerTremor()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, aoeTremor);

        foreach (Collider2D colldier in hitColliders)
        {
            if (colldier.CompareTag("Tree"))
            {
                ObjectShakeScript tree = colldier.GetComponent<ObjectShakeScript>();
                tree.StartShake();
            }
        }
    }

    public void TriggerAOE()
    {
        attackCount = 0;
        Vector2 ultiPos = new Vector2(transform.position.x, transform.position.y + 2f);
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(ultiPos, 10f);
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("BigBuilding"))
            {
                BigBuildingEnemy bigBuilding = collider.GetComponent<BigBuildingEnemy>();
                if (bigBuilding != null)
                {
                    bigBuilding.TakeDamage(aoeDmg);
                }
                else { return; }
            }

            else if (collider.CompareTag("Civilian"))
            {
                Civilian civilian = collider.GetComponent<Civilian>();
                if (civilian != null)
                {
                    civilian.enemyState = Civilian.EnemyState.death;
                }
                else { return; }
            }


            else if (collider.CompareTag("Tree"))
            {
                Trees tree = collider.GetComponent<Trees>();
                if (tree != null)
                {
                    tree.Death();
                }
                else { return; }
            }

            else if (collider.CompareTag("Car"))
            {
                CarAI car = collider.GetComponent<CarAI>();
                if (car != null)
                {
                    car.Death();
                }
                else { return; }
            }
        }
    }

    

    //In the animation, this will deal damage to the select unit
    public void TriggerDamage()
    {
        if (selectedEnemy != null)
        {
            skeletonAnim.timeScale = animationSpeed;
            selectedEnemy.GetComponent<Targetable>().TakeDamage();
            //attackCount += 1;
        }

        else { return; }
    }
    public void ChargeUltimate(int amount)
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

    public void UseUltimate1()
    {
        utlimates[0].UseDamageUltimate(ultimateRadius, playerData.ultimateDamage);
        Vector2 crackPos = new Vector2(transform.position.x, transform.position.y - 1f);
        Instantiate(Groundcrack, transform.position, Quaternion.identity);
        currentUltimateCharge = 0;
    }

    //Trigger ultimate, rage, victory and defeat state here
    public void DisableMovement(int state)
    {
        canMove = false;
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
                break;
            case 3:
                SetCharacterState(PlayerStates.defeat);
                vfxManager.SpawnDeathVFX();
                break;
        }
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public void SetAnimation(int track, AnimationReferenceAsset animation, bool loop, float timeScale)
    {
        if (animation.name.Equals(currentAnimation))
        {
            return;
        }
        Spine.TrackEntry animationEntry = skeletonAnim.state.SetAnimation(track, animation, loop);
        animationEntry.TimeScale = timeScale;
        animationEntry.Complete += AnimationEntry_Complete;
        currentAnimation = animation.name;
    }

    //Triggers after the animation has played
    private void AnimationEntry_Complete(Spine.TrackEntry trackEntry)
    {
        if (isAttacking || isUltimate || isRaging)
        {
            isAttacking = false;
            isUltimate = false;
            isRaging = false;

            if (!canMove)
            {
                canMove = true;
            }

            else { return;  }
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

        if(currentState != PlayerStates.victory || currentState != PlayerStates.defeat)
        {
            SetCharacterState(prevState);
        }

        else
        {
            return;
        }
    }

    public void SetCharacterState(PlayerStates state)
    {
        if (state.Equals(PlayerStates.idle))
        {
            if (!extendedIdle)
            {
                SetAnimation(0, idling, true, 1f);
            }
            else
            {
                SetAnimation(0, idling2, true, 1f);
            }
        }

        if (state.Equals(PlayerStates.move))
        {
            //Moving Upwards
            if(degreeAngle > 45 && degreeAngle < 135)
            {
                attackHitRange = playerData.attackRange;
                SetAnimation(0, moving, true, animationSpeed);
            }

            //Moving Leftwards
            if (degreeAngle > 135 && degreeAngle < 225)
            {
                attackHitRange = playerData.attackRange + 1f;
                SetAnimation(0, moving3, true, animationSpeed);
            }

            //Moving Downwards
            if (degreeAngle > 225 && degreeAngle < 315)
            {
                attackHitRange = playerData.attackRange;
                SetAnimation(0, moving2, true, animationSpeed);
            }

            //Moving Rightward
            if (degreeAngle > 315 && degreeAngle < 360 || degreeAngle > 0 && degreeAngle < 45)
            {
                attackHitRange = playerData.attackRange + 1f;
                SetAnimation(0, moving4, true, animationSpeed);
            }
        }

        if (state.Equals(PlayerStates.attack))
        {
            TriggerAttackDirAnimation();
        }

        if (state.Equals(PlayerStates.ultimate))
        {
            SetAnimation(0, ultimating, false, 1f);
        }

        if (state.Equals(PlayerStates.victory))
        {
            SetAnimation(0, victorying, true, 1f);
        }

        if (state.Equals(PlayerStates.defeat))
        {
            SetAnimation(0, defeating, false, 1f);
        }

        if (state.Equals(PlayerStates.rage))
        {
            SetAnimation(0, raging, false, 1f);
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
            eventManager.currentScore = eventManager.triggerThreshold;
           
        }

        if (Input.GetKeyUp(KeyCode.K))
        {
           PlayerHealthScript playerhealth = GetComponent<PlayerHealthScript>();
           playerhealth.TakeDamage(100);
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

