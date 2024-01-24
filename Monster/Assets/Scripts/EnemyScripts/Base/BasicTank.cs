using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTank : MonoBehaviour
{
    public enum TankState
    {
        move,
        attack,
        reload,
        death,
    }

    //Public variables
    public EnemyScriptableObject enemyData;
    public TankState entityState;
    public Transform playerPos;
    public GameObject pfTankBullet;

    //Seriablizable variables
    [SerializeField] bool hasFired;
    
    [SerializeField] float tempHealth;
    [SerializeField] float tempSpeed;
    [SerializeField] float tempDamage;
    [SerializeField] float tempAtkSpd;
    [SerializeField] float tempRange;

    //private variables
    private Collider2D entityCollider;

    // Start is called before the first frame update
    void Start()
    {
        //External Checks
        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        //Internal Checks
        entityCollider.GetComponent<Collider2D>();

        //Setting Variables
        AssignStat();
    }

    void AssignStat()
    {
        tempHealth = enemyData.health;
        tempSpeed = enemyData.speed;
        tempDamage = enemyData.attackDamage;
        tempAtkSpd = enemyData.attackSpeed;
        tempRange = enemyData.attackRange;
    }

    void Move()
    {
        // Use Delvin's Astar pathfinding
    }

    void Attack()
    {
        if (!hasFired)
        {
            hasFired = true;
            Vector2 dirToPlayer = playerPos.position - transform.position;
            dirToPlayer.Normalize();
            Quaternion rotation = Quaternion.LookRotation(dirToPlayer);

            // Spawn a bullet at the enemy's position and with the calculated rotation
            SpawnBullet(rotation, dirToPlayer);
        }
    }

    void SpawnBullet(Quaternion rotation, Vector2 moveDirection)
    {
        GameObject bullet = Instantiate(pfTankBullet, transform.position, rotation);

        //Move the bullet towards the last known player position
        bullet.transform.position = Vector2.MoveTowards(bullet.transform.position, moveDirection, 3f * Time.deltaTime);
    }

    void Reload()
    {
        if(tempAtkSpd != 0)
        {
            tempAtkSpd -= Time.deltaTime;
        }

        else
        {
            hasFired = false;
            tempAtkSpd = enemyData.attackSpeed;

            if (PlayerInRange())
            {
                entityState = TankState.attack;
            }

            else
            {
                entityState = TankState.move;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        tempHealth -= damage;

        if(tempHealth >= 0)
        {
            entityState = TankState.death;
        }
    }

    void Death()
    {
        entityCollider.enabled = false;
        Destroy(transform.parent.gameObject);
    }

    bool PlayerInRange()
    {
        if (playerPos != null)
        {
            // Calculate the distance between the enemy and the player
            float distanceToPlayer = Vector2.Distance(transform.position, playerPos.position);

            // Check if the player is within the detection range
            return distanceToPlayer <= tempRange;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (entityState)
        {
            case TankState.move:
                Move();
                break;
                
            case TankState.attack:
                Attack();
                break;
                
            case TankState.reload:
                Reload();
                break;
                
            case TankState.death:
                Death();
                break;
        }
    }
}
