using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thief : MonoBehaviour
{
    public enum ThiefState
    {
        run,
        caught,
        released,
    }

    //Public Variable
    public EnemyScriptableObject enemyData;
    public ThiefState entityState;
    public Transform minDist;
    public Transform maxDist;

    public Vector2 velocity;
    public float maxYVelocity;
    public float maxAcceleration;
    public float acceleration;
    public float distanceTravelled;
    public bool brokenFree;

    public GameObject pfEgg;

    //Private Variable
    private PlayerEndlessRunnerController player;

    //Serializable Variable
    [SerializeField] float distanceThreshold;
    [SerializeField] float tempHealth;
    [SerializeField] float tempSpeed;

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
    }

    void Run()
    {
        //How fast the thief is moving
        float velocityRatio = velocity.y / maxYVelocity;
        acceleration = maxAcceleration * (1 - velocityRatio);
        velocity.y += acceleration * Time.deltaTime;
        if (velocity.y >= maxYVelocity)
        {
            velocity.y = maxYVelocity;
        }
    }

    void DistanceBetween()
    {
        distanceTravelled += velocity.y * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < distanceThreshold)
        {
            if(transform.position != minDist.position)
            {
                //Move the thief slowly towards the player
                Vector3 targetPosition = Vector3.Lerp(transform.position, minDist.position, tempSpeed / 2 * Time.deltaTime);
            }

            else
            {
                entityState = ThiefState.caught;
            }
        }

        else
        {
            if(transform.position != maxDist.position)
            {
                //Slowly move towards out of the camera
                transform.position = Vector3.MoveTowards(transform.position, maxDist.position, tempSpeed * Time.deltaTime);
            }

            else
            {
                return;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        tempHealth -= damage;
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

            if (tempHealth <= 0)
            {
                //Spawn egg using fake height script
                entityState = ThiefState.released;
            }
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
            //End the level
        }
    }

    // Update is called once per frame
    void Update()
    {
        DistanceBetween();

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
        }
    }
}