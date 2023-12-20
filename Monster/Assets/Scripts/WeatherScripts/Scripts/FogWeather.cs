using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogWeather : Weather
{
    public WeatherScriptableObject weatherData;
    public float visionDistance = 5f;
    public Transform player;
    private PlayerHandler inputHandler;

    public Transform visionMask; // Reference to the vision mask sprite
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
    }

    void StaticEffect()
    {
        //inputHandler.ultimateRadius = 5;
        visionMask.position = player.position;

        // Limit the player's vision based on the vision distance
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > visionDistance)
        {
            // Set the vision mask's scale to hide areas outside of the vision distance
            float scale = visionDistance / distanceToPlayer;
            visionMask.localScale = new Vector3(scale, scale, 1f);
        }
        else
        {
            // If the player is closer than the vision distance, show the full vision
            visionMask.localScale = new Vector3(1f, 1f, 1f);
        }

    }

    // Update is called once per frame
    void Update()
    {
        StaticEffect();
    }
}
