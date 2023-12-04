using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeteorScript : MonoBehaviour
{
    public Vector2 targetPosition;
    public Vector2 direction;
    public float speed = 2.0f; // Speed at which the object moves

    [SerializeField] bool isTriggered;
    public bool isMoving;
    public bool isActive;
    private Animator animator;
    public GameManagerScript gameManager;
    public GameObject crater;
    private CameraShake cameraShake;
    public PlayerHandler playerHandler;
    public GameObject impactVFX;

    float meteorRadius = 3.4f;
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
                if (!isTriggered)
                {
                    ImpactDamage();
                }
            }
        }
    }

    public void ImpactDamage()
    {
        isTriggered = true;
        PlayExplosion();
        SetShakeValues();
        cameraShake.ShakeCamera();
        MeteorCrashingSFX();
        SpawnCrater();
        Vector2 OverlapPos = new Vector2(playerHandler.transform.position.x, playerHandler.transform.position.y +1f);
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(OverlapPos, meteorRadius);
        foreach (Collider2D collider in hitColliders)
        {
            CollateralScript collateralTrigger = collider.GetComponent<CollateralScript>();
            if (collateralTrigger != null)
            {
                collateralTrigger.CollateralDamage(100f);
                StartCoroutine(DestroyAfterDelay(collider.gameObject, 0f));
            }
           
        }


        ResetShakeValues();
    }

    private IEnumerator DestroyAfterDelay(GameObject objToDestroy, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (objToDestroy != null && objToDestroy.tag != "Player")
        {
            if (objToDestroy.tag == "Civilian")
            {
                Transform parentTransform = objToDestroy.transform.parent;
                if (parentTransform != null)
                {
                    objToDestroy.GetComponent<Civilian>().causeOfDeath = "Meteor shower";
                    Destroy(parentTransform.gameObject);
                }
            }
            else if (objToDestroy.tag == "Car")
            {
                Transform parentTransform = objToDestroy.transform.parent;
                if (parentTransform != null)
                {
                    Destroy(parentTransform.gameObject);
                }
            }
            else if (objToDestroy.name == "CamConfiner")
            {
                // do nothing
            }
            else if (objToDestroy.tag == "BigBuilding")
            {
                SpriteRenderer buildingRenderer = objToDestroy.GetComponentInChildren<SpriteRenderer>();
                if (buildingRenderer != null)
                {
                    buildingRenderer.enabled = false;
                }
            }
            else if (objToDestroy.tag == "Leader")
            {
                Transform parentTransform = objToDestroy.transform.parent;
                if (parentTransform != null)
                {
                    objToDestroy.GetComponent<Leader>().causeOfDeath = "Meteor shower";
                    Destroy(parentTransform.gameObject);
                }
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
        Instantiate(impactVFX, playerHandler.transform.position, Quaternion.identity);
        animator.SetBool("isMoving", false);
    }

    public void DestroyMeteor()
    {
        Destroy(gameObject);
    }

    public void SpawnCrater()
    {
        Vector2 craterPos = new Vector2(playerHandler.transform.position.x, playerHandler.transform.position.y + 1.8f); 
        Instantiate(crater, craterPos, Quaternion.identity);
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