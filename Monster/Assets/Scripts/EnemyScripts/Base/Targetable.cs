using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour
{
    public enum EnemyType {  Building, Tank, BigBuilding, Car, Civilian, Soldier, Tree, PowerPlant, ItemBuilding, leader, }

    public EnemyType enemyType;

    private NewEnemyScript tankEnemy;

    private BigBuildingEnemy bigBEnemy;

    private BigBuildingEnemy buildingEnemy;

    private PowerPlant powerPlant;

    private ItemBuilding itemBuilding;

    private Leader leaderEnemy;

    public PlayerHandler player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        CallScript(enemyType);
    }

    private void CallScript(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Tank:
                tankEnemy = GetComponent<NewEnemyScript>();
                break;

            case EnemyType.Building:
                buildingEnemy = GetComponent<BigBuildingEnemy>();
                break;

            case EnemyType.BigBuilding:
                bigBEnemy = GetComponent<BigBuildingEnemy>();
                break;

            case EnemyType.PowerPlant:
               powerPlant = GetComponent<PowerPlant>();
                break;

            case EnemyType.ItemBuilding:
                itemBuilding = GetComponent<ItemBuilding>();
                break;

            case EnemyType.leader:
                leaderEnemy = GetComponent<Leader>();
                break;
        }
    }

    public void TakeDamage(float damage)
    {
        switch (enemyType)
        {
            case EnemyType.Building:
                buildingEnemy.TakeDamage(damage);
                break;

            case EnemyType.Tank:
                tankEnemy.TakeDamage(damage);
                break;

            case EnemyType.BigBuilding:
                bigBEnemy.TakeDamage(damage);
                break;

            case EnemyType.PowerPlant:
                powerPlant.TakeDamage(damage);
                break;

            case EnemyType.ItemBuilding:
                itemBuilding.TakeDamage(damage);
                break;
        }
    }

}
