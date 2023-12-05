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
        if(hasDied != true)
        {
            Checkfall();
        }
        else
        {

        }

    }

    void SetValue()
    {
        int setSpinValue = Random.Range(800, 100);
        rotationSpeed = setSpinValue;
    }

    public void Death()
    {
        hasDied = true;
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
            int random = Random.Range(0, 0);
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
     
    }


    public void Checkfall()
    {
        float targetRotation = 90f;
        float currentRotation = transform.rotation.eulerAngles.z % 360; // Get rotation within [0, 360) range
        float rotationDifference = Mathf.Abs(currentRotation - targetRotation);

        // Check if the current rotation is close to the target within a specific range
        if (rotationDifference < 1f || rotationDifference > 359f)
        {
            // Tree has rotated close to 180 degrees (or completed a full rotation), stop rotating
            GetComponent<Rigidbody2D>().angularVelocity = 0f;
            Death();
            Debug.Log("Tree has rotated 180 degrees!");
        }
    }
    void KickLogic(Collider2D collision)
    {
        Debug.Log("Kicked");
        entityCollider.enabled = false;

        GetComponent<Rigidbody2D>().angularVelocity = rotationSpeed;
    }

    void SpawnVFX()
    {
        Instantiate(hitVFX, transform.position, Quaternion.identity);
    }

  
}
