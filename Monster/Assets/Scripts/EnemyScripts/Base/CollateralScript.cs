using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollateralScript : MonoBehaviour
{
    public Targetable targetType;

    private GameObject entity;

    // Start is called before the first frame update
    void Start()
    {
        targetType = GetComponent<Targetable>();
        entity = this.gameObject;
    }

    public void CollateralDamage(float damage)
    {
        switch (targetType.enemyType)
        {
            case Targetable.EnemyType.BigBuilding:
                BigBuildingEnemy buildingStats = entity.GetComponent<BigBuildingEnemy>();
                buildingStats.tempHealth -= damage;
                if (buildingStats.tempHealth <= 0)
                {
                    buildingStats.Death();
                }
                break;

            case Targetable.EnemyType.Tank:
                NewEnemyScript tankStats = entity.GetComponent<NewEnemyScript>();
                tankStats.tempHealth -= damage;
                if (tankStats.tempHealth <= 0)
                {
                    tankStats.Death();
                }
                break;

            case Targetable.EnemyType.Civilian:
                Civilian civilianStat = entity.GetComponent<Civilian>();
                civilianStat.enemyState = Civilian.EnemyState.death;
                break;

            case Targetable.EnemyType.leader:
                Leader leaderStat = entity.GetComponent<Leader>();
                leaderStat.enemyState = Leader.EnemyState.death;
                break;

            case Targetable.EnemyType.Soldier:
                HumanSoldier soldierStat = entity.GetComponent<HumanSoldier>();
                soldierStat.isBurnt = true;
                soldierStat.Death();
                break;

            case Targetable.EnemyType.Car:
                CarAI carStat = entity.GetComponent<CarAI>();
                carStat.Death();
                break;

            case Targetable.EnemyType.Tree:
                Trees treeStat = entity.GetComponent<Trees>();
                treeStat.Death();
                break;
        }
    }
}
