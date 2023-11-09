using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trees : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite destroyedSprite;
    private ObjectShakeScript shakeScript;
    private bool isShake;
    private Collider2D entityCollider;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        entityCollider = GetComponent<Collider2D>();
        shakeScript = GetComponent<ObjectShakeScript>();
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
}
