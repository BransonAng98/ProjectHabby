using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Thief : MonoBehaviour
{
    public enum ThiefState
    {
        run,
        caught,
        released,
        death,
    }

    //Public Variable
    public ThiefSO enemyData;
    public ThiefState entityState;
   
    public List<Transform> movePos = new List<Transform>();

    public Vector2 velocity;
    public float maxYVelocity;
    public float distanceTravelled;
    public bool brokenFree;
    public bool isCaught;
    public GameObject tapText;
    public GameObject winScreen;

    public GameObject pfEgg;

    public int posID;

    //Private Variable
    private PlayerEndlessRunnerController player;

    //Serializable Variable
    [SerializeField] float distanceThreshold;
    [SerializeField] float tempHealth;
    [SerializeField] float tempSpeed;
    [SerializeField] float tempDeathSpeed;
    [SerializeField] float tempAccel;
    [SerializeField] float tempMaxAccel;

    // Start is called before the first frame update
    void Start()
    {
        //External Check
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEndlessRunnerController>();

        //Internal Check

        //Assigning Stat
        AssignStat();
        winScreen.gameObject.SetActive(false);
        posID = 2;
    }

    void AssignStat()
    {
        tempHealth = enemyData.health;
        tempSpeed = enemyData.speed;
        tempAccel = enemyData.acceleration;
        tempMaxAccel = enemyData.maxAcceleration;
        tempDeathSpeed = tempSpeed / 5f;
    }

    void Run()
    {
        //How fast the thief is moving
        float velocityRatio = velocity.y / maxYVelocity;
        tempAccel = tempMaxAccel * (1 - velocityRatio);
        velocity.y += tempAccel * Time.deltaTime;
        if (velocity.y >= maxYVelocity)
        {
            velocity.y = maxYVelocity;
        }
    }

    void DistanceBetween()
    {
        if (!isCaught || !brokenFree)
        {
            distanceTravelled += velocity.y * Time.deltaTime;
        }

        else
        {
            return;
        }
    }

    public void ModifyID(int newValue)
    {
        posID = newValue;
    }

    private void UpdatePos()
    {
        switch (posID)
        {
            case 0:
                if(transform.position != movePos[0].position)
                {
                    transform.position = Vector2.MoveTowards(transform.position, movePos[0].position, tempSpeed * Time.deltaTime);
                }

                else
                {
                    entityState = ThiefState.caught;
                    player.currentState = PlayerEndlessRunnerController.PlayerState.SpecialAttack;
                }
                break;

            case 1:
                if(transform.position != movePos[1].position)
                {
                    transform.position = Vector2.MoveTowards(transform.position, movePos[1].position, tempSpeed * Time.deltaTime);
                }

                else
                {
                    return;
                }
                break;

            case 2:
                if (transform.position != movePos[2].position)
                {
                    transform.position = Vector2.MoveTowards(transform.position, movePos[2].position, tempSpeed * Time.deltaTime);
                }

                else
                {
                    return;
                }
                break;

            case 3:
                if (transform.position != movePos[3].position)
                {
                    transform.position = Vector2.MoveTowards(transform.position, movePos[3].position, tempSpeed * Time.deltaTime);
                }

                else
                {
                    return;
                }
                break;

            case 4:
                if (transform.position != movePos[4].position)
                {
                    transform.position = Vector2.MoveTowards(transform.position, movePos[4].position, tempSpeed * Time.deltaTime);
                }

                else
                {
                    return;
                }
                break;

            case 5:
                player.currentState = PlayerEndlessRunnerController.PlayerState.death;
                break;
        }
    }

    void Caught()
    {
        if(brokenFree)
        {
            if (transform.position != movePos[2].position)
            {
                Debug.Log("Moving to pos 2");
                //Slowly move towards out of the camera
                transform.position = Vector3.MoveTowards(transform.position, movePos[2].position, tempSpeed * 1.5f * Time.deltaTime);
            }

            else
            {
                Debug.Log("Running");
                entityState = ThiefState.run;
                brokenFree = false;
            }
        }

        else
        {
            if(transform.position == movePos[0].position)
            {
                tapText.SetActive(true);
            }
        }
    }

    void Released()
    {
        if (transform.position != movePos[2].position)
        {
            //Slowly move towards out of the camera
            transform.position = Vector3.MoveTowards(transform.position, movePos[2].position, tempSpeed /2 * Time.deltaTime);
        }

        else
        {
            return;
        }
    }

    public void TakeDamage(float damage)
    {
        tempHealth -= damage;

        if(tempHealth <= 0)
        {
            entityState = ThiefState.death;
        }

        else
        {
            //Trigger damage VFX here
            return;
        }
    }

    void Death()
    {
        tapText.SetActive(false);
        transform.position = Vector3.MoveTowards(transform.position, movePos[4].position, tempDeathSpeed * Time.deltaTime);
        player.gameEnd = true;
        //player.currentState = PlayerEndlessRunnerController.PlayerState.victory;
        Invoke("EndLevel", 6f);
    }

    void EndLevel()
    {
        winScreen.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        DistanceBetween();
        if(entityState != ThiefState.death)
        {
            UpdatePos();
        }

        switch (entityState)
        {
            case ThiefState.run:
                Run();
                break;

            case ThiefState.caught:
                Caught();
                break;

            case ThiefState.released:
                Released();
                break;

            case ThiefState.death:
                Death();
                break;
        }
    }
}
