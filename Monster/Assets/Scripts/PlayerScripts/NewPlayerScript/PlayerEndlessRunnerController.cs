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
    public PlayerStatScriptableObject playerData;
    public AnimationReferenceAsset movingDown, movingLeft, movingRight, attacking1, attacking2, attacking3, ultimating, victorying, defeating, exhausting, damaging;
    public string currentAnimation;
    public Joystick joystick;
    public LayerMask targetLayer;
    public List<Transform> enemyList = new List<Transform>();
    public float distanceToMaintain;
    public float thresholdTime;
    public bool canMove;

    //Private Variable
    private PlayerState currentState;
    private Transform thiefTransform;
    private Skeleton skeleton;
    private SkeletonAnimation skeletonAnim;
    private Collider2D entityCollider;
    private Rigidbody2D rb;

    //Serilizable Variable
    [SerializeField] bool isCCed;
    [SerializeField] bool canMoveLeft;
    [SerializeField] bool canMoveRight;
    [SerializeField] Vector2 movementInput;
    [SerializeField] float distanceTimerCountdown;

    // Start is called before the first frame update
    void Start()
    {
        //External Check
        thiefTransform = GameObject.FindGameObjectWithTag("Thief").GetComponent<Transform>();

        //Internal Check
        rb = GetComponent<Rigidbody2D>();

        //Setting Variables

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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
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
                canMoveLeft = true;
            }
            else
            {
                Debug.Log("Collided object is at the same position.");
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Reset conditions
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
            
            if(canMoveLeft != true && moveX < 0)
            {
                moveX = 0;
            }

            if(canMoveRight != true && moveX > 0)
            {
                moveX = 0;
            }

            movementInput = new Vector2(moveX, 0).normalized;
            rb.velocity = movementInput;
        }
    }

    void Attack()
    {
        if (isCCed)
        {
            canMove = false;
            currentState = PlayerState.damaged;
        }

        else
        {
            canMove = true;
        }
    }

    public void TriggerDamage()
    {
        if (enemyList.Count != 0)
        {
            enemyList[0].GetComponent<Targetable>().TakeDamage(30);
            //attackCount += 1;
        }

        else { return; }
    }

    void Skill()
    {

    }

    void Death()
    {
        canMove = false;
    }

    void CheckDistance()
    {
        float distance = Vector3.Distance(transform.position, thiefTransform.position);

        if(distance > distanceToMaintain)
        {
            if(distanceTimerCountdown > thresholdTime)
            {
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
    }

    void Damaged()
    {
        canMove = false;
        Invoke("RecoverFromCC", 1.5f);
    }

    void RecoverFromCC()
    {
        currentState = PlayerState.move;
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistance();

        switch (currentState)
        {
            case PlayerState.move:
                Move();
                break;

            case PlayerState.attack:
                Attack();
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
