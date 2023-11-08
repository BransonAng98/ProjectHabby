using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{
    public AudioSource BGMSource;
    public AudioSource CivilianSource;
    public AudioClip screamingSFX;
    public AudioClip bgm;
    public GameManagerScript gamemanager;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void PlayBGM()
    {
        BGMSource.PlayOneShot(bgm);
    }

    public void PlayScreaming()
    {
        CivilianSource.PlayOneShot(screamingSFX);
    }
}
