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
    public AnimationReferenceAsset idling, idling2, moving, moving2, attacking, attacking2, attacking3, attacking4, attacking5, attacking6, ultimating, victorying, defeating, raging;
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
    [SerializeField] private bool isAttacking;

    public GameObject[] legLocations;
    public GameObject HitDetection;
    public GameObject Groundcrack;
    public GameObject skillRdyText;
    public List<UltimateBase> utlimates = new List<UltimateBase>();
    public float currentUltimateCharge;
    public float ultimateRadius = 20f;

    [SerializeField] private bool isUltimate;
    [SerializeField] private bool isRaging;
    [SerializeField] private bool extendedIdle;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isIdle;

    public bool isEnd;

    public float impactTimer;
    public float currentImpactTimer;
    public float idleTimer;
    public Collider2D entitycollider;

    private float idleRoarTimer = 0f;
    private float minRoarInterval = 5f;
    private float maxRoarInterval = 10f;

    //FootSteps
    public AudioSource footstepAudioSource;
    public AudioSource attackAudioSource;
    public AudioSource jumpAudioSource;
    public AudioSource roarAudioSource;
    public AudioClip[] foostepsSFX;
    public AudioClip[] attackSFX;
    public AudioClip monsterRoarSFX;

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
    }
                
    // Update is called once per frame
    void Update()
    {
        CheckUltiRdy();
        Cheats();
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
        if (movementInput != Vector2.zero)
        {
            isMoving = true;
            cameraShake.ShakeCamera();

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

    public void SpawnFootprint()
    {
        vfxManager.SpawnImpactAtFoot(0);
        vfxManager.SpawnImpactAtFoot(1);
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

        if(eventName == "foot")
        {
            SpawnFootprint();
            PlaySFX();
        }

        if(eventName == "jump")
        {
            JumpSFX();
        }

        if(eventName == "land")
        {
            PlaySFX();
        }
    }

    public void CheckUltiRdy()
    {

        if (currentUltimateCharge == playerData.maxUltimateCharge)
        {
            skillRdyText.SetActive(true);
        }

        if (currentUltimateCharge == 0)
        {
            skillRdyText.SetActive(false);
        }

    }
    private void PlayerAttack()
    {
        RaycastHit2D hit = Physics2D.Raycast(HitDetection.transform.position, lastKnownVector, playerData.attackRange, enemyLayer);
        // Check if the raycast hits an object
        if (hit.collider != null)
        {
            //hit.collider.gameObject.GetComponent<Targetable>().TakeDamage();
            Debug.Log(hit);
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

    void TriggerAttackDirAnimation()
    {
        float playerX = this.transform.position.x;
        float objectX = selectedEnemy.gameObject.transform.position.x;
        Vector3 dir =  selectedEnemy.transform.position - HitDetection.transform.position;
        float angle = Vector3.Angle(dir, Vector3.down);

        if (objectX > playerX)
        {
            if(angle >= 135f)
            {
                SetAnimation(0, attacking3, true, 1f);
            }
            if(angle >= 45f && angle < 135f)
            {
                SetAnimation(0, attacking2, true, 1f);
            }
            if(angle >= 0f && angle < 45f)
            {
                SetAnimation(0, attacking, true, 1f);
            }
        }
        else if (objectX < playerX)
        {
            if (angle >= 135f)
            {
                SetAnimation(0, attacking6, true, 1f);
            }
            if (angle >= 45f && angle < 135f)
            {
                SetAnimation(0, attacking5, true, 1f);
            }
            if (angle >= 0f && angle < 45f)
            {
                SetAnimation(0, attacking4, true, 1f);
            }
        }
    }

    //In the animation, this will deal damage to the select unit
    public void TriggerDamage()
    {
        if (selectedEnemy != null)
        {
            selectedEnemy.GetComponent<Targetable>().TakeDamage();
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
                    Invoke("UseUltimate1", 1f);
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

        SetCharacterState(prevState);
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
            if(movementInput.y > 0)
            {
                SetAnimation(0, moving, true, 1f);
            }

            else if(movementInput.y < 0)
            {
                SetAnimation(0, moving2, true, 1f);
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
            SetAnimation(0, victorying, false, 1f);
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

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Artillery artillery = GameObject.FindGameObjectWithTag("Artillery").GetComponent<Artillery>();
            StartCoroutine(artillery.SpawnArtilleryWithDelay());
        }

        if (Input.GetKeyUp(KeyCode.K))
        {
           PlayerHealthScript playerhealth = GetComponent<PlayerHealthScript>();
           playerhealth.TakeDamage(300);
        }

        if (Input.GetKeyUp(KeyCode.X))
        {
            MissileManager missileManager = GameObject.FindGameObjectWithTag("MissileManager").GetComponent<MissileManager>();
            missileManager.StartEvent();
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
       if(isUltimate == true)
        {
            AudioClip UltimateAttackToPlay = attackSFX[2];
            attackAudioSource.PlayOneShot(UltimateAttackToPlay);
        }
        if(isIdle == true)
        {
            if (Time.time - idleRoarTimer > Random.Range(minRoarInterval, maxRoarInterval))
            {
                Debug.Log("Roar");
                roarAudioSource.PlayOneShot(monsterRoarSFX);
                // Reset the timer
                idleRoarTimer = Time.time;
            }         
        }
    }

    public void JumpSFX()
    {
        jumpAudioSource.PlayOneShot(jumpAudioSource.clip);
    }


}

