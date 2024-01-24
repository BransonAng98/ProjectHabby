using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class CarWanderingScript : MonoBehaviour
{
    public float radius = 20;
    IAstarAI ai;
    public CarAI carscript;
    [SerializeField]
    private float angle;

    Vector3 previousPosition;
    [SerializeField]
    private Vector2 delta;
    [SerializeField]
    private float scale = 10f;
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
        delta = (transform.position - previousPosition) * scale;
        previousPosition = transform.position;
        SetSpriteDirection();
        
        // Update the destination of the AI if
        // the AI is not already calculating a path and
        // the ai has reached the end of the path or it has no path at all
        if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
        {
            ai.destination = PickRandomPoint();
            Vector2 direction = (ai.destination - ai.position).normalized;
            angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            angle = NormalizeAngle(angle);
            ai.SearchPath();
        }
    }

   void SetSpriteDirection()
{
        if (ai.destination != null)
        {
            if (delta.x > 0f)
            {
                carscript.SetSpriteRight();
            }
            if (delta.x > 0f && delta.y > 0.5)
            {
                carscript.SetSpriteUpperRight();
            }
            if (delta.y > 0.5f)
            {
                carscript.SetSpriteUp();
            }
            if (delta.x <0f && delta.y < -0.5f)
            {
                carscript.SetSpriteUpperLeft();
            }
            if (delta.x < 0f)
            {
                carscript.SetSpriteLeft();
            }
            if (delta.x < 0f && delta.y < -0.5f)
            {
                carscript.SetSpriteLowerLeft();
            }
            if (delta.y < -0.5f)
            {
                carscript.SetSpriteDown();
            }
            if (delta.x > 0f && delta.y < -0.5f)
            {
                carscript.SetSpriteLowerRight();
            }
        }
}

    private float NormalizeAngle(float angle)
    {
        return (angle + 360) % 360;
    }
}

