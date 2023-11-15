using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileManager : MonoBehaviour
{
    public GameObject missilePrefab;
    public GameObject tapText;
    public Camera mainCam;
    public Joystick joystick;
    [SerializeField] private PlayerHandler playerHandler;
    [SerializeField] private Animator anim;

    public int missileMaxAmount;
    public float touchRadius;

    public float eventDuration;
    public float spawnRadius;

    private float eventTimer;
    private GameObject guide;
    private bool isEventActive;
    private bool isLaunched;
    private bool hasSpawned;
    public bool hasEnded;
    

    // Start is called before the first frame update
    void Start()
    {
        eventTimer = 0;
        isEventActive = false;
        playerHandler = GameObject.Find("CrabPlayer").GetComponent<PlayerHandler>();
        anim = GameObject.Find("MilitaryAbilityWarning").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isEventActive)
        {

            SpawnGuide();

            Invoke("DeactiveBanner", 3f);

            anim.SetBool("Close", true);

            Invoke("ResetActivation", 15f);

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0); // Get the first touch (you can loop through all touches if needed)

                if (touch.phase == TouchPhase.Began)
                {
                    // Get the position of the touch
                    Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                    transform.position = touchPosition;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(touchPosition, touchRadius);
                    foreach (Collider2D col in colliders)
                    {
                        if (col.CompareTag("Missile"))
                        {
                            MissileScript missile = col.GetComponent<MissileScript>();
                            if (missile != null)
                            {
                                missile.BlowUp();
                            }
                            else
                            {
                                Debug.Log("nothing");
                            }
                        }
                    }

                }
            }
            eventTimer += Time.deltaTime;
            if (eventTimer >= eventDuration || playerHandler.isEnd == true)
            {
                EndEvent();
                DestroyGuide();
            }
        }
    }

    void SpawnGuide()
    {
        if (!hasSpawned)
        {
            Vector2 textPos = new Vector2(playerHandler.transform.position.x, playerHandler.transform.position.y + 8f);
            guide = Instantiate(tapText, textPos, Quaternion.identity);
            hasSpawned = true;
        }
    }

    void DestroyGuide()
    {
        if (guide != null)
        {
            Destroy(guide);
            hasSpawned = false;
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
        hasEnded = true;
    }

    public void StartEvent()
    {
        if(isLaunched == false)
        {
            joystick.gameObject.SetActive(false);
            Invoke("GetSpawnPoints" ,2f);
            isEventActive = true;
            isLaunched = true;
            hasEnded = false;
        }
       
    }

}
