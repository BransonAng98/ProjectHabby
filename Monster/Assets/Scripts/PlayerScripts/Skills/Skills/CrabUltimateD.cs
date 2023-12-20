using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabUltimateD : UltimateBase
{
    private GameObject player;
    public PlayerStatScriptableObject playerData;
    private PlayerVFXManager vfxManager;
    public Transform detectionOrigin;

    [SerializeField] float ultRadius;
    [SerializeField] float ultDamage;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        vfxManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerVFXManager>();

        AssignVariables();
    }

    void AssignVariables()
    {
        switch (playerData.ultimateLevel)
        {
            case 1:
                ultRadius = 10f;
                ultDamage = 10f;
                break;
                
            case 2:
                ultRadius = 15f;
                ultDamage = 20f;
                break;
                
            case 3:
                ultRadius = 20f;
                ultDamage = 30f;
                break;

        }
    }

    public override void UseDamageUltimate()
    {
        base.UseDamageUltimate();
        vfxManager.SpawnUltiVFX();
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(detectionOrigin.position, ultRadius);
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Tank"))
            {
                NewEnemyScript tank = collider.GetComponent<NewEnemyScript>();
                if (tank != null)
                {
                    tank.TakeDamage(ultDamage);
                }
                else { return; }
            }

            else if (collider.CompareTag("BigBuilding"))
            {
                BigBuildingEnemy bigBuilding = collider.GetComponent<BigBuildingEnemy>();
                if (bigBuilding != null)
                {
                    bigBuilding.TakeDamage(ultDamage);
                }
                else { return; }
            }

            else if (collider.CompareTag("Civilian"))
            {
                Civilian civilian = collider.GetComponent<Civilian>();
                if (civilian != null)
                {
                    civilian.enemyState = Civilian.EnemyState.death;
                    civilian.causeOfDeath = "Ulted by crab";
                }
            }


            else if (collider.CompareTag("Leader"))
            {
                Leader leader = collider.GetComponent<Leader>();
                if (leader != null)
                {
                    leader.enemyState = Leader.EnemyState.death;
                    leader.causeOfDeath = "Ulted by crab";
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
