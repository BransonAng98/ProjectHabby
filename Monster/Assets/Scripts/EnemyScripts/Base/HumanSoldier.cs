using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSoldier : MonoBehaviour
{
    public enum EnemyState { move, attack, death }

    public EnemyState enemyState;
    public EnemyScriptableObject enemyData;
    private Collider2D entityCollider;
    public Transform player;
    public Animator anim;
    private SpriteRenderer spriteRenderer;
    private float lastPosX;
    private float attackCDLeft;
    public bool isBurnt;

    private PlayerHandler inputHandler;

    private bool lootDropped;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        entityCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        
        lastPosX = transform.position.x;
        enemyState = EnemyState.move;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerLeg")
        {
            entityCollider.enabled = false;
            enemyState = EnemyState.death;
        }
    }

    void Move(Vector2 dir)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > enemyData.attackRange)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            transform.Translate(direction * enemyData.speed * Time.deltaTime);
        }

        else
        {
            entityCollider.enabled = false; 
        }
    }

    void CheckState(float dis)
    {
        if (enemyState != EnemyState.death)
        {
            if (dis <= enemyData.attackRange)
            {
                // Attack the player
                enemyState = EnemyState.attack;
            }
            else
            {
                // Move towards the player
                enemyState = EnemyState.move;
            }
        }

        else return;
    }

    void Attack()
    {
        attackCDLeft -= Time.deltaTime;
        if (attackCDLeft <= 0f)
        {
            attackCDLeft = enemyData.attackSpeed;
            player.gameObject.GetComponent<PlayerHealthScript>().TakeDamage((int)enemyData.attackDamage);
        }
    }

    public void Death()
    {
        if (isBurnt)
        {
            anim.SetBool("bDeath", true);
        }
        else
        {
            anim.SetBool("nDeath", true);
        }

        Destroy(gameObject, 2f);
    }

    void TriggerLoot()
    {
        inputHandler.ChargeUltimate(1);
        lootDropped = true;
    }

    void FlipSprite()
    {
        // Check if the sprite's horizontal position has changed since the last frame
        float currentPositionX = transform.position.x;
        if (currentPositionX > lastPosX)
        {
            // Moving right, so flip the sprite to face right
            spriteRenderer.flipX = true;
        }
        else if (currentPositionX < lastPosX)
        {
            // Moving left, so flip the sprite to face left
            spriteRenderer.flipX = false;
        }

        // Update the last position for the next frame
        lastPosX = currentPositionX;
    }

    // Update is called once per frame
    void Update()
    {
        FlipSprite();
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        CheckState(distanceToPlayer);

        switch (enemyState)
        {
            case EnemyState.death:
                if (lootDropped != true)
                {
                    TriggerLoot();
                }
                Death();
                break;

            case EnemyState.move:
                anim.SetBool("move", true);
                Move(player.position);
                break;

            case EnemyState.attack:
                anim.SetBool("attack", true);
                Attack();
                break;
        }
    }
}
