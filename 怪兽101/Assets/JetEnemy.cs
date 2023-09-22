
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetEnemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    public Transform cameraTransform;
    private bool isToRightOfCamera = false;
    public float destroyTime = 5f;
    private float currentTime = 0f;

    public GameObject bombPrefab;
    public Transform firingPoint;
    public float fireRate = 1f;
    private float nextFireTime = 0f;

    private bool isPlayerWithinCollider = false; // Flag to check if the player is within the collider

    private void Start()
    {
        checkPosition();
    }

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= destroyTime)
        {
            Destroy(gameObject);
        }

        // Check if the player is within the collider and enough time has passed to drop another bomb
        if (isPlayerWithinCollider && Time.time > nextFireTime)
        {
            BombsAway();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void checkPosition()
    {
        if (transform.position.x > cameraTransform.position.x)
        {
            isToRightOfCamera = true;
            MoveLeft();
            Debug.Log("Object is to the right of the camera.");
        }
        else
        {
            isToRightOfCamera = false;
            MoveRight();
            Debug.Log("Object is not to the right of the camera.");
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

    void BombsAway()
    {
        GameObject newBomb = Instantiate(bombPrefab, firingPoint.position, firingPoint.rotation);
    }

    void MoveLeft()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(-moveSpeed, 0f);
    }

    void MoveRight()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(moveSpeed, 0f);
    }
}