using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockPickUpItem : MonoBehaviour
{
    [SerializeField] private ClockSystem clockSystem;
    [SerializeField] private bool canInteract;
    [SerializeField] private float interactTime;
    public float timeThreshold;
    private SpriteRenderer spriteRenderer;
    private Collider2D collider;

    // Start is called before the first frame update
    void Start()
    {
        clockSystem = GameObject.Find("Timer").GetComponent<ClockSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Color spriteColor = spriteRenderer.material.color;
        spriteColor.a = 0.5f;
        spriteRenderer.material.color = spriteColor;
        collider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (canInteract)
            {
                SlowTimer();
                collider.enabled = false;
                spriteRenderer.enabled = false;
                Destroy(gameObject, 5.1f);
            }
        }
    }

    void SlowTimer()
    {
        clockSystem.timeSpeed = 0.5f;
        Invoke("ResetTimer", 5f);
    }

    void ResetTimer()
    {
        clockSystem.timeSpeed = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (interactTime >= timeThreshold)
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
}
