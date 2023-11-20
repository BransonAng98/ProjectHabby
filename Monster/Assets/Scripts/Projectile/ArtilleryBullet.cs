using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryBullet : MonoBehaviour
{
    public EnemyScriptableObject enemyData;
    public GameObject explosionVFX;
    private CircularIndicator storedData;
    private Collider2D entityCollider;

    public AudioSource artiAudioSource;
    public AudioClip[] artiSFX;

    private void Start()
    {
        entityCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        CheckTrigger();

    }

    void CheckTrigger()
    {
        if (storedData.isInRange == true)
        {
            entityCollider.enabled = true;
        }

        else
        {
            entityCollider.enabled = false;
        }
    }

    public void GetData(GameObject data)
    {
        if (data)
        {
            storedData = data.GetComponent<CircularIndicator>();
        }

        else
        {

        }
    }

    public void BlowUp()
    {
        Debug.Log("Blow Up");
        //PlayExplodeSFX();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayExplodeSFX();
        if (collision.gameObject != null && collision.gameObject.CompareTag("Player"))
        {
            if (gameObject != null)
            {
                GameObject bomb = Instantiate(explosionVFX, collision.transform.position, Quaternion.identity);

                PlayerHealthScript playerHealth = collision.gameObject.GetComponent<PlayerHealthScript>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(enemyData.attackDamage);
                    //Destroy(gameObject);
                }

                
            }
        }
    }

    public void PlayExplodeSFX()
    {
        AudioClip soundtoPlay = artiSFX[Random.Range(1, artiSFX.Length)];
        artiAudioSource.PlayOneShot(soundtoPlay);
    }  
}