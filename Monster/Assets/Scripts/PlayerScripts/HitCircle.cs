using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCircle : MonoBehaviour
{
    public PlayerStatScriptableObject playerData;
    public float speed = 10.0f;  // The speed of rotation.
    public float lagSpeed =  0.2f;  // The speed of rotation.
    public Transform player;    // Reference to the player's Transform.
    public Joystick joystick;
    [SerializeField] private Rigidbody2D playerRb;

    private Vector3 playerLastPosition;
    [SerializeField] private float prevInput;
    [SerializeField] private float currentAngle;
    [SerializeField] private float angleDifference;
    [SerializeField] private float inputAngle;
    [SerializeField] private float prevInputX;
    [SerializeField] private float prevInputY;

    public GameObject rageDirVFX;
    public SpriteRenderer currentArrowSprite;
    public Sprite defaultArrowSprite;
    public Sprite dashArrowSprite;
    public bool triggerHoldingDown;
    public bool lagInput;

    private PlayerHandler playerHandler;


    private void Start()
    {
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        playerHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
       
        currentArrowSprite.sprite = defaultArrowSprite;
        currentArrowSprite.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }

    private void Update()
    {
        PositionAndOrientateArrow();
    }

    void PositionAndOrientateArrow()
    {
        float horizontalInput = joystick.Horizontal;
        float verticalInput = joystick.Vertical;

        if(horizontalInput != 0 && verticalInput != 0)
        {
            // Allow player to move the arrow when the character isn't moving
            inputAngle = Mathf.Atan2(verticalInput, horizontalInput) * Mathf.Rad2Deg;
            prevInputX = horizontalInput;
            prevInputY = verticalInput;
        }

        else
        {
            inputAngle = Mathf.Atan2(prevInputY, prevInputX) * Mathf.Rad2Deg;
        }

        // Calculate the angle between the sprite and the player character.
        float angleToPlayer = Mathf.Atan2(player.position.y - transform.position.y, player.position.x - transform.position.x) * Mathf.Rad2Deg;

        if (playerRb.velocity != Vector2.zero)
        {
            // Calculate the difference between the input angle and angle to the player.
            angleDifference = inputAngle - angleToPlayer;
            prevInput = inputAngle;
        }

        else
        {
            if (!triggerHoldingDown)
            {
                angleDifference = prevInput - angleToPlayer;
            }

            else
            {
                // Calculate the difference between the input angle and angle to the player.
                angleDifference = inputAngle - angleToPlayer;
                prevInput = inputAngle;
            }
        }

        // Update the current angle by adding the difference and the orbit speed.
        if (!lagInput)
        {
            Debug.Log("Using Regular Speed");
            currentAngle += (angleDifference + 180f) % 360f + speed * Time.deltaTime;
        }

        else
        {
            Debug.Log("Using Lag Speed");
            currentAngle += (angleDifference + 180f) % 360f + lagSpeed * Time.deltaTime;
        }

        // Calculate the new position of the object in orbit around the player character.
        float x = player.position.x + 6.5f * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float y = player.position.y + 4.5f * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
        transform.position = new Vector3(x, y, transform.position.z);

        // Rotate the object to face the player character.
        Vector3 direction = player.position - transform.position;
        float lookAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, lookAngle);

        if (playerHandler.isDashing)
        {
            currentArrowSprite.sprite = dashArrowSprite;
            currentArrowSprite.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            rageDirVFX.SetActive(true);
            rageDirVFX.transform.rotation = Quaternion.Euler(lookAngle, -90, 0);

            float dashVFXDirRadius = 4.5f; // Adjust this value to control the distance of rageDirVFX from the player
            rageDirVFX.transform.position = new Vector3(
                player.position.x + dashVFXDirRadius * Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                player.position.y + dashVFXDirRadius * Mathf.Sin(currentAngle * Mathf.Deg2Rad),
                rageDirVFX.transform.position.z);
        }
        else
        {
            currentArrowSprite.sprite = defaultArrowSprite;
            currentArrowSprite.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            rageDirVFX.SetActive(false);
        }

    }

}



