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
    public bool hasDied;
    public treeFakeHeightScript fakeheight;
    private Transform player;
    public Vector2 groundDispenseVelocity;
    public Vector2 verticalDispenseVelocity;
    [SerializeField] int rotationSpeed;
    public bool fallleft;
    public bool fallright;

    // New flag to track whether death has been triggered
    private bool deathTriggered = false;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        entityCollider = GetComponent<Collider2D>();
        shakeScript = GetComponent<ObjectShakeScript>();
        audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        fakeheight = GetComponentInParent<treeFakeHeightScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(fallleft == true)
        {
            TreeFallLeft();
        }

        if(fallright == true)
        {
            TreeFallRight();
        }
    }

    public void Death()
    {
        fallright = false;
        fallleft = false;
        entityCollider.enabled = false;
        hasDied = true;
        spriteRenderer.sortingOrder = 2;
        rotationSpeed = 0;

        if (entityCollider != null)
        {
            SpawnVFX();
            audiomanager.PlayTreeSFX();
            entityCollider.enabled = false;

            if (!isShake)
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
            entityCollider.enabled = false;
            int random = Random.Range(0, 3);
            switch (random)
            {
                case 0:
                    if (!isKicking)
                    {
                        Death();
                    }
                    break;

                case 1:
                    fallleft = true;
                   
                    break;
                case 2:
                    fallright = true;
                    break;
            }
        }

    }

    public void TreeFallLeft()
    {
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0f, 0f, 90f);
        rotationSpeed = 10;
        // Smoothly interpolate towards the target rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        float normalizedAngle = (transform.rotation.eulerAngles.z + 360) % 360;
        if (normalizedAngle >= 90f && !deathTriggered)
        {
            // If so, set isKicking to false or perform any other desired actions
            rotationSpeed = 0;
            isKicking = false;
            fallleft = false;
            // Set the flag to true to prevent repeated triggering
            deathTriggered = true;
            // Call the Death function
            Death();
        }
    }
    
    public void TreeFallRight()
    {
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0f, 0f, -90f);
        rotationSpeed = 10;
        // Smoothly interpolate towards the target rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        float normalizedAngle = (transform.rotation.eulerAngles.z + 360) % 360;
        if (normalizedAngle <= 270f && !deathTriggered)
        {
            // If so, set isKicking to false or perform any other desired actions
            rotationSpeed = 0;
            isKicking = false;
            fallright = false;
            // Set the flag to true to prevent repeated triggering
            deathTriggered = true;
            Death();
        }
    }

    void SpawnVFX()
    {
        ObjectPooler.Instance.SpawnFromPool("TreeHit", transform.position, Quaternion.identity);
    }
}