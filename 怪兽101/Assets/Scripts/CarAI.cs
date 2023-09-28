using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 randomDestination;
    public float roamRadius = 20.0f;
    public float roamInterval = 5.0f;
    private Transform player;
    private Vector3 lastPosition;
    private Vector2 movingDirection;

    private SpriteRenderer spriteRenderer;
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    public Sprite destroyedSprite;

    bool isDestroyed;
    Collider2D entityCollider;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        InvokeRepeating("SetRandomDestination", 0, roamInterval);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastPosition = transform.position;
        entityCollider = GetComponent<Collider2D>();
    }

    void SetRandomDestination()
    {
        if(isDestroyed != true)
        {
            randomDestination = Random.insideUnitSphere * roamRadius;
            randomDestination += transform.position;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDestination, out hit, roamRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(randomDestination);
            }
        }

        else { agent.updatePosition = false; agent.updateRotation = false; }
    }

    void FlipSprite(Vector2 movDir)
    {
        if (Mathf.Abs(movDir.x) > Mathf.Abs(movDir.y))
        {
            if (movDir.x > 0)
            {
                spriteRenderer.sprite = rightSprite;
            }
            else
            {
                spriteRenderer.sprite = leftSprite;
            }
        }
        else
        {
            if (movDir.y > 0)
            {
                spriteRenderer.sprite = upSprite;
            }
            else
            {
                spriteRenderer.sprite = downSprite;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerLeg")
        {
            isDestroyed = true;
            spriteRenderer.sprite = destroyedSprite;
            entityCollider.enabled = false;
            Destroy(gameObject, 5f);
        }
    }

    private void Update()
    {
        Vector3 currentPosition = transform.position;

        // Calculate the velocity or position change since the last frame.
        Vector3 positionChange = currentPosition - lastPosition;

        // Calculate the moving direction as a normalized vector.
        movingDirection = positionChange.normalized;

        // Update the last position for the next frame.
        lastPosition = currentPosition;

        if(isDestroyed != true)
        {
            FlipSprite(movingDirection);
        }
    }
}
