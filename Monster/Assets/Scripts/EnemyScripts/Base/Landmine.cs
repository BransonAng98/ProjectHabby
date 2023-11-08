using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmine : MonoBehaviour
{
    public float radius;
    public float countdown;
    public int damage;
    [SerializeField] private float timer;
    [SerializeField] private bool isTriggered;
    [SerializeField] private bool isStep;

    public GameObject explosionVFX;
    private AudioSource audioS;

    // Start is called before the first frame update
    void Start()
    {
        audioS = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isStep)
        {
            timer += Time.deltaTime;

            if (timer >= countdown)
            {
                Explosion();
                Destroy(gameObject);
            }
        }

        else
        {
            timer = 0f;
        }
    }

    private void Explosion()
    {
        //Play explosion VFX & SFX
        Collider2D hitRadius = Physics2D.OverlapCircle(transform.position, radius);
        if (hitRadius.gameObject.CompareTag("Player"))
        {
            PlayerHealthScript playerHp = hitRadius.GetComponent<PlayerHealthScript>();
            playerHp.TakeDamage(damage);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            isStep = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isStep = false;
        }
    }
}
