using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAMScript : MonoBehaviour
{
    public AudioSource BGMSource;
    public AudioSource FeedbackSource;

    void Start()
    {
        PlayBGM();
    }

    public void PlayBGM()
    {
        BGMSource.Play();
    }

    public void PlayTap()
    {

        FeedbackSource.Play();
    }
}
