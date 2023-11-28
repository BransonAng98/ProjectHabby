using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class vehicleFakeHeightScript : MonoBehaviour
{
    public UnityEvent onGroundHitEvent;
    public Transform transObject;
    public Transform transBody;
    public CarAI carscript;

    public Vector2 groundVelocity;
    public float verticalVelocity;
    private float gravity = -40;

    public bool isGrounded;


    private void Start()
    {
        carscript = GetComponentInChildren<CarAI>();
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
            verticalVelocity += gravity * Time.deltaTime;
            transBody.position += new Vector3(0, verticalVelocity, 0) * Time.deltaTime;
        }
        transObject.position += (Vector3)groundVelocity * Time.deltaTime;

    }

    void checkGroundHit()
    {
        if (transBody.position.y  < transObject.position.y && !isGrounded)
        {
            isGrounded = true;
            transBody.position = transObject.position;
            
            Groundhit();
        }
 
    }

    void Groundhit()
    {
        onGroundHitEvent.Invoke();
    }

    public void Stick()
    {
        //groundVelocity = Vector2.zero;
        //carscript.Death();
        if(carscript.isKicking)
        {
            groundVelocity = Vector2.zero;
            GetComponentInChildren<Rigidbody2D>().angularVelocity = 0f;
            carscript.Death();
            Invoke("Delete", 2f);
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
