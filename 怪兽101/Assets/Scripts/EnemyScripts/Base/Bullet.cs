using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector3 shootDir;
    public EnemyScriptableObject enemyData;
    public GameObject explosionVFX;
    public void SetUp(Vector3 shootDir)
    {
        this.shootDir = shootDir;
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        float moveSpeed = 20f;
        transform.position += shootDir * moveSpeed * Time.deltaTime; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            GameObject explosion = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            collision.GetComponent<PlayerHealthScript>().TakeDamage(enemyData.attackDamage);
            Destroy(gameObject);
        }
    }
}
