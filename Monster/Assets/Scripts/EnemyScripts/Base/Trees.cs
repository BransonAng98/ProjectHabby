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

    // New flag to track whether death has been triggered
    private bool deathTriggered = false;

    // Start is called before the first frame update
    void Start()
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
        if (isKicking == true)
        {
            Quaternion targetRotation = transform.rotation * Quaternion.Euler(0f, 0f, 90f);
            rotationSpeed = 5;
            // Smoothly interpolate towards the target rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Check if the tree rotation angle is greater than or equal to 90 and death has not been triggered
        if (Mathf.Abs(transform.rotation.eulerAngles.z) >= 90f && !deathTriggered)
        {
            entityCollider.enabled = false;
            // If so, set isKicking to false or perform any other desired actions
            rotationSpeed = 0;
            isKicking = false;
            // Set the flag to true to prevent repeated triggering
            deathTriggered = true;

            // Call the Death function
            Death();
        }
    }

    public void Death()
    {
        hasDied = true;
        spriteRenderer.sortingOrder = 2;
        rotationSpeed = 0;
        if (entityCollider == null)
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
            int random = Random.Range(0, 2);
            switch (random)
            {
                case 0:
                    if (!isKicking)
                    {
                        Death();
                    }
                    break;

                case 1:
                    isKicking = true;
                    //KickLogic(collision);
                    break;
            }
        }

    }

    void SpawnVFX()
    {
        Instantiate(hitVFX, transform.position, Quaternion.identity);
    }
}