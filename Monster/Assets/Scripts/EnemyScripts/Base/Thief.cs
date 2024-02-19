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
    public Transform minDist;
    public Transform maxDist;

    public Vector2 velocity;
    public float maxYVelocity;
    public float distanceTravelled;
    public bool brokenFree;
    public bool isCaught;

    public GameObject pfEgg;

    //Private Variable
    private PlayerEndlessRunnerController player;

    //Serializable Variable
    [SerializeField] float distanceThreshold;
    [SerializeField] float tempHealth;
    [SerializeField] float tempSpeed;
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
    }

    void AssignStat()
    {
        tempHealth = enemyData.health;
        tempSpeed = enemyData.speed;
        tempAccel = enemyData.acceleration;
        tempMaxAccel = enemyData.maxAcceleration;
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
        distanceTravelled += velocity.y * Time.deltaTime;

        float distance = distanceTravelled - player.distanceTravelled;
        if (distance < distanceThreshold)
        {
            if(transform.position != minDist.position)
            {
                //Move the thief slowly towards the player
                Debug.Log("Moving towards the player");
                transform.position = Vector3.MoveTowards(transform.position, minDist.position, tempSpeed * Time.deltaTime);
            }

            else
            {
                player.currentState = PlayerEndlessRunnerController.PlayerState.SpecialAttack;
                entityState = ThiefState.caught;
            }
        }

        else
        {
            if(transform.position != maxDist.position)
            {
                //Slowly move towards out of the camera
                Debug.Log("Moving away from the player");
                transform.position = Vector3.MoveTowards(transform.position, maxDist.position, tempSpeed * Time.deltaTime);
            }

            else
            {
                entityState = ThiefState.run;
            }
        }
    }

    void Caught()
    {
        if(brokenFree)
        {
            if (transform.position != maxDist.position)
            {
                //Slowly move towards out of the camera
                transform.position = Vector3.MoveTowards(transform.position, maxDist.position, tempSpeed * 1.5f * Time.deltaTime);
            }

            else
            {
                entityState = ThiefState.run;
                brokenFree = false;
            }
        }

        else
        {
            return;
        }
    }

    void Released()
    {
        if (transform.position != maxDist.position)
        {
            //Slowly move towards out of the camera
            transform.position = Vector3.MoveTowards(transform.position, maxDist.position, tempSpeed /2 * Time.deltaTime);
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
        transform.position = Vector3.MoveTowards(transform.position, maxDist.position, tempSpeed / 2 * Time.deltaTime);
        Invoke("EndLevel", 6f);
    }

    void EndLevel()
    {
        SceneManager.LoadScene("Endless_MainMenu");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCaught)
        {
            DistanceBetween();
        }
        else
        {
            return;
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
