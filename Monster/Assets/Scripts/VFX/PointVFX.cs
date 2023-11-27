using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointVFX : MonoBehaviour
{
    public float initialMoveSpeed;
    public float decelerationRate;

    private float currentMoveSpeed;

    void Start()
    {
        currentMoveSpeed = initialMoveSpeed;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = currentPosition + Vector3.up * currentMoveSpeed * Time.deltaTime;
        transform.position = newPosition;

        currentMoveSpeed -= decelerationRate * Time.deltaTime;
        if (currentMoveSpeed <= 0f)
        {
            DestroyObj();
        }
    }

    public void DestroyObj()
    {
        Destroy(gameObject);
    }
}
