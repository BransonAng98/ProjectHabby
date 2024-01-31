using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArmyVehicle : MonoBehaviour
{
    public enum SpawnVehicleState
    {
        move,
        deploy,
        retreat,
        death,
    }

    //Public Variables
    public SpawnVehicleState entityState;
    public EnemyScriptableObject enemyData;
    public GameObject spawnedSoldiers;
    public float spawnDistance = 10f;
    public float despawnDistance = 20f;
    public float transitTiming;

    //Private Variables
    private Transform playerTransform;

    //Serializable Variables
    [SerializeField] float tempHealth;
    [SerializeField] float tempSpeed;
    [SerializeField] float tempAttack;

    // Start is called before the first frame update
    void Start()
    {
        //External Check
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        //Internal Check

        //Assign Variables
        AssignStat();
    }

    void AssignStat()
    {
        tempHealth = enemyData.health;
        tempSpeed = enemyData.speed;
        tempAttack = enemyData.attackDamage;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BigBuilding")
        {
            BigBuildingEnemy collidedBuilding = collision.gameObject.GetComponent<BigBuildingEnemy>();
            if (collidedBuilding != null)
            {
                collidedBuilding.TakeDamage(tempAttack);
            }
        }


        if (collision.gameObject.tag == "PlayerLeg")
        {
            entityState = SpawnVehicleState.death;
        }


        if (collision.gameObject.tag == "Leader")
        {
            Leader leader = collision.gameObject.GetComponent<Leader>();
            if (leader != null)
            {
                leader.enemyState = Leader.EnemyState.death;
            }
        }

        if (collision.gameObject.tag == "Civilian")
        {
            Civilian civilian = collision.gameObject.GetComponent<Civilian>();
            if (civilian != null)
            {
                civilian.enemyState = Civilian.EnemyState.death;
            }
        }

        if (collision.gameObject.tag == "Tree")
        {
            Trees tree = collision.gameObject.GetComponent<Trees>();
            if (tree != null)
            {
                tree.Death();
            }
        }
    }

    void Move()
    {
        // Move towards the player
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, tempSpeed * Time.deltaTime);

        // Check distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= spawnDistance)
        {
            // Spawn enemies when close to the player
            entityState = SpawnVehicleState.deploy;
        }
    }

    void Deploy()
    {
        //Spawn enemies here

        Invoke("TransitToRetreat", transitTiming);
    }

    void TransitToRetreat()
    {
        //Switch to retreat state
        entityState = SpawnVehicleState.retreat;
    }

    void Retreat()
    {
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, -tempSpeed * 2f * Time.deltaTime);

        if (IsVisibleFromCamera())
        {
            // Despawn when out of camera view
            Despawn();
        }
    }

    void Despawn()
    {
        // Destroy the enemy when out of camera view
        if (!IsVisibleFromCamera())
        {
            Destroy(gameObject);
        }
    }

    void Death()
    {
        Destroy(transform.parent.gameObject);
    }

    bool IsVisibleFromCamera()
    {
        // Check if the enemy is visible from the camera
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        return GeometryUtility.TestPlanesAABB(planes, GetComponent<Renderer>().bounds);
    }

    // Update is called once per frame
    void Update()
    {
        switch (entityState)
        {
            case SpawnVehicleState.move:
                Move();
                break;

            case SpawnVehicleState.deploy:
                Deploy();
                break;

            case SpawnVehicleState.retreat:
                Retreat();
                break;

            case SpawnVehicleState.death:
                Death();
                break;
        }
    }
}
