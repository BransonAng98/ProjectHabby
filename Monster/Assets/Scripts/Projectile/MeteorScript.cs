using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeteorScript : MonoBehaviour
{
    public Vector2 targetPosition;
    public Vector2 direction;
    public float speed = 2.0f; // Speed at which the object moves

    public bool isMoving;
    private Animator animator;
    public bool isActive;
    public GameManagerScript gamemanager;

    public GameObject crater;

    public PlayerHandler playerHandler;

    float meteorRadius = 8f;
    public PlayerStatScriptableObject playerSO;
    public AudioManagerScript audiomanager;
    public AudioSource meteorAudioSource;
    public AudioClip meteormovingSFX;
    public AudioClip meteorExplosionSFX;

    public void Start()
    {
        isMoving = true;
        
        playerHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        gamemanager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        
        animator = GetComponent<Animator>();
        animator.SetBool("isMoving", true);
       
        Vector2 landingPos = new Vector2(playerHandler.transform.position.x, playerHandler.transform.position.y + 5f);
        targetPosition = landingPos;

        // Calculate the direction vector towards the target position
        direction = (targetPosition - (Vector2)transform.position).normalized;
    }

    private void Update()
    {

        if (isMoving == true)
        {
            // Update the object's position
            //transform.position = (Vector2)transform.position + direction * speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Calculate the angle in radians
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Rotate the object to face the movement direction
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            // Check if the object has reached the target position
            if (Vector2.Distance(transform.position, targetPosition) <= 0f)
            {
                isMoving = false;
                transform.rotation = Quaternion.Euler(Vector3.zero);
                //Vector2 explosionPos = new Vector2(transform.position.x, transform.position.y);
                //transform.position = explosionPos;
                ImpactDamage();
            }
        }
    }

    public void ImpactDamage()
    {
        PlayExplosion();
        MeteorCrashingSFX();
        Crater();
        Vector2 OverlapPos = new Vector2(transform.position.x, transform.position.y);
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(OverlapPos, meteorRadius);
        foreach (Collider2D collider in hitColliders)
        {
            CollateralScript collateralTrigger = collider.GetComponent<CollateralScript>();
            if (collateralTrigger != null)
            {
                collateralTrigger.CollateralDamage(100f);
            }
        }
    }
    public void PlayExplosion()

    {
        Vector2 explosionPos = new Vector2(transform.position.x, transform.position.y + 3.3f);
        transform.position = explosionPos;
        animator.SetBool("isMoving", false);
    }

    public void DestroyMeteor()
    {
        Destroy(gameObject);
    }

    public void Crater()
    {
        Vector2 spawnPos = new Vector2(playerHandler.transform.position.x, playerHandler.transform.position.y + 3f);
        Instantiate(crater, spawnPos, Quaternion.identity);
    }

    public void SpawnPlayer()
    {
        gamemanager.SpawnPlayer();
    }

    public void ActivatePlayer()
    {
        gamemanager.ActivatePlayer();
    }

    public void MeteorMovingSFX()
    {
        meteorAudioSource.PlayOneShot(meteormovingSFX);
    }

    public void MeteorCrashingSFX()
    {
        meteorAudioSource.PlayOneShot(meteorExplosionSFX);
    }
}