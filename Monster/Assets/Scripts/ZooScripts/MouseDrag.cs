using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDrag : MonoBehaviour
{
    private Vector3 ResetCamera;
    private Vector3 Origin;
    private Vector3 Difference;
    private bool Drag = false;

    public float minYLimit = 0f; // Minimum Y position limit
    public float maxYLimit = 10f; // Maximum Y position limit

    void Start()
    {
        ResetCamera = Camera.main.transform.position;
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Difference = new Vector3(0f, mousePosition.y - Camera.main.transform.position.y, 0f);

            if (!Drag)
            {
                Drag = true;
                Origin = mousePosition;
            }
        }
        else
        {
            Drag = false;
        }

        if (Drag)
        {
            float newY = Origin.y - Difference.y;

            // Clamp the Y position within the specified range
            newY = Mathf.Clamp(newY, minYLimit, maxYLimit);
            Debug.Log("Stop Screen");

            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, newY, Camera.main.transform.position.z);
        }

        if (Input.GetMouseButton(1))
        {
            Camera.main.transform.position = ResetCamera;
        }
    }
}