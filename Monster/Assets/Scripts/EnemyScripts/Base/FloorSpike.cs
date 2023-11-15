using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSpike : MonoBehaviour
{
    public int damage;
    [SerializeField] private bool isTriggered;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!isTriggered)
            {
                PlayerHealthScript playerDamage = collision.GetComponent<PlayerHealthScript>();
                if (playerDamage != null)
                {
                    playerDamage.TakeDamage(damage);
                }

                isTriggered = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTriggered = false;
        }
    }
}
