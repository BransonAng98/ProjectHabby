using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainWeather : Weather
{
    public WeatherScriptableObject weatherData;
    private PlayerHandler inputHandler;
    public bool isLeft;
    private Rigidbody2D rb;

    public float ogSpeed;
    // Start is called before the first frame update
    void Start()
    {
        weatherData.target = GameObject.FindGameObjectWithTag("Player");
        rb = weatherData.target.GetComponent<Rigidbody2D>();
        inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        ogSpeed = inputHandler.playerData.speed;
        RandomizeRainDirection();
    }

    void RandomizeRainDirection()
    {
        int randomint = Random.Range(0, 1);
        if (randomint == 0)
        {
            //Rain comes from the left
            isLeft = true;
        }
        else
        {
            //Rain comes from the right
            isLeft = false;
        }
    }
    //void StaticEffect()
    //{
    //    if (isLeft)
    //    {
    //        if (rb.velocity.x < 0)
    //        {
    //            inputHandler.move = 1f;
    //        }
    //        else
    //        {
    //            inputHandler.move = ogSpeed;
    //        }
    //    }

    //    else
    //    {
    //        if (rb.velocity.x > 0)
    //        {
    //            inputHandler.move = 1f;
    //        }
    //        else
    //        {
    //            inputHandler.move = ogSpeed;
    //        }
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        //StaticEffect();
    }
}
