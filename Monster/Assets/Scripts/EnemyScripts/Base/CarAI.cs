using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;
using Haptics.Vibrations;
using System.Linq;
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
    public Sprite upperrightSprite;
    public Sprite lowerrightSprite;
    public Sprite upperleftSprite;
    public Sprite lowerleftSprite;
    public Sprite destroyedSprite;
    public Sprite verticaldestroyedSprite;

    //Kick Variables
    [SerializeField] public bool isKicking;
    [SerializeField] int kickForce = 4;
    [SerializeField] int rotationSpeed;
    public float stopDelay = 2f;

    //VFX
    public GameObject smokeTrailVFX;

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



    // Walk speed that can be set in Inspector
    [SerializeField]
    private float moveSpeed = 2f;

    void Start()
    {
        VibrateHaptics.Initialize();
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

        // Set position of Enemy as position of the first waypoint
        //transform.position = waypoints[waypointIndex].transform.position;

    }

    private void Update()
    {
        Move();
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

    public void SetSpriteLeft()
    {
        if (leftSprite != null)
        {
            spriteRenderer.sprite = leftSprite;
        }
    }

    public void SetSpriteRight()
    {
        if (rightSprite != null)
        {
            spriteRenderer.sprite = rightSprite;
        }
    }

    public void SetSpriteUpperRight()
    {
        if (upperrightSprite != null)
        {
            spriteRenderer.sprite = upperrightSprite;
        }
    }

    public void SetSpriteLowerRight()
    {
        if (lowerrightSprite != null)
        {
            spriteRenderer.sprite = lowerrightSprite;
        }
    }

    public void SetSpriteUpperLeft()
    {
        if (upperleftSprite != null)
        {
            spriteRenderer.sprite = upperleftSprite;
        }
    }

    public void SetSpriteLowerLeft()
    {
        if (lowerleftSprite != null)
        {
            spriteRenderer.sprite = lowerleftSprite;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerLeg")
        {
            moveSpeed = 0f;
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
  

    // Method that actually make Enemy walk
    private void Move()
    {

    }


    public void Death()
    {
        VibrateHaptics.VibrateHeavyClick();
        Invoke("StopVibration", 1f);
        spriteRenderer.sortingOrder = 2;
        scoremanager.amtOfCarskilled += 1;
        scoremanager.goldearned += 3;
        if (!hasTriggered)
        {
            levelManager.CalculateScore(1);
            hasTriggered = true;
        }

        if (!hasDied)
        {
            hasDied = true;
        }
        audiomanager.PlayCarSFX();
        //PlaySFX();
        ObjectPooler.Instance.SpawnFromPool("FireExplosionB", transform.position, Quaternion.identity);

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
            GameObject smoke = ObjectPooler.Instance.SpawnFromPool("CarSmoke", transform.position, Quaternion.Euler(-90, 0, 0));
            smoke.transform.SetParent(this.gameObject.transform);
            entityCollider.enabled = false;
        }

        fadescript.StartFading();

    }

    void StopVibration()
    {
        VibrateHaptics.Release();
    }

    public void DestroyObject()
    {
        fakeheight.Delete();
    }

    void KickLogic(Collider2D collision)
    {
        VibrateHaptics.VibrateTick();
        ObjectPooler.Instance.SpawnFromPool("CarKick", transform.position, Quaternion.identity);
        smokeTrailVFX.SetActive(true);
        Vector2 kickDirection = transform.position - player.transform.position;
        Vector2 newDir = kickDirection.normalized;
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