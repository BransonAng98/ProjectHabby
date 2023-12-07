using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class artidamagecolliderScript : MonoBehaviour
{
    public GameObject explosionVFX;
    public EnemyScriptableObject enemyData;
    private CapsuleCollider2D entitycollider;
    // Start is called before the first frame update
    void Start()
    {
        entitycollider = GetComponent<CapsuleCollider2D>();
        entitycollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void turnonCollider()
    {
        entitycollider.enabled = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("DamageByArti");
            GameObject explosion = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            collision.gameObject.GetComponent<PlayerHealthScript>().TakeDamage(enemyData.attackDamage);
            //Destroy(gameObject);
        }
    }
}
