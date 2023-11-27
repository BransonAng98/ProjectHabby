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
    public bool isActive;
    private Animator animator;
    public GameManagerScript gameManager;
    public GameObject crater;
    private CameraShake cameraShake;
    public PlayerHandler playerHandler;

    float meteorRadius = 4.8f;
    public PlayerStatScriptableObject playerSO;
    public AudioSource meteorAudioSource;
    public AudioClip meteormovingSFX;
    public AudioClip meteorExplosionSFX;

    public void Start()
    {
        isMoving = true;
        
        playerHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        cameraShake = FindObjectOfType<CameraShake>();


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
                ImpactDamage();
                
            }
        }
    }

    public void ImpactDamage()
    {
        PlayExplosion();
        SetShakeValues();
        cameraShake.ShakeCamera();
        MeteorCrashingSFX();
        SpawnCrater();
        Vector2 OverlapPos = new Vector2(transform.position.x, transform.position.y -5f);
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(OverlapPos, meteorRadius);
        foreach (Collider2D collider in hitColliders)
        {
            CollateralScript collateralTrigger = collider.GetComponent<CollateralScript>();
            if (collateralTrigger != null)
            {
                collateralTrigger.CollateralDamage(100f);
            }
            StartCoroutine(DestroyAfterDelay(collider.gameObject, 0f));
        }


        ResetShakeValues();
    }

    private IEnumerator DestroyAfterDelay(GameObject objToDestroy, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (objToDestroy.tag != "Player")
        {
            if(objToDestroy.tag == "Civilian")
            {
                Destroy(objToDestroy.transform.parent);
            }
            else
            {
                Destroy(objToDestroy);
            }
          
        }
    }

    void SetShakeValues()
    {
        cameraShake.shakeStrength = 5f;
        cameraShake.shakeFrequency = 4f;
    }
    void ResetShakeValues()
    {
        cameraShake.shakeStrength = 1.3f;
        cameraShake.shakeFrequency = 3f;
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

    public void SpawnCrater()
    {
        Instantiate(crater, playerHandler.transform.position, Quaternion.identity);
    }

    public void SpawnPlayer()
    {
        gameManager.SpawnPlayer();
    }

    public void ActivatePlayer()
    {
        gameManager.ActivatePlayer();
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