using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropVFX : MonoBehaviour
{
    //public AudioManagerScript audiomanager;
    public Sprite destroyedSprite;
    public SpriteRenderer spriteRenderer;
    private Collider2D propCollider;
    public GameObject sparkexplosionVFX;
    // Start is called before the first frame update
    void Start()
    {
        propCollider = GetComponent<BoxCollider2D>();
        //audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            spriteRenderer.sortingOrder = 2;
            //audiomanager.PlayPropSFX();
            GameObject sparkexplosion = Instantiate(sparkexplosionVFX, transform.position, Quaternion.identity);
            sparkexplosion.transform.SetParent(this.transform);
           // ObjectPooler.Instance.SpawnFromPool("SparkExplosion", transform.position, Quaternion.identity);
            spriteRenderer.sprite = destroyedSprite;
            propCollider.enabled = false;
        }
    }
}
