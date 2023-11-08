using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    [SerializeField] private bool canInteract;
    [SerializeField] private float interactTime;
    public float timeThreshold;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Color spriteColor = spriteRenderer.material.color;
        spriteColor.a = 0.5f;
        spriteRenderer.material.color = spriteColor;
    }

    // Update is called once per frame
    void Update()
    {
        if(interactTime >= timeThreshold)
        {
            interactTime += Time.deltaTime;
        }
        else
        {
            interactTime = timeThreshold;
            canInteract = true;
            Color spriteColor = spriteRenderer.material.color;
            spriteColor.a = 1f;
            spriteRenderer.material.color = spriteColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (canInteract)
            {
                PlayerHealthScript recoverHealth = collision.GetComponent<PlayerHealthScript>();
                if(recoverHealth != null)
                {
                    recoverHealth.RecoverHealth(30);
                    Destroy(gameObject);
                }
            }
        }
    }
}
