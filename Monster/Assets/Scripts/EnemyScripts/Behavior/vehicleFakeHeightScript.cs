using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class vehicleFakeHeightScript : MonoBehaviour
{
    public FadeObjectinParent fadescript;
    public UnityEvent onGroundHitEvent;
    public Transform transObject;
    public Transform transBody;
    public CarAI carscript;

    public Vector2 groundVelocity;
    public float verticalVelocity;
    private float minGravity = -20f;
    private float maxGravity = -40f;
    [SerializeField] private float Gravity;

    public bool isGrounded;


    private void Start()
    {
        Gravity = Random.Range(minGravity, maxGravity);
        carscript = GetComponentInChildren<CarAI>();
        fadescript = GetComponent<FadeObjectinParent>();
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
        if (transBody.position.y  < transObject.position.y && !isGrounded)
        {
            isGrounded = true;
            //transBody.position = transObject.position;
            
            Groundhit();
        }
 
    }

    void Groundhit()
    {
        onGroundHitEvent.Invoke();
    }

    public void Stick()
    {
        if(carscript.isKicking)
        {
            carscript.hasDied = true;
            groundVelocity = Vector2.zero;
            GetComponentInChildren<Rigidbody2D>().angularVelocity = 0f;
            carscript.entityCollider.enabled = false;
            carscript.Death();
            fadescript.StartFading();
        }
        else
        {
            groundVelocity = Vector2.zero;
        }
    }

    public void Delete()
    {
        Destroy(gameObject);
    }
}
