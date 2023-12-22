using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Haptics.Vibrations;

public class ParticleSFXScript : MonoBehaviour
{
    public AudioSource particleaudioSource;
    public AudioClip[] particleSFX;
    public GameManagerScript gamemanager;
    // Start is called before the first frame update
    void Start()
    {
        VibrateHaptics.Initialize();
        AudioClip sound = particleSFX[(Random.Range(0, particleSFX.Length))];
        particleaudioSource.PlayOneShot(sound);
        gamemanager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        if (gamemanager.gameEnded == false)
        {
            VibrateHaptics.VibrateHeavyClick();
            Invoke("StopVibration", 0.5f);
        }
    }

    void StopVibration()
    {
        VibrateHaptics.Release();
    }

    // Update is called once per frame
    void Update()
    {
        if(gamemanager.gameEnded == true)
        {
            particleaudioSource.volume = 0f;
        }
    }
}
