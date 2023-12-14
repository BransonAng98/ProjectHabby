using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

public class CarAI : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    bool hasTriggered;

    private Vector3 randomDestination;
    public float roamRadius = 20.0f;
    public float roamInterval = 5.0f;
    private Transform player;
    private PlayerHandler playerscript;
    private Vector3 lastPosition;
    private Vector2 movingDirection;

    private SpriteRenderer spriteRenderer;
    private EventManager eventManager;
    private ObjectFadeEffect objectFader;

    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    public Sprite destroyedSprite;
    public Sprite verticaldestroyedSprite;

    //Kick Variables
    [SerializeField] public bool isKicking;
    [SerializeField] int kickForce = 4;
    [SerializeField] int rotationSpeed;
    public float stopDelay = 2f;

    //VFX
    public GameObject explosionVFX;
    public GameObject smokeVFX;
    public GameObject smokeTrailVFX;
    public GameObject kickedVFX;

    private Sprite intitialSprite;

    public AudioManagerScript audiomanager;
    public AudioSource vehicleaudioSource;
    public AudioClip[] vehicleSFX;

    public Collider2D entityCollider;
    public bool isVertical;
    bool hasDied;
    public vehicleFakeHeightScript fakeheight;
    public FadeObjectinParent fadescript;
    //public FadeObjectinParent fadescript;
    public Vector2 groundDispenseVelocity;
    public Vector2 verticalDispenseVelocity;
    public ScoreManagerScript scoremanager;
    

    void Start()
    {
        scoremanager = GameObject.Find("ScoreManager").GetComponent<ScoreManagerScript>();
        audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        playerscript = GetComponent<PlayerHandler>();
        //fadescript = GetComponentInParent<FadeObjectinParent>();
        fakeheight = GetComponentInParent<vehicleFakeHeightScript>();
        eventManager = GameObject.FindGameObjectWithTag("EventManager").GetComponent<EventManager>();
        levelManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        entityCollider = GetComponent<Collider2D>();
        objectFader = GetComponent<ObjectFadeEffect>();
        fadescript = GetComponentInParent<FadeObjectinParent>();

        lastPosition = transform.position;
        intitialSprite = spriteRenderer.sprite;
        smokeTrailVFX.SetActive(false);
        CheckOrientation();
        SetValue();
    }

    void SetValue()
    {
        //Randomize car rotation speed
        int setSpinValue = Random.Range(500, 700);
        rotationSpeed = setSpinValue;
    }

    public void SetSpriteRenderer(SpriteRenderer sr)
    {
        spriteRenderer = sr;
    }

    public void SetSpriteUp()
    {
        if (upSprite != null)
        {
            spriteRenderer.sprite = upSprite;
        }
    }
    public void SetSpriteDown()
    {
        if (downSprite != null)
        {
            spriteRenderer.sprite = downSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerLeg")
        {
            int random = Random.Range(0, 1 + 1);
            switch (random)
            {
                case 0:
                    if (!isKicking)
                    {
                        Death();
                    }
                    break;

                case 1:
                    KickLogic(collision);
                    break;
            }
        }

        if (isKicking)
        {
            if (collision.gameObject.tag == "BigBuilding")
            {
                Death();
                BigBuildingEnemy bigBuilding = collision.gameObject.GetComponent<BigBuildingEnemy>();
                if (bigBuilding != null)
                {
                    bigBuilding.TakeDamage(1);
                }
                Destroy(gameObject.transform.parent.gameObject);
            }

            if (collision.gameObject.tag == "Leader")
            {
                Leader leader = collision.gameObject.GetComponent<Leader>();
                if (leader != null)
                {
                    leader.enemyState = Leader.EnemyState.death;
                    leader.causeOfDeath = "Crushed by car";
                }
            }

            if (collision.gameObject.tag == "Civilian")
            {
                Civilian civilian = collision.gameObject.GetComponent<Civilian>();
                if (civilian != null)
                {
                    civilian.enemyState = Civilian.EnemyState.death;
                    civilian.causeOfDeath = "Crushed by car";
                }
            }

            if (collision.gameObject.tag == "Tree")
            {
                Trees tree = collision.gameObject.GetComponent<Trees>();
                if (tree != null)
                {
                    tree.Death();
                }
            }
        }

    }

    public void Death()
    {
        spriteRenderer.sortingOrder = 2;   
        scoremanager.amtOfCarskilled += 1;
        if (!hasTriggered)
        {
            levelManager.CalculateScore(3);
            hasTriggered = true;
        }

        if (!hasDied)
        {
            hasDied = true;
        }
        audiomanager.PlayCarSFX();
        //PlaySFX();

        GameObject explosion = Instantiate(explosionVFX, transform.position, Quaternion.identity);


        if (isVertical == true)
        {
            spriteRenderer.sprite = verticaldestroyedSprite;
        }
        if (isVertical == false)
        {
            spriteRenderer.sprite = destroyedSprite;
        }



        if (isKicking)
        { 
            return;
        }
        else
        {
            smokeTrailVFX.SetActive(false);
            GameObject smoke = Instantiate(smokeVFX, transform.position, Quaternion.Euler(-90, 0, 0));
            smoke.transform.SetParent(this.gameObject.transform);
            entityCollider.enabled = false;
        }

        fadescript.StartFading();
        
    }

    /*public void PlaySFX()
    {
        AudioClip deathSFX = vehicleSFX[(Random.Range(0, vehicleSFX.Length))];
        vehicleaudioSource.PlayOneShot(deathSFX);
    }*/

    public void DestroyObject()
    {
        fakeheight.Delete();
    }

    void KickLogic(Collider2D collision)
    {
        Instantiate(kickedVFX, transform.position, Quaternion.identity);
        smokeTrailVFX.SetActive(true);
        Vector2 kickDirection = transform.position - player.transform.position;
        Vector2 newDir =  kickDirection.normalized;
        isKicking = true;
        fakeheight.isGrounded = false;
        GetComponentInParent<vehicleFakeHeightScript>().Initialize(newDir * Random.Range(groundDispenseVelocity.x, groundDispenseVelocity.y), Random.Range(verticalDispenseVelocity.x, verticalDispenseVelocity.y));

        GetComponent<Rigidbody2D>().angularVelocity = rotationSpeed;
    }


    public void CheckOrientation()
    {
        if (spriteRenderer.sprite == upSprite || spriteRenderer.sprite == downSprite)
        {
            isVertical = true;
        }

        if (spriteRenderer.sprite == leftSprite || spriteRenderer.sprite == rightSprite)
        {
            isVertical = false;
        }
    }
}