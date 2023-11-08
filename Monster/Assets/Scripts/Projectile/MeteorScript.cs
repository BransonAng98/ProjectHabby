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
    public GameObject player;
    public GameObject playerStatusBars;

    private ShakeScript shakeScript;
    public GameObject crater;

    public PlayerHandler playerHandler;

    float meteorRadius = 5f;
    public PlayerStatScriptableObject playerSO;
    public ClockSystem clock;
    public AudioSource meteorAudioSource;
    public AudioClip meteormovingSFX;
    public AudioClip meteorExplosionSFX;



    public void Start()
    {
        animator = GetComponent<Animator>();
        shakeScript = GameObject.Find("CM vcam1").GetComponent<ShakeScript>();
        playerHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        playerHandler.canMove = false;
        animator.SetBool("isMoving", true);

        Vector2 landingPos = new Vector2(player.transform.position.x + 0.9f, player.transform.position.y + 4f);
        targetPosition = landingPos;

        // Calculate the direction vector towards the target position
        direction = (targetPosition - (Vector2)transform.position).normalized;
        //hitcircle.SetActive(false);
        playerStatusBars.SetActive(false);
    }

    public void Shake()
    {
        shakeScript.CineShake();
    }

    public void ReassignFollow()
    {
        shakeScript.ReAssignCam();
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

                // Object has reached the target, you can add further actions here if needed.
                PlayExplosion();
                Vector2 explosionPos = new Vector2(transform.position.x, transform.position.y + 3f);
                transform.position = explosionPos;
                UseUltimate();
            }
        }

     

    }

    public void UseUltimate()
    {
        Vector2 explosionPos = new Vector2(transform.position.x, transform.position.y - 5f);
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(explosionPos, meteorRadius);
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
        animator.SetBool("isMoving", false);
    }

    public void DestroyMeteor()
    {
        Destroy(gameObject);
    }

    public void ActivatePlayer()
    {
        playerHandler.canMove = true;
        clock.startTime = true;
    }

    public void SpawnPlayer()
    {
        Vector2 spawnPos = new Vector2(player.transform.position.x, player.transform.position.y + 3f);
        Instantiate(crater, spawnPos, Quaternion.identity);

        player.GetComponent<MeshRenderer>().enabled = true;
        playerStatusBars.SetActive(true);
    }



    public void StartMoving()
    {
        isMoving = true;
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