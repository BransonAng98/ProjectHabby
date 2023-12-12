using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSFXScript : MonoBehaviour
{
    public AudioSource particleaudioSource;
    public AudioClip[] particleSFX;
    // Start is called before the first frame update
    void Start()
    {
        AudioClip sound = particleSFX[(Random.Range(0, particleSFX.Length))];
        particleaudioSource.PlayOneShot(sound);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
