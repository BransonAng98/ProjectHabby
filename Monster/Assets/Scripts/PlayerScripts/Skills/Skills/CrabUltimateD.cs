using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabUltimateD : UltimateBase
{
    private GameObject player;
    private PlayerVFXManager vfxManager;
    public Transform detectionOrigin;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        vfxManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerVFXManager>();
    }

    public override void UseDamageUltimate(float ultimateRadius, float ultimateDamage)
    {
        base.UseDamageUltimate(ultimateRadius, ultimateDamage);
        vfxManager.SpawnUltiVFX();
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(detectionOrigin.position, ultimateRadius);
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Tank"))
            {
                NewEnemyScript tank = collider.GetComponent<NewEnemyScript>();
                if (tank != null)
                {
                    tank.TakeDamage(ultimateDamage);
                }
                else { return; }
            }

            else if (collider.CompareTag("BigBuilding"))
            {
                BigBuildingEnemy bigBuilding = collider.GetComponent<BigBuildingEnemy>();
                if (bigBuilding != null)
                {
                    bigBuilding.TakeDamage(ultimateDamage);
                }
                else { return; }
            }

            else if (collider.CompareTag("Civilians"))
            {
                Civilian civilian = collider.GetComponent<Civilian>();
                if (civilian != null)
                {
                    civilian.enemyState = Civilian.EnemyState.death;
                }

                else
                {
                    Leader leader = collider.GetComponent<Leader>();
                    if(leader != null)
                    {
                        leader.enemyState = Leader.EnemyState.death;
                    }
                }
            }


            else if (collider.CompareTag("Tree"))
            {
                Trees tree = collider.GetComponent<Trees>();
                if (tree != null)
                {
                    tree.Death();
                }
                else { return; }
            }

            else if (collider.CompareTag("Car"))
            {
                CarAI car = collider.GetComponent<CarAI>();
                if (car != null)
                {
                    car.Death();
                }
                else { return; }
            }

            else if (collider.CompareTag("Solider"))
            {
                HumanSoldier soldier = collider.GetComponent<HumanSoldier>();
                if (soldier != null)
                {
                    soldier.isBurnt = true;
                    soldier.Death();
                }
                else { return; }
            }
        }
    }
}
