using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MovementInput { get; private set; }
    public PlayerStatScriptableObject playerSO;
    private Rigidbody2D rb;
    public bool attackNow;
    public bool ultimating;
    float ultimateRadius = 10f;
    public float currentUltimateCharge;

    public Vector2 boxSize; // Adjust the size as needed.
    private float rangeX;
    private float rangeY;
    public LayerMask enemyLayer;
    public Collider2D selectedEnemy;
    bool facingLeft;
    public bool isAttacking;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rangeX = playerSO.attackRange;
        rangeY = playerSO.attackRange;
        boxSize = new Vector2(rangeX, rangeY);
    }

    private void Update()
    {
        ProcessInput();
        CheckFlip();
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, boxSize, 0f, enemyLayer);

        if (colliders.Length > 0)
        {
            Collider2D nearestEnemy = FindNearestEnemy(colliders);
            selectedEnemy = nearestEnemy;
            if(isAttacking == false)
            {
                CheckAttack(true);
                isAttacking = true;
                Invoke("ResetAttack", playerSO.attackSpeed);
            }
        }

        else { selectedEnemy = null; CheckAttack(false); }
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    private void FixedUpdate()
    {
        OnAnimationMove();
    }

    void ProcessInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        MovementInput = new Vector2(moveX, moveY).normalized;
    }

    public void CheckAttack(bool canAttack)
    {
        attackNow = canAttack;
    }

    public void StopAttackAnimation()
    {
        attackNow = false;
    }

    public void DeactivateUltimate()
    {
        ultimating = false;
        currentUltimateCharge = 0;
    }

    private Collider2D FindNearestEnemy(Collider2D[] enemies)
    {
        selectedEnemy = null;
        float nearestDistance = float.MaxValue;
        Vector2 playerPosition = transform.position;

        foreach (Collider2D enemy in enemies)
        {
            Transform enemyTransform = enemy.transform;
            Vector3 directionToEnemy = enemyTransform.position - transform.position;
            float angle = Vector2.Angle(transform.up, directionToEnemy);

            if(angle < 180f)
            {
                float distance = Vector2.Distance(playerPosition, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    selectedEnemy = enemy;
                }
            }
        }

        return selectedEnemy;
    }

    public void UseUltimate()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, ultimateRadius);
        foreach(Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                NewEnemyScript tank = collider.GetComponent<NewEnemyScript>();
                if(tank != null)
                {
                    tank.TakeDamage(playerSO.ultimateDamage);
                }
                else { return; }
            }

            else if (collider.CompareTag("BigBuilding"))
            {
                BigBuildingEnemy bigBuilding = collider.GetComponent<BigBuildingEnemy>();
                if(bigBuilding != null)
                {
                    bigBuilding.TakeDamage(playerSO.ultimateDamage);
                }
                else { return; }
            }

            else if (collider.CompareTag("Civilian"))
            {
                Civilian civilian = collider.GetComponent<Civilian>();
                if(civilian != null)
                {
                    civilian.enemyState = Civilian.EnemyState.death;
                }
                else { return; }
            }
        }
    }

    public void Attack()
    {
        if(selectedEnemy != null)
        {
            selectedEnemy.GetComponent<Targetable>().TakeDamage();
        }

        else { return; }
    }

    public void ChargeUltimate(int amount)
    {
        if(currentUltimateCharge != playerSO.maxUltimateCharge)
        {
            currentUltimateCharge += amount;
        }

        if(currentUltimateCharge >= playerSO.maxUltimateCharge)
        {
            currentUltimateCharge = playerSO.maxUltimateCharge;
        }
    }
    public void CheckFlip()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if(facingLeft != true)
            {
                transform.Rotate(0.0f, 180.0f, 0.0f);
                facingLeft = true;
            }

            else { return; }
        }

        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (facingLeft == true)
            {
                transform.Rotate(0.0f, -180.0f, 0.0f);
                facingLeft = false;
            }

            else { return; }
        }

    }

    void OnAnimationMove()
    {
        rb.velocity = new Vector2(MovementInput.x * playerSO.speed, MovementInput.y * playerSO.speed);
    }
}
