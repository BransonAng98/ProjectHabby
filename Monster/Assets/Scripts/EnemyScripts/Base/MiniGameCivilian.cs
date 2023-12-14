using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameCivilian : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] float angularVelocityIncrease;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        RandomizeSpin();
        Destroy(gameObject, 2f);
    }

    void RandomizeSpin()
    {
        int random = Random.Range(1, 5);
        angularVelocityIncrease = random;
    }

    private void Update()
    {
        rb.angularVelocity += angularVelocityIncrease;
    }
}
