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
    Vector3 lastDirection;

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
    Vector3 lastPickedDirection;

    void Update()
    {
        if (!carscript.hasDied)
        {
            delta = transform.position - previousPosition;
            previousPosition = transform.position;
            SetSpriteDirection();

            if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
            {
                Vector2 newDirection;

                // Ensure that the new direction is not opposite to the last picked direction
                do
                {
                    newDirection = (PickRandomPoint() - ai.position).normalized;
                } while (Vector2.Dot(newDirection, lastPickedDirection) < -0.5f);

                ai.destination = ai.position + (Vector3)newDirection * radius;
                lastDirection = newDirection;

                ai.SearchPath();
            }
        }
        else
        {
            return;
        }
    }


    void SetSpriteDirection()
    {
        const float thresshold = 0.01f;
        if (delta.x > thresshold)//going right
        {
            goingright = true;
            goingleft = false;

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
            goingright = false;

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
                goingdown = false;
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
                goingleft = false;

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
}
