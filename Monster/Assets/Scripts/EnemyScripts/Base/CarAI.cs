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
    public Sprite verticaldestroyedSprite;

    //Kick Variables
    [SerializeField] private bool isKicking;
    public float kickForce;
    public float rotationSpeed = 180f;
    public float stopDelay = 2f;

    //VFX
    public GameObject explosionVFX;
    public GameObject smokeVFX;

    private Sprite intitialSprite;

    public AudioSource vehicleaudioSource;
    public AudioClip[] vehicleSFX;

    Collider2D entityCollider;
    public bool isVertical;
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
        CheckOrientation();

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
            int random = Random.Range(0, 1 +1);
            switch (random)
            {
                case 0:
                    Death();
                        break;

                case 1:
                    KickLogic(collision);
                    break;
            }
        }

        if (collision.gameObject.tag == "BigBuilding")
        {
            if (isKicking)
            {
                BigBuildingEnemy bigBuilding = collision.gameObject.GetComponent<BigBuildingEnemy>();
                if (bigBuilding != null)
                {
                    bigBuilding.TakeDamage(1);
                }
                Destroy(gameObject);
            }
            else { return; }
            
        }

        if (collision.gameObject.tag == "Civilian")
        {
            if (isKicking)
            {
                Debug.Log(collision.gameObject.name);
                Civilian civilian = collision.gameObject.GetComponent<Civilian>();
                if (civilian != null)
                {
                    civilian.enemyState = Civilian.EnemyState.death;
                }
            }
            else { return; }
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
            GameObject smoke = Instantiate(smokeVFX, transform.position, Quaternion.Euler(-90, 0, 0));
            smoke.transform.SetParent(this.gameObject.transform);
            entityCollider.enabled = false;
        }
        objectFader.StartFading();
    }

    public void PlaySFX()
    {
        AudioClip deathSFX = vehicleSFX[(Random.Range(0, vehicleSFX.Length))];
        vehicleaudioSource.PlayOneShot(deathSFX);
    }
   
    void KickLogic(Collider2D collision)
    {
        isKicking = true;
        Death();
        Vector2 kickDirection = transform.position - collision.transform.position;

        // Normalize the direction vector to maintain consistent force regardless of distance
        kickDirection.Normalize();

        // Apply a force in the opposite direction
        GetComponent<Rigidbody2D>().AddForce(kickDirection * kickForce, ForceMode2D.Impulse);

        GetComponent<Rigidbody2D>().angularVelocity = rotationSpeed;

        StartCoroutine(StopMovementAfterDelay());
    }

    IEnumerator StopMovementAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(stopDelay);

        // Stop the movement by setting the velocity to zero
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        // Stop the rotation
        GetComponent<Rigidbody2D>().angularVelocity = 0f;
    }

    public void CheckOrientation()
    {
        if(spriteRenderer.sprite == upSprite || spriteRenderer.sprite == downSprite)
        {
            isVertical = true;
        }

        if (spriteRenderer.sprite == leftSprite || spriteRenderer.sprite == rightSprite)
        {
            isVertical = false;
        }
    }

    private void Update()
    {
       
    }
}