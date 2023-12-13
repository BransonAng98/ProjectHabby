using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMissileScript : MonoBehaviour
{
    public float missilespeed = 20f;     // Speed at which the missile moves
    public int damageAmount;    // Amount of damage the missile does to the player
    
    public float destroyTime; // Set the default destroy time in seconds
    private float currentTime = 0f;
    public GameObject explosionVFX;
    public Transform explosionPos;
    //public Transform jetPos;

    private Transform missileObj;
    public bool isLeft;

    private void Start()
    {
        missileObj = GetComponent<Transform>();
       
        CheckAndFire();
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        
        if (currentTime >= destroyTime)
        {
            //SpawnExplosion();
            Destroy(gameObject); // Destroy the GameObject
        }
    }

    public void CheckAndFire()
    {

        if (isLeft == true)
        {
            missileObj.rotation = Quaternion.Euler(0, 180, 0);
            GetComponent<Rigidbody2D>().velocity = new Vector2(-missilespeed, 0f);
        }
        else
        {
            missileObj.rotation = Quaternion.Euler(0, 0, 0);
            GetComponent<Rigidbody2D>().velocity = new Vector2(missilespeed, 0f);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
     
        if (collision.CompareTag("Player"))
        {
            SpawnExplosion();
            collision.GetComponent<PlayerHealthScript>().TakeDamage(damageAmount);
            collision.GetComponent<PlayerHandler>().DisableMovement(6);
            Destroy(gameObject);
        }

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
        Instantiate(explosionVFX, explosionPos.position, Quaternion.identity);
    }


}
