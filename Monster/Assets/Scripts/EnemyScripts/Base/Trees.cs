using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trees : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite destroyedSprite;
    public GameObject hitVFX;

    private ObjectShakeScript shakeScript;
    private bool isShake;
    public Collider2D entityCollider;

    public AudioManagerScript audiomanager;

    public AudioSource treeaudioSource;
    public AudioClip[] treeSFX;

    public bool isKicking;
    public treeFakeHeightScript fakeheight;
    private Transform player;
    public Vector2 groundDispenseVelocity;
    public Vector2 verticalDispenseVelocity;
    [SerializeField] int rotationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        entityCollider = GetComponent<Collider2D>();
        shakeScript = GetComponent<ObjectShakeScript>();
        audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        fakeheight = GetComponentInParent<treeFakeHeightScript>();
        SetValue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetValue()
    {
        int setSpinValue = Random.Range(800, 100);
        rotationSpeed = setSpinValue;
    }

    public void Death()
    {
        spriteRenderer.sortingOrder = 2;
        rotationSpeed = 0;
        if(entityCollider == null)
        {
            return;
        }

        else
        {
            SpawnVFX();
            audiomanager.PlayTreeSFX();
            entityCollider.enabled = false;
            if (isShake != true)
            {
                shakeScript.StartShake();
                isShake = true;
            }
            spriteRenderer.sprite = destroyedSprite;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerLeg"))
        {
            int random = Random.Range(0, 6);
            
            if(random == 0)
            {
                KickLogic(collision);
            }
            else if (!isKicking)
            {
                Death();
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
                Death();
                Leader leader = collision.gameObject.GetComponent<Leader>();
                if (leader != null)
                {
                    leader.enemyState = Leader.EnemyState.death;
                    leader.causeOfDeath = "Crushed by Tree";
                }
            }

            if (collision.gameObject.tag == "Civilian")
            {
                Death();
                Civilian civilian = collision.gameObject.GetComponent<Civilian>();
                if (civilian != null)
                {
                    civilian.enemyState = Civilian.EnemyState.death;
                    civilian.causeOfDeath = "Crushed by Tree";
                }
            }
        }
    }

    void KickLogic(Collider2D collision)
    {
        spriteRenderer.sortingOrder = 4;
        Vector2 kickDirection = transform.position - player.transform.position;
        Vector2 newDir = kickDirection.normalized;
        isKicking = true;
        fakeheight.isGrounded = false;
        GetComponentInParent<treeFakeHeightScript>().Initialize(newDir * Random.Range(groundDispenseVelocity.x, groundDispenseVelocity.y), Random.Range(verticalDispenseVelocity.x, verticalDispenseVelocity.y));

        GetComponent<Rigidbody2D>().angularVelocity = rotationSpeed;
    }

    void SpawnVFX()
    {
        Instantiate(hitVFX, transform.position, Quaternion.identity);
    }

    /*public void PlaySFX()
    {
        AudioClip deathSFX = treeSFX[(Random.Range(0, treeSFX.Length))];
        treeaudioSource.PlayOneShot(deathSFX);
    }*/
}
