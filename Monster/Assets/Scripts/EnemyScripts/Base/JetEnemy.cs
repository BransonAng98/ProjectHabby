
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetEnemy : MonoBehaviour
{
    public float moveSpeed = 10f;
    private Rigidbody2D rb;

    public Transform cameraTransform;

    //public GameObject missilePrefab;
    public Transform firingPoint;
    public float fireRate = 1f;
    private float nextFireTime = 0f;

    public float destroyAfterSeconds = 12f;
    private float destroyTimer;

    private bool movingLeft;
    private bool isPlayerWithinCollider = false; // Flag to check if the player is within the collider

    private void Start()
    {
        cameraTransform = GameObject.Find("Main Camera").GetComponent<Transform>();
        destroyTimer = 0;
        checkPosition();
    }

    private void FixedUpdate()
    {
        // Run the despawn timer
        DespawnTimer();
        //Check if can fire
        
    }

    //void CheckCanFire()
    //{

    //    if (isPlayerWithinCollider && Time.time > nextFireTime)
    //    {
    //        MissilesAway();
    //        nextFireTime = Time.time + 1.5f / fireRate;
    //    }
    //}

    void DespawnTimer()
    {
        destroyTimer += Time.deltaTime;

        if (destroyTimer >= destroyAfterSeconds) // Check if it's time to destroy the plane
        {
            Destroy(gameObject);
        }
    }


    void checkPosition()
    {
        if (transform.position.x > cameraTransform.position.x)
        {
            MoveLeft();
        }
        else
        {
            MoveRight();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isPlayerWithinCollider = true; // Player is within the collider
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isPlayerWithinCollider = false; // Player has exited the collider

        }
    }

    //public void SpawnMissiles()
    //{
    //    GameObject newMissile = Instantiate(missilePrefab, firingPoint.position, firingPoint.rotation);
    //    if (movingLeft == true)
    //    {
    //        newMissile.GetComponent<PlaneMissileScript>().isLeft = true;
    //    }
    //    else
    //    {
    //        newMissile.GetComponent<PlaneMissileScript>().isLeft = false;
    //    }

    //}

    void MoveLeft()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector3 scale = transform.localScale;
        scale.x *= 1;
        // Apply the new scale
        transform.localScale = scale;
        movingLeft = true;

        rb.velocity = new Vector2(-moveSpeed, 0f);

    }

    void MoveRight()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        // Apply the new scale
        transform.localScale = scale;
        movingLeft = false;

        rb.velocity = new Vector2(moveSpeed, 0f);
    }
}