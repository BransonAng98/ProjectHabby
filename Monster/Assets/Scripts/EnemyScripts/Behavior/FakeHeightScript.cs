using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FakeHeightScript : MonoBehaviour
{
    public GameObject spawnerReference;
    public UnityEvent onGroundHitEvent;
    public Transform transObject;
    public Transform transBody;


    public Vector2 groundVelocity;
    public float verticalVelocity;
    private float minGravity = -30f;
    private float maxGravity = -50f;
    [SerializeField] private float Gravity;
    private float minlandheight = -2f;
    private float maxlandheight = 2f;
    private float landheight;
    public bool isGrounded;


    private void Start()
    {
        Gravity = Random.Range(minGravity, maxGravity);
        landheight = Random.Range(minlandheight, maxlandheight);
        if (spawnerReference != null)
        {
            spawnerReference.GetComponent<BigBuildingEnemy>();
        }

    }
    private void Update()
    {
        UpdatePosition();
        checkGroundHit();
    }

    public void Initialize(Vector2 groundVelocity, float verticalVelocity)
    {
        this.groundVelocity = groundVelocity;
        this.verticalVelocity = verticalVelocity;
    }

    void UpdatePosition()
    {
        if (!isGrounded)
        {
            verticalVelocity += Gravity * Time.deltaTime;
            transBody.position += new Vector3(0, verticalVelocity, 0) * Time.deltaTime;
        }
        transObject.position += (Vector3)groundVelocity * Time.deltaTime;

    }

    void checkGroundHit()
    {
        /*float heightdistance = spawnerReference.GetComponent<BigBuildingEnemy>().spawnheight;
        if (transBody.position.y  < transObject.position.y - heightdistance && !isGrounded)
        {
            isGrounded = true;
            //transBody.position = transObject.position;
            
            Groundhit();
        }*/
        if (spawnerReference != null)
        {
            float heightdistance = spawnerReference.GetComponent<BigBuildingEnemy>().spawnheight;
            if (transBody.position.y < transObject.position.y - heightdistance + landheight && !isGrounded)
            {
                isGrounded = true;
                //transBody.position = transObject.position;

                Groundhit();
            }
        }

        else
        {
            isGrounded = true;
            Groundhit();
        }
    }

    void Groundhit()
    {
        onGroundHitEvent.Invoke();
    }

    public void Stick()
    {
        groundVelocity = Vector2.zero;
    }

    public void Delete()
    {
        Destroy(gameObject);
    }
}

