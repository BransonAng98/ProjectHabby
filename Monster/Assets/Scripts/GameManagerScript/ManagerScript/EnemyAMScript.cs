using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAMScript : MonoBehaviour
{
    public AudioSource buildingAudioSource;
    public AudioClip[] damageSFX;
    public AudioClip[] deathSFX;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBuildingDamagedSFX()
    {
        AudioClip damagesoundtoPlay = damageSFX[Random.Range(0, damageSFX.Length)];
        buildingAudioSource.PlayOneShot(damagesoundtoPlay);
    }
}
