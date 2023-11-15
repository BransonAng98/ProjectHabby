using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabUltimateD : UltimateBase
{
    public GameObject ultimateVFX;
    private GameObject player;
    public Transform detectionOrigin;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void UseDamageUltimate(float ultimateRadius, float ultimateDamage)
    {
        base.UseDamageUltimate(ultimateRadius, ultimateDamage);
        Vector2 ultiPos = new Vector2(player.transform.position.x, player.transform.position.y - 0.8f);
        GameObject ultiVFX = Instantiate(ultimateVFX, ultiPos, Quaternion.identity);
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

            else if (collider.CompareTag("Civilian"))
            {
                Civilian civilian = collider.GetComponent<Civilian>();
                if (civilian != null)
                {
                    civilian.enemyState = Civilian.EnemyState.death;
                }
                else { return; }
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
