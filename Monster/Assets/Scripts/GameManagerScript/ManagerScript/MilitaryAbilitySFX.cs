using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilitaryAbilitySFX : MonoBehaviour
{
    public AudioManagerScript audiomanager;

    // Start is called before the first frame update
    void Start()
    {
        audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
    }

  

    public void PlayWarning()
    {
        audiomanager.PlayWarningSFX();
    }

    public void PlayWooshStart()
    {
        audiomanager.PlayWoosh1SFX();
    }
    public void PlayWooshEnd()
    {
        audiomanager.PlayWoosh2SFX();
    }
}
