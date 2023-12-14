using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryBullet : MonoBehaviour
{
    public EnemyScriptableObject enemyData;
    public GameObject explosionVFX;
    private CircularIndicator storedData;
    private Collider2D entityCollider;

    private void Start()
    {
        entityCollider = GetComponent<Collider2D>();
        
    }
    

    private void Update()
    {
       
    }



    public void GetData(GameObject data)
    {
        if (data)
        {
            storedData = data.GetComponent<CircularIndicator>();
        }
    }

    public void BlowUp()
    {
        Debug.Log("Blow Up");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 6f);
        foreach (Collider2D collider in hitColliders)
        {
            CollateralScript collateralTrigger = collider.GetComponent<CollateralScript>();
            if (collateralTrigger != null)
            {
                collateralTrigger.CollateralDamage(100f);
            }
        }
    }
  
}