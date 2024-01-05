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
    public AudioSource buildingAudioSource;
    public AudioSource treeaudioSource;
    public AudioSource civilianAudioSource;
    public AudioSource carAudioSource;
    public AudioSource propAudioSource;
    public AudioSource babblingAudioSource;
    public AudioSource endScreenAudioSource;

    public AudioClip[] treeSFX;
    public AudioClip[] militaryIncomingSFX;
    public AudioClip[] feedbackSFX;
    public AudioClip wooshSFX1;
    public AudioClip wooshSFX2;
    public AudioClip[] screamingSFX;
    public AudioClip sirenSFX;
    public AudioClip Victory;
    public AudioClip Defeat;
    public AudioClip[] buildingdamageSFX;
    public AudioClip[] buildingdeathSFX;
    public AudioClip[] civillianDeathSFX;
    public AudioClip[] carDeathSFX;
    public GameManagerScript gamemanager;
    public EventManager eventmanagerScript;
    [SerializeField] float minTime;
    [SerializeField] float maxTime;
    private bool isCivillianDeathSFXPlaying = false;
    private float civillianDeathCooldown = 0.3f; // Set the cooldown time (adjust as needed)
    private bool isBuildingDeathSFXPlaying = false;
    private float buildingDeathCooldown = 0.3f; // Set the cooldown time (adjust as needed)
    private bool isTreeSFXPlaying = false;
    private float treeSFXCooldown = 0.3f; // Set the cooldown time (adjust as needed)
    private bool isCarSFXPlaying = false;
    private float carSFXCooldown = 0.3f; // Set the cooldown time (adjust as needed)
    // Start is called before the first frame update
    void Start()
    {
        minTime = 1f;
        maxTime = 1f;
        eventmanagerScript = GameObject.Find("EventManager").GetComponent<EventManager>();
        gamemanager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        PlayCiviBabbling();

    }
     void Awake()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
      if(gamemanager.gameEnded == true)
        {
            CivilianSource.volume = 0f;
        }
    }

    public void PlayBGM()
    {
        BGMSource.Play();
    }
    
    public void PlayVictoryBGM()
    {
        endScreenAudioSource.PlayOneShot(Victory);
        Debug.Log("Win");
    }

    public void PlayDefeatBGM()
    {
        endScreenAudioSource.PlayOneShot(Defeat);
        Debug.Log("Lose");
    }
    public void PlayScreaming()
    {
        // Choose a random screaming sound from the list
        AudioClip randomScream = screamingSFX[Random.Range(0, 2)];

        // Play the chosen screaming sound
        CivilianSource.PlayOneShot(randomScream);
    }

    public void PlayTap()
    {
        AudioClip SoundtoPlay = feedbackSFX[Random.Range(0, 3)];
        feedbackaudioSource.PlayOneShot(SoundtoPlay);
    }
    public void PlayPointCalculation()
    {
        AudioClip SoundtoPlay = feedbackSFX[Random.Range(4, 4)];
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

    public void playBuildingDamageFX()
    {
        AudioClip damagesoundtoPlay = buildingdamageSFX[Random.Range(0, buildingdamageSFX.Length)];
        buildingAudioSource.PlayOneShot(damagesoundtoPlay);
        Debug.Log("PlaySound");
    }

    public void playBuildingDeathSFX()
    {
        if (!isBuildingDeathSFXPlaying)
        {
            StartCoroutine(PlayBuildingDeathWithCooldown());
        }
    }

    private IEnumerator PlayBuildingDeathWithCooldown()
    {
        isBuildingDeathSFXPlaying = true;

        AudioClip deathsoundtoPlay = buildingdeathSFX[Random.Range(0, buildingdeathSFX.Length)];
        buildingAudioSource.PlayOneShot(deathsoundtoPlay);

        yield return new WaitForSeconds(buildingDeathCooldown);

        isBuildingDeathSFXPlaying = false;
    }

    public void PlayTreeSFX()
    {
        if (!isTreeSFXPlaying)
        {
            StartCoroutine(PlayTreeSFXWithCooldown());
        }
    }

    private IEnumerator PlayTreeSFXWithCooldown()
    {
        isTreeSFXPlaying = true;

        AudioClip deathSFX = treeSFX[(Random.Range(0, treeSFX.Length))];
        treeaudioSource.PlayOneShot(deathSFX);

        yield return new WaitForSeconds(treeSFXCooldown);

        isTreeSFXPlaying = false;
    }

    public void PlaycivillianDeathSFX()
    {
        if (!isCivillianDeathSFXPlaying)
        {
            StartCoroutine(PlayCivillianDeathWithCooldown());
        }
    }

    private IEnumerator PlayCivillianDeathWithCooldown()
    {
        isCivillianDeathSFXPlaying = true;

        AudioClip deathsoundtoPlay = civillianDeathSFX[Random.Range(0, civillianDeathSFX.Length)];
        civilianAudioSource.PlayOneShot(deathsoundtoPlay);

        yield return new WaitForSeconds(civillianDeathCooldown);

        isCivillianDeathSFXPlaying = false;
    }

    public void PlayCarSFX()
    {
        if (!isCarSFXPlaying)
        {
            StartCoroutine(PlayCarSFXWithCooldown());
        }
    }

    private IEnumerator PlayCarSFXWithCooldown()
    {
        isCarSFXPlaying = true;

        // Assuming you have a carSFX array similar to treeSFX and others
        AudioClip carSound = carDeathSFX[(Random.Range(0, carDeathSFX.Length))];
        carAudioSource.PlayOneShot(carSound);

        yield return new WaitForSeconds(carSFXCooldown);

        isCarSFXPlaying = false;
    }

    // For Props use car sounds since they are basically the same
    public void PlayPropSFX()
    {
        if (!isCarSFXPlaying)
        {
            StartCoroutine(PlaypropSFXWithCooldown());
        }
    }

    private IEnumerator PlaypropSFXWithCooldown()
    {
        isCarSFXPlaying = true;

        // Assuming you have a carSFX array similar to treeSFX and others
        AudioClip carSound = carDeathSFX[(Random.Range(0, carDeathSFX.Length))];
        propAudioSource.PlayOneShot(carSound);

        yield return new WaitForSeconds(carSFXCooldown);

        isCarSFXPlaying = false;
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
    
    public void PlayCiviBabbling()
    {
        babblingAudioSource.Play();
       
    }
    public void StopBGM()
    {
        BGMSource.Stop();
    }

    public void FadeOutBGM(float duration)
    {
        StartCoroutine(FadeOutBGMCoroutine(duration));
    }

    // Coroutine for lerping BGM volume
    private IEnumerator FadeOutBGMCoroutine(float duration)
    {
        float elapsedTime = 0f;
        float startVolume = BGMSource.volume;

        while (elapsedTime < duration)
        {
            BGMSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the volume is set to 0 at the end
        BGMSource.volume = 0f;
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
