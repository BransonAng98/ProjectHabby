using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissileScript : MonoBehaviour
{
    public Camera mainCam;
    public Transform target;
    private Rigidbody2D rb;
    public EnemyScriptableObject enemyData;
    private MissileManager missileManager;
    public GameObject explosionVFX;

    public float speed;
    public float rotateSpeed = 200f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        missileManager = GameObject.FindGameObjectWithTag("MissileManager").GetComponent<MissileManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 direction = (Vector2)target.position - rb.position;
        direction.Normalize();
        
        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.velocity = transform.up * speed;
        if(missileManager.hasEnded == true)
        {
            speed = 10f;
        }
        else if (missileManager.hasEnded == false)
        {
            speed = 2f;
        }
    }

    public void BlowUp()
    {
        Destroy(gameObject);
        Instantiate(explosionVFX, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameObject explosion = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            collision.gameObject.GetComponent<PlayerHealthScript>().TakeDamage((int)enemyData.attackDamage);
            collision.GetComponent<PlayerHandler>().DisableMovement(6);
            Destroy(gameObject);
        }
    }

}
