using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{
    public AudioSource BGMSource;
    public AudioSource CivilianSource;
    public AudioSource feedbackaudioSource;
    public AudioSource militaryAbilitySource;
    public AudioClip[] feedbackSFX;
    public AudioClip wooshSFX1;
    public AudioClip wooshSFX2;
    public AudioClip[] screamingSFX;
    public AudioClip sirenSFX;
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

    public void PlayWarningSFX()
    {
        militaryAbilitySource.PlayOneShot(sirenSFX);
    }

    public void PlayWoosh1SFX()
    {
        militaryAbilitySource.PlayOneShot(wooshSFX1);
    }

    public void PlayWoosh2SFX()
    {
        militaryAbilitySource.PlayOneShot(wooshSFX2);
    }




    public IEnumerator PlayRandomScreaming()
    {
        while (true)
        {
            // Wait for a random amount of time before playing the next scream
            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);
            
            while (CivilianSource.isPlaying)
            {
                yield return null;
            }
            // Play a random scream
            PlayScreaming();
        }
    }


    public IEnumerator StartTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

        // After 5 seconds, change min and max times
        minTime = 20f;
        maxTime = 30f;
    }
}
