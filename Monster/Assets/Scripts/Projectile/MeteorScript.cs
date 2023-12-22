using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Haptics.Vibrations;
public class MeteorScript : MonoBehaviour
{
    public Vector2 targetPosition;
    public Vector2 direction;
    public float speed = 2.0f; // Speed at which the object moves

    [SerializeField] bool isTriggered;
    public bool isMoving;
    public bool isActive;
    private bool isPlayed;
    public GameManagerScript gameManager;
    public GameObject crater;
    private CameraShake cameraShake;
    public PlayerHandler playerHandler;
    public GameObject impactVFX;

    public float meteorRadius;
    public PlayerStatScriptableObject playerSO;
    public AudioSource meteorAudioSource;
    public AudioClip meteormovingSFX;
    public AudioClip meteorExplosionSFX;
    public PlayerHealthScript healthscript;
    public void Start()
    {
        VibrateHaptics.Initialize();
        isMoving = true;

        if (!isPlayed)
        {
            MeteorMovingSFX();
            isPlayed = true;
        }

        playerHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        cameraShake = FindObjectOfType<CameraShake>();
        healthscript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthScript>();


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
                SetShakeValues();
                isMoving = false;
                transform.rotation = Quaternion.Euler(Vector3.zero);
                if (!isTriggered)
                {
                    ImpactDamage();
                    SpawnPlayer();
                    ActivatePlayer();
                    Invoke("DelayStopShake", 2f);
                    Invoke("DestroyMeteor", 4f);
                }
            }
        }
    }

    public void ImpactDamage()
    {
        gameManager.StartCountdown();
        isTriggered = true;
        PlayExplosion();
        cameraShake.ShakeCamera();
        Invoke("DelayStopShake", 0.6f);
        MeteorCrashingSFX();
        SpawnCrater();
        playerHandler.ChargeUltimate(45);
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
            else if(collider.gameObject.tag == "Prop")
            {
                Destroy(collider.gameObject);
            }
        }
    }

    private IEnumerator DestroyAfterDelay(GameObject objToSort, float delay)
    {
        healthscript.activateAbiliityBar = true;
        yield return new WaitForSeconds(delay);

        if (objToSort != null && objToSort.tag != "Player")
        {
            if (objToSort.tag == "Civilian")
            {
                SpriteRenderer parentTransform = objToSort.GetComponent<SpriteRenderer>();
                if (parentTransform != null)
                {
                    objToSort.GetComponent<Civilian>().causeOfDeath = "Meteor shower";
                    parentTransform.sortingOrder = 1;
                }
            }
            else if (objToSort.tag == "Car")
            {
                SpriteRenderer parentTransform = objToSort.GetComponent<SpriteRenderer>();
                if (parentTransform != null)
                {
                    parentTransform.sortingOrder = 1;
                }
            }
            else if (objToSort.tag == "Tree")
            {
                SpriteRenderer parentTransform = objToSort.GetComponent<SpriteRenderer>();
                if (parentTransform != null)
                {
                    parentTransform.sortingOrder = 1;
                }
            }
            else if (objToSort.name == "CamConfiner")
            {
                // do nothing
            }
            else if (objToSort.tag == "BigBuilding")
            {
                SpriteRenderer buildingRenderer = objToSort.GetComponentInChildren<SpriteRenderer>();
                if (buildingRenderer != null)
                {
                    buildingRenderer.sortingOrder = 1;
                }
            }
            else if (objToSort.tag == "Leader")
            {
                SpriteRenderer parentTransform = objToSort.GetComponent<SpriteRenderer>();
                if (parentTransform != null)
                {
                    objToSort.GetComponent<Leader>().causeOfDeath = "Meteor shower";
                    parentTransform.sortingOrder = 1;
                }
            }
            else
            {
                
            }
        }
    }

    void SetShakeValues()
    {
        cameraShake.shakeStrength = 5f;
        cameraShake.shakeFrequency = 4f;
    }
    void DelayStopShake()
    {
        cameraShake.StopShaking();
    }

    public void PlayExplosion()
    {
        Instantiate(impactVFX, playerHandler.transform.position, Quaternion.identity);
    }

    public void DestroyMeteor()
    {
        StopVibration();
        Destroy(gameObject);
    }

    public void SpawnCrater()
    {
        Vector2 craterPos = new Vector2(playerHandler.transform.position.x, playerHandler.transform.position.y + 1.8f); 
        Instantiate(crater, craterPos, Quaternion.identity);

    }

    public void SpawnPlayer()
    {
        VibrateHaptics.VibrateHeavyClick();
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

    void StopVibration()
    {
        VibrateHaptics.Release();
    }
}