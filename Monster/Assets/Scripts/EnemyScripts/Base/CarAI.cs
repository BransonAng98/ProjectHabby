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
    public GameObject smokeVFX;
    public GameObject sparkVFX;
    private Sprite intitialSprite;

    public AudioSource vehicleaudioSource;
    public AudioClip[] vehicleSFX;

    Collider2D entityCollider;
    bool hasDied;

    void Start()
    {
        eventManager = GameObject.FindGameObjectWithTag("EventManager").GetComponent<EventManager>();
        levelManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        entityCollider = GetComponent<Collider2D>();
        objectFader = GetComponent<ObjectFadeEffect>();

        lastPosition = transform.position;
        intitialSprite = spriteRenderer.sprite;

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
            Death();
        }
    }

    public void Death()
    {
        if (!hasTriggered)
        {
            levelManager.CalculateScore(3);
            hasTriggered = true;
        }

        if (!hasDied)
        {
            eventManager.AddScore();
            hasDied = true;
        }
        PlaySFX();

        GameObject smokePuff = Instantiate(smokeVFX, transform.position, Quaternion.identity);
        GameObject sparkPuff = Instantiate(sparkVFX, transform.position, Quaternion.identity);
        spriteRenderer.sprite = destroyedSprite;
        spriteRenderer.sortingOrder = 1;
        entityCollider.enabled = false;
        objectFader.StartFading();
    }

    public void PlaySFX()
    {
        AudioClip deathSFX = vehicleSFX[(Random.Range(0, vehicleSFX.Length))];
        vehicleaudioSource.PlayOneShot(deathSFX);
    }

    private void Update()
    {
        // Vector3 currentPosition = transform.position;

        // // Calculate the velocity or position change since the last frame.
        // Vector3 positionChange = currentPosition - lastPosition;

        // // Calculate the moving direction as a normalized vector.
        //// movingDirection = positionChange.normalized;



        // // Update the last position for the next frame.
        // lastPosition = currentPosition;

        // if(isDestroyed != true)
        // {
        //     FlipSprite(movingDirection);
        // }

        // if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
        // {
        //     ai.destination = PickRandomPoint();
        //     ai.SearchPath();
        // }
    }
}