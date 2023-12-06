using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryBullet : MonoBehaviour
{
    public EnemyScriptableObject enemyData;
    public GameObject explosionVFX;
    private Collider2D entityCollider;
    public Transform circlePos;
    [SerializeField]private float bulletSpeed;
    public AudioSource artiAudioSource;
    public AudioClip[] artiSFX;
    public float damageradius;
    public bool reachedDestination;
    private void Start()
    {
        entityCollider = GetComponent<Collider2D>();
        circlePos = GameObject.FindGameObjectWithTag("IndicatorCircle").GetComponent<Transform>();
        float randomOffsetX = Random.Range(-3f, 3f); // Adjust the range as needed
        float randomOffsetY = Random.Range(-3f, 3f); // Adjust the range as needed

        circlePos.position += new Vector3(randomOffsetX, randomOffsetY, 0f);
        bulletSpeed = 10f;
        damageradius = 5;
        entityCollider.enabled = false;
        
    }
    

    private void Update()
    {
        Vector3 moveDirection = (circlePos.position - transform.position).normalized;
        transform.position += moveDirection * bulletSpeed * Time.deltaTime;
        float distanceToCirclePos = Vector3.Distance(transform.position, circlePos.position);
        float enableColliderDistance = 0.3f;
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x);
        float rotationAngle = Mathf.Rad2Deg * angle - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);

        if (distanceToCirclePos <= enableColliderDistance)
        {
            entityCollider.enabled = true;
        }

        if (circlePos == null)
        {
            Destroy(gameObject);
        }
    }


    public void BlowUp()
    {
        Debug.Log("Blow Up");
        PlayExplodeSFX();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayExplodeSFX();
        if (collision.CompareTag("IndicatorCircle"))
        {
            Debug.Log("ReachedDestination");
            reachedDestination = true;
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
          
        }

        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerHealthScript>().TakeDamage(enemyData.attackDamage);
            Debug.Log("ArtilleryDamage");
           
        }
    }



    public void PlayExplodeSFX()
    {
        AudioClip soundtoPlay = artiSFX[Random.Range(0, artiSFX.Length)];
        artiAudioSource.PlayOneShot(soundtoPlay);
    }  
}