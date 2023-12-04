using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KickHeightScript : MonoBehaviour
{
    public UnityEvent onGroundHitEvent;
    public Transform transObject;
    public Transform transBody;
    public Vector2 groundVelocity;
    public float verticalVelocity;
    private float minGravity = -20f;
    private float maxGravity = -30f;
    [SerializeField] private float Gravity;
    public Civilian civilianscript;
    public bool isGrounded;


    private void Start()
    {
        Gravity = Random.Range(minGravity, maxGravity);
        civilianscript = GetComponentInChildren<Civilian>();

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
        if (transBody.position.y < transObject.position.y && !isGrounded)
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
        if(civilianscript.isKicking)
        {
            groundVelocity = Vector2.zero;
            civilianscript.Death();
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
