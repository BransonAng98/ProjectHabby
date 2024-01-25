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

    private Vector3 previousDirection;
    void Start()
    {
        ai = GetComponent<IAstarAI>();
        carscript = gameObject.GetComponent<CarAI>();
    }
    Vector3 PickRandomPoint(Vector3 avoidDirection)
    {
        Vector3 randomPoint;
        do
        {
            randomPoint = Random.insideUnitSphere * radius;
            randomPoint.y = 0;
            randomPoint += ai.position;
        } while (Vector3.Dot((randomPoint - ai.position).normalized, avoidDirection) > 0.8f); // Adjust the threshold as needed

        return randomPoint;
    }

    void Update()
    {
        delta = transform.position - previousPosition;
        previousPosition = transform.position;
        SetSpriteDirection();
        
        if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
        {
            Vector3 avoidDirection = -previousDirection.normalized;
            ai.destination = PickRandomPoint(avoidDirection);
            previousDirection = (ai.destination - ai.position).normalized;
            ai.SearchPath();
        }
    }

    void SetSpriteDirection()
    {
        const float threshold = 0.03f;
        const float diagonalThreshold = 0.05f; // Adjust this value for diagonal turns

        if (Mathf.Abs(delta.x) > threshold || Mathf.Abs(delta.y) > threshold)
        {
            if (Mathf.Abs(delta.x) > diagonalThreshold || Mathf.Abs(delta.y) > diagonalThreshold)
            {
                // Diagonal movement
                if (delta.x > 0)
                {
                    if (delta.y > 0)
                        carscript.SetSpriteUpperRight();
                    else if (delta.y < 0)
                        carscript.SetSpriteLowerRight();
                    else
                        carscript.SetSpriteRight();
                }
                else if (delta.x < 0)
                {
                    if (delta.y > 0)
                        carscript.SetSpriteUpperLeft();
                    else if (delta.y < 0)
                        carscript.SetSpriteLowerLeft();
                    else
                        carscript.SetSpriteLeft();
                }
                else
                {
                    // delta.x is close to zero (not moving horizontally)
                    if (delta.y > 0)
                        carscript.SetSpriteUp();
                    else if (delta.y < 0)
                        carscript.SetSpriteDown();
                    // If delta.y is close to zero, you can handle it as a special case or leave it empty.
                }
            }
            else
            {
                // Non-diagonal movement
                if (delta.x > threshold)
                    carscript.SetSpriteRight();
                else if (delta.x < -threshold)
                    carscript.SetSpriteLeft();
                else
                {
                    if (delta.y > threshold)
                        carscript.SetSpriteUp();
                    else if (delta.y < -threshold)
                        carscript.SetSpriteDown();
                    // If delta.y is close to zero, you can handle it as a special case or leave it empty.
                }
            }
        }
    }
}

