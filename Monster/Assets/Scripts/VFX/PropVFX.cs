using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropVFX : MonoBehaviour
{
    public GameObject shatterVFX;
    public Sprite destroyedSprite;
    public SpriteRenderer spriteRenderer;
    private Collider2D propCollider;
    // Start is called before the first frame update
    void Start()
    {
        propCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerLeg"))
        {
            Instantiate(shatterVFX, transform.position, Quaternion.identity);
            spriteRenderer.sprite = destroyedSprite;
            propCollider.enabled = false;
        }
    }
}