using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Haptics.Vibrations;

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

    //Private Variable
    private Transform thiefTransform;
    private Collider2D entityCollider;
    private Skeleton skeleton;
    private SkeletonAnimation skeletonAnim;
    private Rigidbody2D rb;
    private Thief thiefEntity;
    private float TimeSinceLastTap;

    public GameObject helicopter;
    public float heliPlayerDistance;
    //Serilizable Variable
    [SerializeField] bool isCCed;
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

    public ERScoreManager erSM;

    // Start is called before the first frame update
    void Start()
    {
        //External Check
        thiefTransform = GameObject.FindGameObjectWithTag("Thief").GetComponent<Transform>();
        thiefEntity = GameObject.FindGameObjectWithTag("Thief").GetComponent<Thief>();
        erSM = GameObject.Find("ScoreManager").GetComponent<ERScoreManager>();
        //Internal Check
        rb = GetComponent<Rigidbody2D>();
        entityCollider = GetComponent<Collider2D>();

        //Setting Variables
        AssignStat();
    }

    void AssignStat()
    {
        tempHealth = playerData.health;
        tempSpeed = playerData.speed;
        tempAccel = playerData.acceleration;
        tempMaxAccel = playerData.maxAcceleration;
        tempCCCD = playerData.ccRecoverTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BigBuilding")
        {
            if (!enemyList.Contains(collision.transform))
            {
                if(currentState != PlayerState.attack)
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
                Debug.Log("Collided object is on the right.");
                canMoveRight = false;
            }
            else if (currentPos.x > objPos.x)
            {
                Debug.Log("Collided object is on the left.");
                canMoveLeft = false;
            }
            else
            {
                Debug.Log("Collided object is at the same position.");
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

                if(enemyList.Count == 0)
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

            if (moveX < 0 || moveX > 0)
            {
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
            }

            else
            {
                showMoveX = 0;
                if (rb.velocity.x > 0 || rb.velocity.x < 0)
                {
                    rb.drag = dragCoefficient;
                }
                else
                {
                    // If velocity is already zero, reset the drag to zero
                    rb.drag = 0f;
                }
            }

            //How fast the player is moving
            float velocityRatio = velocity.y / maxYVelocity;
            tempAccel = tempMaxAccel * (1 - velocityRatio);
            velocity.y += tempAccel * Time.deltaTime;
            if(velocity.y >= maxYVelocity)
            {
                velocity.y = maxYVelocity;
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
        Destroy(gameObject.transform.parent, 2f);
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

        float distance = distanceTravelled - thiefEntity.distanceTravelled;
        if(distance < distanceToMaintain)
        {
            if(distanceTimerCountdown < thresholdTime)
            {
                //Trigger Countdown here
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
            distanceTimerCountdown = 0f;
        }
    }

    void SpecialAttack()
    {
        canMove = false;
        //Stop the screen from moving

        if(thiefEntity != null)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    thiefEntity.brokenFree = false;
                    thiefEntity.TakeDamage(15);
                }

                if(touch.phase == TouchPhase.Ended)
                {
                    if(TimeSinceLastTap != tapTimeThreshold)
                    {
                        TimeSinceLastTap += Time.deltaTime;
                    }

                    else
                    {
                        TimeSinceLastTap = 0f;
                        thiefEntity.brokenFree = true;
                        Debug.Log("Thief has escaped");
                    }
                }
            }
        }
    }

    void Damaged()
    {
        canMove = false;
        entityCollider.enabled = false;
        Invoke("RecoverFromCC", tempCCCD);
    }

    void RecoverFromCC()
    {
        currentState = PlayerState.move;
        entityCollider.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        heliPlayerDistance = Vector2.Distance(helicopter.transform.position,transform.position);
        erSM.DistanceToTarget = Mathf.RoundToInt(heliPlayerDistance);
        CheckDistance();
        switch (currentState)
        {
            case PlayerState.move:
                Move();
                break;

            case PlayerState.attack:
                //Attack();
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
