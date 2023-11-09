using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{
    public AudioSource BGMSource;
    public AudioSource CivilianSource;
    public AudioSource feedbackaudioSource;
    public AudioClip[] feedbackSFX;
    public AudioClip[] screamingSFX;
    public AudioClip bgm;
    public GameManagerScript gamemanager;
    [SerializeField] float minTime;
    [SerializeField] float maxTime;
    // Start is called before the first frame update
    void Start()
    {
        minTime = 1f;
        maxTime = 1f;
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
        // Choose a random screaming sound from the list
        AudioClip randomScream = screamingSFX[Random.Range(0, screamingSFX.Length)];

        // Play the chosen screaming sound
        CivilianSource.PlayOneShot(randomScream);
    }

    public void PlayTap()
    {
        AudioClip SoundtoPlay = feedbackSFX[Random.Range(0, feedbackSFX.Length)];
        feedbackaudioSource.PlayOneShot(SoundtoPlay);
    }

   

    public IEnumerator PlayRandomScreaming()
    {
        while (true)
        {
            // Wait for a random amount of time before playing the next scream
            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);

            // Play a random scream
            PlayScreaming();
        }
    }


    public IEnumerator StartTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

        // After 5 seconds, change min and max times
        minTime = 10f;
        maxTime = 15f;
    }
}
