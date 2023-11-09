using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileManager : MonoBehaviour
{
    public GameObject missilePrefab;
    public Camera mainCam;
    public Joystick joystick;
    public LayerMask missileMask;
    
    public int missileMaxAmount;
    public float touchRadius;

    public float eventDuration;
    public float timeScaleDuringEvent = 0.5f;

    public float spawnRadius;

    private float eventTimer;
    private bool isEventActive;
    private bool isLaunched;



    // Start is called before the first frame update
    void Start()
    {
        eventTimer = 0;
        isEventActive = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isEventActive)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0); // Get the first touch (you can loop through all touches if needed)

                if (touch.phase == TouchPhase.Began)
                {

                    // Get the position of the touch
                    Vector3 touchPosition = touch.position;

                    // Convert the touch position to world space if needed (e.g., for 3D objects)
                    Vector3 worldTouchPosition = mainCam.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, 10));


                    Collider2D[] colliders = Physics2D.OverlapCircleAll(touchPosition, touchRadius, missileMask);

                    if (colliders.Length > 0)
                    {
                        foreach (Collider2D col in colliders)
                        {
                            MissileScript missile = col.GetComponent<MissileScript>();
                            missile.BlowUp();
                        }
                    }

                }
            }
            eventTimer += Time.deltaTime;
            if (eventTimer >= eventDuration)
            {
                EndEvent();
            }
        }
    }

    public void SpawnMissiles(Vector3 position)
    {
        int missileSpawnRange = Random.Range(1,missileMaxAmount);

        for (int i = 0; i < missileSpawnRange; i++)
        {
            Vector3 randomDirection = Random.insideUnitCircle.normalized;
            Vector3 spawnPos = position + (position - Camera.main.transform.position).normalized + randomDirection *spawnRadius;

            Instantiate(missilePrefab, spawnPos, Quaternion.identity);
        }
    }
    private void GetSpawnPoints()
    {
        mainCam = Camera.main; // Get the main camera
        if (mainCam != null)
        {
            // Calculate the corners in world space
            Vector3 bottomLeft = mainCam.ViewportToWorldPoint(new Vector3(0, 0, mainCam.nearClipPlane));
            Vector3 topLeft = mainCam.ViewportToWorldPoint(new Vector3(0, 1, mainCam.nearClipPlane));
            Vector3 bottomRight = mainCam.ViewportToWorldPoint(new Vector3(1, 0, mainCam.nearClipPlane));
            Vector3 topRight = mainCam.ViewportToWorldPoint(new Vector3(1, 1, mainCam.nearClipPlane));

            SpawnMissiles(bottomLeft);
            SpawnMissiles(topLeft);
            SpawnMissiles(bottomRight);
            SpawnMissiles(topRight);

        }

    }

    private void EndEvent()
    {
        joystick.gameObject.SetActive(true);
        eventTimer = 0;
        isEventActive = false;
        isLaunched = false;
    }

    public void StartEvent()
    {
        if(isLaunched == false)
        {
            joystick.gameObject.SetActive(false);
            GetSpawnPoints();
            isEventActive = true;
            isLaunched = true;
        }
       
    }

}
