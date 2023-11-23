using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trees : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite destroyedSprite;
    public GameObject hitVFX;

    private ObjectShakeScript shakeScript;
    private bool isShake;
    private Collider2D entityCollider;

    public AudioManagerScript audiomanager;

    public AudioSource treeaudioSource;
    public AudioClip[] treeSFX;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        entityCollider = GetComponent<Collider2D>();
        shakeScript = GetComponent<ObjectShakeScript>();
        audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Death()
    {
        if(entityCollider == null)
        {
            return;
        }

        else
        {
            SpawnVFX();
            audiomanager.PlayTreeSFX();
            entityCollider.enabled = false;
            if (isShake != true)
            {
                shakeScript.StartShake();
                isShake = true;
            }
            spriteRenderer.sprite = destroyedSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerLeg"))
        {
          
            Death();
        }
    }

    void SpawnVFX()
    {
        Instantiate(hitVFX, transform.position, Quaternion.identity);
    }

    /*public void PlaySFX()
    {
        AudioClip deathSFX = treeSFX[(Random.Range(0, treeSFX.Length))];
        treeaudioSource.PlayOneShot(deathSFX);
    }*/
}
