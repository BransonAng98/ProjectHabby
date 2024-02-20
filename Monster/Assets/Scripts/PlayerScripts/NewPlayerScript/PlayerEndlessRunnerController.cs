using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Spine;
using TMPro;
using Haptics.Vibrations;
using TMPro;

public class PlayerEndlessRunnerController : MonoBehaviour
{
    public enum PlayerState
    {
        move,
        attack,
        skill,
        death,
        SpecialAttack,
        damaged,
    }

    //Public Variable
    public EndlessRunnerPlayerSO playerData;
    public Joystick joystick;
    public LayerMask targetLayer;
    public List<Transform> enemyList = new List<Transform>();
    public float distanceToMaintain;
    public float thresholdTime;
    public bool canMove;
    public bool canAttack;
    public Vector2 velocity;
    public float maxYVelocity = 100f;
    public float distanceTravelled;
    public float tapTimeThreshold;
    public PlayerState currentState;
    public float dragCoefficient = 0.1f;
    public Transform minDist;
    public LayerMask enemyLayer;
    public TextMeshProUGUI distText;
    public Image vignette;

    public float knockbackForce = 100f;
    public float knockbackDuration = 10f;
    public float flashSpeed;
    public bool isCCed;
    public GameObject loseScreen;
    public TextMeshProUGUI tapText;

    //Private Variable
    private Transform thiefTransform;
    private Collider2D entityCollider;
    private Skeleton skeleton;
    private SkeletonAnimation skeletonAnim;
    private Rigidbody2D rb;
    private Thief thiefEntity;
    private float knockbackTimer;

    public GameObject helicopter;
    public float heliPlayerDistance;
    public bool gameEnd;

    private Color normalColor = Color.white;
    private Color enlargedColor = Color.red;

    //Serilizable Variable
    [SerializeField] bool canMoveLeft = true; 
    [SerializeField] bool canMoveRight = true;
    [SerializeField] Vector2 movementInput;
    [SerializeField] float distanceTimerCountdown;
    [SerializeField] float ccRecoverTime;

    [SerializeField] float tempHealth;
    [SerializeField] float tempSpeed;
    [SerializeField] float tempAccel;
    [SerializeField] float tempMaxAccel;
    [SerializeField] float tempCCCD;
    [SerializeField] float showMoveX;
    [SerializeField] bool tapRecorded;
    [SerializeField] private float timeSinceLastTap;
    [SerializeField] Vector2 startingPos;

    public ERScoreManager erSM;
    public GridSpawner gridspawnerScript;

    // Start is called before the first frame update
    void Start()
    {
        //External Check
        thiefTransform = GameObject.FindGameObjectWithTag("Thief").GetComponent<Transform>();
        thiefEntity = GameObject.FindGameObjectWithTag("Thief").GetComponent<Thief>();
        erSM = GameObject.Find("ScoreManager").GetComponent<ERScoreManager>();
        gridspawnerScript = GameObject.Find("GridSpawner").GetComponent<GridSpawner>();

        //Internal Check
        rb = GetComponent<Rigidbody2D>();
        entityCollider = GetComponent<Collider2D>();
        startingPos = GetComponentInParent<Transform>().position;

        //Setting Variables
        AssignStat();
        loseScreen.gameObject.SetActive(false);
        tapText.gameObject.SetActive(false);
    }

    void AssignStat()
    {
        tempHealth = playerData.health;
        tempSpeed = playerData.speed;
        tempAccel = playerData.acceleration;
        tempMaxAccel = playerData.maxAcceleration;
        tempCCCD = playerData.ccRecoverTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "MilitaryBuilding")
        {
            
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;

            // Apply knockback force
            rb.velocity = Vector2.zero;
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

            // Start knockback timer
            knockbackTimer = knockbackDuration;

            Debug.Log("Collision");

            currentState = PlayerState.damaged;

            Destroy(collision.gameObject, 0.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BigBuilding")
        {
            if (!enemyList.Contains(collision.transform))
            {
                if(currentState != PlayerState.attack && currentState != PlayerState.SpecialAttack)
                {
                    currentState = PlayerState.attack;
                }

                enemyList.Add(collision.transform);
            }

            else
            {
                return;
            }
        }

        if (collision.gameObject.tag == "Limits")
        {
            Vector3 objPos = collision.gameObject.transform.position;
            Vector3 currentPos = transform.position;

            if (currentPos.x < objPos.x)
            {
                canMoveRight = false;

                if(showMoveX == 0)
                {
                    rb.velocity = Vector2.zero;
                }
            }
            else if (currentPos.x > objPos.x)
            {
                canMoveLeft = false;

                if (showMoveX == 0)
                {
                    rb.velocity = Vector2.zero;
                }
            }
            else
            {
                return;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BigBuilding")
        {
            if (enemyList.Contains(collision.transform))
            {
                enemyList.Remove(collision.transform);

                if(enemyList.Count == 0 && currentState != PlayerState.SpecialAttack)
                {
                    currentState = PlayerState.move;
                }
            }

            else
            {
                return;
            }
        }

        if (collision.gameObject.tag == "Limits")
        {
            if (!canMoveLeft)
            {
                canMoveLeft = true;
            }

            if (!canMoveRight)
            {
                canMoveRight = true;
            }
        }
    }

    void Move()
    {
        if (isCCed)
        {
            canMove = false;
            currentState = PlayerState.damaged;
        }

        else
        {
            canMove = true;

            float moveX = joystick.Horizontal;
            showMoveX = moveX;

            rb.drag = 0;

            if (canMoveLeft != true && moveX < 0)
            {
                moveX = 0;
            }

            if (canMoveRight != true && moveX > 0)
            {
                moveX = 0;
            }

            movementInput = new Vector2(moveX, 0).normalized;
            rb.velocity = movementInput * tempSpeed;

            //How fast the player is moving

            if (isCCed)
            {
                velocity.y = 0;
            }

            else
            {
                float velocityRatio = velocity.y / maxYVelocity;
                tempAccel = tempMaxAccel * (1 - velocityRatio);
                velocity.y += tempAccel * Time.deltaTime;
                if (velocity.y >= maxYVelocity)
                {
                    velocity.y = maxYVelocity;
                }
            }
        }
    }

    void Attack()
    {
        TriggerDamage();
    }

    public void TriggerDamage()
    {
        if (enemyList.Count != 0)
        {
            //Destroy(enemyList[0].gameObject);
            enemyList[0].GetComponent<Targetable>().TakeDamage(30);
        }

        else { return; }
    }

    void Skill()
    {
        // Need to decide on the skill
    }

    void Death()
    {
        canMove = false;
        entityCollider.enabled = false;
        Destroy(gameObject, 2f);
        loseScreen.gameObject.SetActive(true);
    }

    void CheckDistance()
    {
        if (canMove)
        {
            distanceTravelled += velocity.y * Time.deltaTime;
        }

        else
        {
            velocity = Vector2.zero;
        }

        float distance = thiefEntity.distanceTravelled - distanceTravelled;
        if(distance > distanceToMaintain)
        {
            FlashVignette();

            if (distanceTimerCountdown < thresholdTime)
            {
                //Trigger Countdown here
                Debug.Log("Trigger end game countdown");
                distanceTimerCountdown += Time.deltaTime;
            }

            else
            {
                if (currentState != PlayerState.death)
                {
                    currentState = PlayerState.death;
                }
            }
        }

        else
        {
            vignette.enabled = false;
            distText.color = Color.white;
            Debug.Log("Reset end game countdown");
            distanceTimerCountdown = 0f;
        }
    }

    void MovePlayerForSpecial()
    {
        if(transform.position != minDist.position)
        {
            Vector2 movePosition = new Vector2(minDist.position.x, minDist.position.y + 6f);
            transform.position = Vector3.MoveTowards(transform.position, movePosition, tempSpeed * Time.deltaTime);
        }

        else
        {
            return;
        }
    }

    void SpecialAttack()
    {
        entityCollider.enabled = false;
        canMove = false;
        joystick.gameObject.SetActive(false);

        //Reset distance travelled for both objects
        distanceTravelled = 0;
        thiefEntity.distanceTravelled = 0;
        
        //Stop the screen from moving
        MovePlayerForSpecial();
        if(thiefEntity != null)
        {
            tapText.gameObject.SetActive(true);
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    Debug.Log("Tap registered");
                    tapRecorded = true;
                    timeSinceLastTap = 0;
                    thiefEntity.brokenFree = false;
                    thiefEntity.TakeDamage(5);
                }

                if(touch.phase == TouchPhase.Ended)
                {
                    tapRecorded = false;
                }
            }
        }

        if (!tapRecorded)
        {
            if (timeSinceLastTap < tapTimeThreshold)
            {
                timeSinceLastTap += 1f * Time.deltaTime;
            }

            else
            {
                tapText.gameObject.SetActive(false);
                entityCollider.enabled = true;
                transform.position = Vector3.MoveTowards(transform.position, startingPos, tempSpeed * Time.deltaTime);
                timeSinceLastTap = 0f;
                tapRecorded = false;
                thiefEntity.brokenFree = true;
                joystick.gameObject.SetActive(true);
                currentState = PlayerState.move;
                Debug.Log("Thief has escaped");
            }
        }
    }

    void Damaged()
    {
        canMove = false;
        entityCollider.enabled = false;
        isCCed = true;
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
            {
                // Reset velocity after knockback duration expires
                rb.velocity = Vector2.zero;
                float reduceVelocity = maxYVelocity * 0.3f;
                float holderVelocity = velocity.y - reduceVelocity;
                velocity.y = holderVelocity; 
            }
        }

        else
        {
            Debug.Log("Moving back Phase");
            if (transform.position.y != startingPos.y)
            {
                Debug.Log("Moving back to starting pos:" + startingPos);
                Vector2 movePos = new Vector2(transform.position.x, startingPos.y);
                float recoverSpeed = tempSpeed / 2f;
                transform.position = Vector3.MoveTowards(transform.position, movePos, recoverSpeed * Time.deltaTime);
            }

            else
            {
                Debug.Log("Moved back to OG pos");
                isCCed = false;
                currentState = PlayerState.move;
                entityCollider.enabled = true;
            }
        }
    }

    public void FlashVignette()
    {
        distText.color = Color.Lerp(normalColor, enlargedColor, Mathf.PingPong(Time.time * flashSpeed, 1f));
        vignette.enabled = true;

        float alpha = Mathf.PingPong(Time.time * flashSpeed, 1f);

        alpha = Mathf.Lerp(0f, 1f, alpha);

        Color vignetteColor = vignette.color;
        vignetteColor.a = alpha;
        vignette.color = vignetteColor;
    }

    // Update is called once per frame
    void Update()
    {
        heliPlayerDistance = Vector2.Distance(helicopter.transform.position,transform.position);
        erSM.DistanceToTarget = Mathf.RoundToInt(heliPlayerDistance);
        if (gameEnd)
        {
            vignette.enabled = false;
            return;
        }

        else
        {
            CheckDistance();
        }
        switch (currentState)
        {
            case PlayerState.move:
                Move();
                break;

            case PlayerState.attack:
                Attack();
                Move();
                break;

            case PlayerState.skill:
                Skill();
                break;

            case PlayerState.death:
                Death();
                break;

            case PlayerState.SpecialAttack:
                SpecialAttack();
                break;

            case PlayerState.damaged:
                Damaged();
                break;
        }
    }
}
