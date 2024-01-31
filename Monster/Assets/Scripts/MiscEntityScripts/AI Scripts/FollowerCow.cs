using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerCow : MonoBehaviour
{
    public enum CowState
    {
        idle,
        roam,
        panic,
        detonate,
        death,
    } 
    
    //Public Variables
    public CowState entityState;
    public EnemyScriptableObject enemyData;
    public float changeDirectionInterval;
    public float maxWanderDistance;
    public Transform blockingEntity;
    public float detonationTime;
    public GameObject detonationeRadiusPf;
    public float blastRadiusMaxSize = 10f;
    public float blastExpansionSpeed = 2f;
    public Transform leaderTransform;
    public LayerMask obstacleLayer;
    public float movementRadius;

    //Private Variables
    private PlayerHandler inputHandler;
    private PlayerHealthScript playerHealth;
    private ScoreManagerScript scoreManager;
    private AudioManagerScript audioManager;
    private Collider2D entityCollider;
    private Animator anim;
    private float timeSinceLastDirectionChange;
    private Transform playerTransform;
    private Vector2 targetPosition;
    private Vector2 randomDirection;
    private float currentBlastRadiusSize;

    //Serializable Variables
    [SerializeField] float tempHealth;
    [SerializeField] float tempSpeed;
    [SerializeField] float tempDamage;
    [SerializeField] float tempRange;
    [SerializeField] float explosionRadius;
    [SerializeField] bool isBlocked;
    [SerializeField] bool isDetonating;
    [SerializeField] bool isTriggered;

    // Start is called before the first frame update
    void Start()
    {
        //External Check
        inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthScript>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        scoreManager = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManagerScript>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManger").GetComponent<AudioManagerScript>();

        //Internal Check
        entityCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();

        //Set Variables
        AssignStat();
    }

    void AssignStat()
    {
        tempHealth = enemyData.health;
        tempSpeed = enemyData.speed;
        tempDamage = enemyData.attackDamage;
        tempRange = enemyData.attackRange;
    }

    //Assign the leader variable when spawning the follower cows
    void AssignLeader(Transform leaderPos)
    {
        leaderTransform = leaderPos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDetonating)
        {
            if (collision.gameObject.tag == "BigBuilding")
            {
                blockingEntity = collision.gameObject.transform;
                isBlocked = true;
            }
            if (collision.gameObject.tag == "Blocker")
            {
                blockingEntity = collision.gameObject.transform;
                isBlocked = true;
            }
        }

        else
        {
            Instantiate(detonationeRadiusPf, transform.position, Quaternion.identity);
            entityState = CowState.detonate;
        }
    }

    void Idle()
    {
        anim.SetBool("isIdle", true);
    }

    void Roam()
    {
        anim.SetBool("isMove", true);
        if (!PlayerInRange())
        {
            if(leaderTransform != null)
            {
                float distanceToLeader = Vector3.Distance(transform.position, leaderTransform.position);

                // Move in a radius around the leader with collision avoidance
                MoveInRadiusWithAvoidance();
            }

            else
            {
                isDetonating = true;
                entityState = CowState.panic;

                if (!isTriggered)
                {
                    ChooseRandomDirection();
                }
            }
        }
        else
        {
            isDetonating = true;
            entityState = CowState.panic;

            if (!isTriggered)
            {
                ChooseRandomDirection();
            }
        }
    }

    void MoveInRadiusWithAvoidance()
    {
        Vector2 randomOffset = Random.insideUnitCircle * movementRadius;
        Vector3 targetPosition = new Vector3(leaderTransform.position.x + randomOffset.x, transform.position.y, leaderTransform.position.z + randomOffset.y);

        // Check for nearby colliders
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f, obstacleLayer);

        // Avoidance behavior
        foreach (var collider in colliders)
        {
            if (collider.transform != transform)
            {
                Vector3 avoidDirection = transform.position - collider.transform.position;
                targetPosition += avoidDirection.normalized * tempSpeed * Time.deltaTime;
            }
        }

        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, tempSpeed * Time.deltaTime);
    }

    void ChooseRandomDirection()
    {
        // Choose a random direction
        isTriggered = true;
        float randomAngle = Random.Range(0f, 360f);
        randomDirection = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));
    }

    void Panic()
    {
        transform.Translate(randomDirection * tempSpeed * Time.deltaTime);
    }

    void Detonate()
    {
        detonationTime -= Time.deltaTime;
        if (detonationTime <= 0)
        {
            Explode();
        }

        else
        {
            ExpandBlastRadius();
        }
    }
    void ExpandBlastRadius()
    {
        if (currentBlastRadiusSize < blastRadiusMaxSize)
        {
            currentBlastRadiusSize += Time.deltaTime * blastExpansionSpeed;
            UpdateBlastRadiusVisual();
        }
    }

    void UpdateBlastRadiusVisual()
    {
        // This assumes you have a circular blast radius visual represented by a GameObject
        detonationeRadiusPf.transform.localScale = new Vector3(currentBlastRadiusSize, currentBlastRadiusSize, 1f);
    }

    void Explode()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D collider in hitColliders)
        {
            Targetable affectedEntity = collider.GetComponent<Targetable>();
            if (affectedEntity != null)
            {
                affectedEntity.TakeDamage(tempDamage);
                entityState = CowState.death;
            }

            else
            {
                return;
            }
        }
    }

    void Death()
    {
        Destroy(transform.parent.gameObject);
    }

    bool PlayerInRange()
    {
        if (playerTransform != null)
        {
            // Calculate the distance between the enemy and the player
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // Check if the player is within the detection range
            return distanceToPlayer <= tempRange;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (entityState)
        {
            case CowState.idle:
                Idle();
                break;

            case CowState.roam:
                Roam();
                break;

            case CowState.panic:
                Panic();
                break;

            case CowState.detonate:
                Detonate();
                break;

            case CowState.death:
                Death();
                break;
        }
    }
}
