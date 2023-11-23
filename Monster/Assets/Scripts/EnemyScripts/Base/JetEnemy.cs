
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetEnemy : MonoBehaviour
{
    public float moveSpeed = 10f;
    private Rigidbody2D rb;

    public Transform cameraTransform;

    public float destroyAfterSeconds = 12f;
    private float destroyTimer;

    private bool movingLeft;
 
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