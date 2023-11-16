using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{
    public AudioSource BGMSource;
    public AudioSource CivilianSource;
    public AudioSource feedbackaudioSource;
    public AudioSource militaryAbilityWarningSource;
    public AudioSource militaryAbilitySource;
    public AudioClip[] militaryIncomingSFX;
    public AudioClip[] feedbackSFX;
    public AudioClip wooshSFX1;
    public AudioClip wooshSFX2;
    public AudioClip[] screamingSFX;
    public AudioClip sirenSFX;
    public AudioClip bgm;
    public AudioClip Victory;
    public AudioClip Defeat;
    public GameManagerScript gamemanager;
    public EventManager eventmanagerScript;
    [SerializeField] float minTime;
    [SerializeField] float maxTime;
    // Start is called before the first frame update
    void Start()
    {
        minTime = 1f;
        maxTime = 1f;
        eventmanagerScript = GameObject.Find("EventManager").GetComponent<EventManager>();
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void PlayBGM()
    {
        BGMSource.PlayOneShot(bgm);
    }
    
    public void PlayVictoryBGM()
    {
        BGMSource.PlayOneShot(Victory);
        Debug.Log("Win");
    }

    public void PlayDefeatBGM()
    {
        BGMSource.PlayOneShot(Defeat);
        Debug.Log("Lose");
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
        militaryAbilityWarningSource.PlayOneShot(sirenSFX);
    }

    public void PlayWoosh1SFX()
    {
        militaryAbilityWarningSource.PlayOneShot(wooshSFX1);
    }

    public void PlayWoosh2SFX()
    {
        militaryAbilityWarningSource.PlayOneShot(wooshSFX2);
    }

    public void PlayIncomingAbility()
    {
        //float randomPitch = Random.Range(0.8f, 1.2f);
        //militaryAbilitySource.pitch = randomPitch;
        
      
        if(eventmanagerScript.eventNumber == 0)
        {
            //airstrike
            AudioClip soundtoPlay = militaryIncomingSFX[0];
            militaryAbilitySource.PlayOneShot(soundtoPlay);
        }
        if (eventmanagerScript.eventNumber == 1)
        {
            //artillery
            AudioClip soundtoPlay = militaryIncomingSFX[1];
            militaryAbilitySource.PlayOneShot(soundtoPlay);
        }
        if (eventmanagerScript.eventNumber == 2)
        {
            AudioClip soundtoPlay = militaryIncomingSFX[0];
            militaryAbilitySource.PlayOneShot(soundtoPlay);
        }

       
    }

    public void StopBGM()
    {
        BGMSource.Stop();
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
