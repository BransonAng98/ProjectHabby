using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmarkAMScript : MonoBehaviour
{
    public AudioSource lmBGM;
   
    public AudioSource screamingAudioSource;

    public AudioClip bgm;
    
    public AudioClip[] screamingSFX;
    [SerializeField] float minTime;
    [SerializeField] float maxTime;
    // Start is called before the first frame update
    void Start()
    {
        PlayBGM();
        StartCoroutine(PlayRandomScreaming());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayBGM()
    {
        lmBGM.PlayOneShot(bgm);
    }



    public void PlayScreaming()
    {
        // Choose a random screaming sound from the list
        AudioClip randomScream = screamingSFX[Random.Range(0, screamingSFX.Length)];

        // Play the chosen screaming sound
        screamingAudioSource.PlayOneShot(randomScream);
    }

    public IEnumerator PlayRandomScreaming()
    {
        while (true)
        {
            // Wait for a random amount of time before playing the next scream
            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);

            while (screamingAudioSource.isPlaying)
            {
                yield return null;
            }
            // Play a random scream
            PlayScreaming();
        }
    }
}
