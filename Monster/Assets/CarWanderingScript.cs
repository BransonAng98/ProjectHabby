using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class CarWanderingScript : MonoBehaviour
{
    [SerializeField]
    private float radius = 50;

    IAstarAI ai;
    public CarAI carscript;


    Vector3 previousPosition;
    [SerializeField]
    private Vector2 delta;
    [SerializeField]
    private float scale = 0f;

    [SerializeField]
    private float detectionRadius = 1; // New variable for detection radius

    // Bools
    public bool goingup;
    public bool goingleft;
    public bool goingright;
    public bool goingdown;

    void Start()
    {
        ai = GetComponent<IAstarAI>();
        carscript = gameObject.GetComponent<CarAI>();
    }
    Vector3 PickRandomPoint()
    {
        var point = Random.insideUnitSphere * radius;
        point.y = 0;
        point += ai.position;
        return point;
    }
    void Update()
    {
        delta = transform.position - previousPosition;
        previousPosition = transform.position;
        SetSpriteDirection();
        CheckPlayerProximity();

        if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
        {
            ai.destination = PickRandomPoint();
            Vector2 direction = (ai.destination - ai.position).normalized;
            ai.SearchPath();
        }
    }

    void SetSpriteDirection()
    {
        const float thresshold = 0.03f;
        if (delta.x > thresshold)//going right
        {
            goingright = true;

            carscript.SetSpriteRight();

            if (goingup == true)
            {
                carscript.SetSpriteUpperRight();
                goingright = false;
                goingup = false;
            }

            if (goingdown == true)
            {
                carscript.SetSpriteLowerRight();
                goingright = false;
                goingdown = false;
            }
        }
        else if (delta.x < -thresshold) // going left
        {
            goingleft = true;

            carscript.SetSpriteLeft();

            if (goingup == true)
            {
                carscript.SetSpriteUpperLeft();
                goingup = false;
                goingleft = false;
            }

            if (goingdown == true)
            {
                carscript.SetSpriteLowerLeft();
                goingdown = false;
                goingleft = false;
            }
        }
        else // delta.x is close to zero (not moving horizontally)
        {
            if (delta.y > thresshold) // going up 
            {
                goingup = true;

                carscript.SetSpriteUp();

                if (goingright == true)
                {
                    carscript.SetSpriteUpperRight();
                    goingright = false;
                    goingup = false;
                }

                if (goingleft == true)
                {
                    carscript.SetSpriteUpperLeft();
                    goingleft = false;
                    goingup = false;
                }
            }
            else if (delta.y < -thresshold)
            {
                goingdown = true;

                carscript.SetSpriteDown();

                if (goingright == true)
                {
                    carscript.SetSpriteLowerRight();
                    goingright = false;
                    goingdown = false;
                }

                if (goingleft == true)
                {
                    carscript.SetSpriteLowerLeft();
                    goingleft = false;
                    goingdown = false;
                }
            }
            // If delta.y is close to zero, you can handle it as a special case or leave it empty.

        }
    }

    void CheckPlayerProximity()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        foreach (Collider2D col in hitColliders)
        {
            if (col.CompareTag("Player"))
            {
                Debug.Log("PlayerNearCar");
                // Player is within the detection radius
                ai.maxSpeed = 0; // Set A* Pathfinding speed to 0 (stop the car)
                return; // No need to check further if the player is found
            }
        }
    }
}