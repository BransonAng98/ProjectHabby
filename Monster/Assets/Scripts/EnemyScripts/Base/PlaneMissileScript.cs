using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMissileScript : MonoBehaviour
{
    public float missilespeed = 20f;     // Speed at which the missile moves
    public int damageAmount = 10;    // Amount of damage the missile does to the player
    public EnemyScriptableObject enemyData;
    public float destroyTime; // Set the default destroy time in seconds
    private float currentTime = 0f;
    public GameObject explosionVFX;
    //public Transform jetPos;
    private SpriteRenderer spriteRenderer;

    public bool isLeft;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Set initial velocity to move the missile forward;
        CheckAndFire();
    }

    private void Update()
    {
        currentTime += Time.deltaTime;

        // Check if the current time exceeds the specified destroy time
        if (currentTime >= destroyTime)
        {
            SpawnExplosion();
            Destroy(gameObject); // Destroy the GameObject
        }
    }

    public void CheckAndFire()
    {

        if (isLeft == true)
        {
            spriteRenderer.flipX = true;
            GetComponent<Rigidbody2D>().velocity = new Vector2(-missilespeed, 0f);
        }
        else
        {
            spriteRenderer.flipX = false;
            GetComponent<Rigidbody2D>().velocity = new Vector2(missilespeed, 0f);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the bomb collided with the player
        if (collision.CompareTag("Player"))
        {
            SpawnExplosion();
            collision.GetComponent<PlayerHealthScript>().TakeDamage(enemyData.attackDamage);

            Destroy(gameObject);
        }
        //else if (collision.CompareTag("BigBuilding"))
        //{
        //    // Check if the collider has a BigBuildingEnemy component
        //    BigBuildingEnemy bigBuilding = collision.GetComponent<BigBuildingEnemy>();

        //    if (bigBuilding != null)
        //    {
        //        SpawnExplosion();
        //        bigBuilding.TakeDamage(enemyData.attackDamage);
        //    }
        //    else
        //    {
        //        Debug.LogWarning("BigBuildingEnemy component not found on the collided object.");
        //    }

        //    Destroy(gameObject);
        //}
    }

    void BlowUp()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 3f);
        foreach (Collider2D collider in hitColliders)
        {
            CollateralScript collateralTrigger = collider.GetComponent<CollateralScript>();
            if (collateralTrigger != null)
            {
                collateralTrigger.CollateralDamage(100f);
            }
        }
    }

    void SpawnExplosion()
    {
        GameObject bomb = Instantiate(explosionVFX, transform.position, Quaternion.identity);
    }

}
