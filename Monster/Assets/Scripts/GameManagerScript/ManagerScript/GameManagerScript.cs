using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Haptics.Vibrations;

public class GameManagerScript : MonoBehaviour
{
    public LevelManagerScriptableObject levelData;
    public GameObject player; // Prefab of the player
    public GameObject meteorObject;
    public GameObject deployScreen;
    public GameObject tutorialScreen;
    public bool gameStarted = false;
    public List<GameObject> obstacleList = new List<GameObject>();
   
    public AudioManagerScript audiomanager;
    //private GNAManager GNAManager;
    private LevelManager levelManager;
    public GameObject endScreen;
    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject destructionBar;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI startingText;
 
    public float textMoveSpeed;
    public float textFadeDuration;
    public float textMoveDuration;
    public float countdownDuration = 3.0f;

    //public TextMeshProUGUI GNAText;
    private PlayerHandler inputHandler;
    public bool isVictory;
    public bool hasActivated;
    public float fadeDuration;
    public float displayDuration;


    //Meteor
    public GameObject playerStatusBars;
    public GameObject hitIndicator;
    //public List<Collider2D> playerLegs = new List<Collider2D>();
    public GameObject joystick;
    public ClockSystem clock;
    public GameObject scoreDisplay;
    public bool gameEnded;
    public bool activatePlayer;

    private void Start()
    {
        VibrateHaptics.Initialize();
        Time.timeScale = 1f;
        audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        levelManager = GetComponent<LevelManager>();
        startingText = GameObject.Find("GoText").GetComponent<TextMeshProUGUI>();
        //GNAManager = GetComponent<GNAManager>();
        player.GetComponent<MeshRenderer>().enabled = false;
        //scoreDisplay.SetActive(false);
        endScreen.SetActive(false);
        //AstarPath.active.Scan(); //scan the grid
        DisplayObjective();
        ScanAndInsert();
        DisableObstacles();
        DeactivatePlayer();
       
    }


    void Update()
    {
        if (!gameStarted && Input.anyKeyDown)
        {
            StartGame();
            StartCoroutine(MoveUpAndFadeOut(objectiveText.gameObject, textMoveDuration, textFadeDuration));
        }
    }

    public void TriggerEndScreen()
    {
        //gameEnded = true;
        inputHandler.enableInput = false;
        Time.timeScale = 1f;
        levelText.text = "" + levelManager.levelData.cityLevel;
        //GNAText.text = "" + GNAManager.gnaData.inGameGNA;
        endScreen.SetActive(true);
       
        if (isVictory == false)
        {
            
            audiomanager.PlayVictoryBGM();  
            winScreen.SetActive(false);
            
           
        }

        else
        {
            audiomanager.PlayDefeatBGM();
            loseScreen.SetActive(false);
            
        }
        scoreDisplay.SetActive(true);
        
    }

    public void LoadNextScene()
    {
        VibrateHaptics.VibrateClick();
        VibrateHaptics.Release();
        SceneManager.LoadScene("LevelSelectScene");
    }

    void DeactivatePlayer()
    {
        playerStatusBars.SetActive(false);
        hitIndicator.SetActive(false);
        joystick.SetActive(false);

    }

    public void ActivatePlayer()
    {
        TriggerIntro();
        Invoke("ActivateInput", 3.5f);
    }

    void ActivateInput()
    {
   
        inputHandler.enableInput = true;
        inputHandler.canAttack = true;
        inputHandler.canMove = true;
        clock.startTime = true;
        hasActivated = true;
        //inputHandler.entitycollider.enabled = true;
        inputHandler.EnableColliders();
        joystick.SetActive(true);
      
    }

    public void SpawnPlayer()
    {
       
        audiomanager.babblingAudioSource.Stop();
        player.GetComponent<MeshRenderer>().enabled = true;
        inputHandler.DisableMovement(5);
        playerStatusBars.SetActive(true);
        hitIndicator.SetActive(true);
        
    }

    void SpawnMeteor()
    {
        Vector2 MeteorSpawnZone = new Vector2(player.transform.position.x + 15f, player.transform.position.y + 25f);
        Instantiate(meteorObject, MeteorSpawnZone, Quaternion.identity);
    }
    void StartGame()
    {
        VibrateHaptics.VibrateClick();
        audiomanager.PlayTap();
        // Call any other functions or actions to start your game
        if (deployScreen.activeSelf == true)
        {
            deployScreen.SetActive(false);
        }

        if(tutorialScreen.activeSelf == true)
        {
            tutorialScreen.SetActive(false);
            levelData.tutorialPlayed = true;
        }
        audiomanager.PlayBGM();
        audiomanager.BGMSource.loop = true;
        
        StartCoroutine(audiomanager.PlayRandomScreaming());
        StartCoroutine(audiomanager.StartTimer(1f));
        
        // Set the game as started
        gameStarted = true;
        SpawnMeteor();

    }

    void ScanAndInsert()
    {
        GameObject[] obstacleCollider = GameObject.FindGameObjectsWithTag("Obstacle");

        foreach (var obs in obstacleCollider)
        {
            obstacleList.Add(obs);
        }
    }

    void DisableObstacles()
    {
        foreach (var obs in obstacleList)
        {
            Destroy(obs);
        }

    }

    void DisplayObjective()
    {
        VibrateHaptics.Release();

        if (levelData.tutorialPlayed)
        {
            deployScreen.SetActive(true);
        }

        else
        {
            tutorialScreen.SetActive(true);
        }
        objectiveText.enabled = true;
        // Display the game objective text
        SetObjectiveText("");
        SetObjectiveText("Destroy the city within the time limit!");
    }

    IEnumerator MoveUpAndFadeOut(GameObject obj, float moveDuration, float fadeDuration)
    {
        float elapsedTime = 0f;
        RectTransform rectTransform = obj.GetComponent<RectTransform>();

        // Move up
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;

            // Move the object upwards using RectTransform
            rectTransform.anchoredPosition += Vector2.up * textMoveSpeed * Time.deltaTime;

            yield return null;
        }
        // Reset timer for fading
        elapsedTime = 0f;

        // Fade out
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            // Fade out the object
            objectiveText.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        // Ensure it reaches the target alpha exactly
        objectiveText.alpha = 0f;

        // Turn off the objective text
        objectiveText.enabled = false;
        activatePlayer = true;
    }

    public void TriggerIntro()
    {
        StartCoroutine(StartGameSequence());
    }

    IEnumerator StartGameSequence()
    {
        // Fade in the destruction bar
        yield return StartCoroutine(FadeInObject(destructionBar, fadeDuration));

    }

    void SetObjectiveText(string text)
    {
        objectiveText.text = text;
    }

    public void StartCountdown()
    {
        startingText.enabled = true;
        StartCoroutine(CountdownCoroutine());
    }

    private System.Collections.IEnumerator CountdownCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
        SetCountdownText("3");

        yield return new WaitForSeconds(1.0f);
        SetCountdownText("2");

        yield return new WaitForSeconds(1.0f);
        SetCountdownText("1");

        yield return new WaitForSeconds(1.0f);
        SetCountdownText("GO!");

        // Optionally clear the text after a delay
        yield return new WaitForSeconds(1.0f);
        SetCountdownText("");
        startingText.enabled = false;
    }

    private void SetCountdownText(string text)
    {
        if (startingText != null)
        {
            startingText.text = text;
        }
    }

    IEnumerator FadeInObject(GameObject obj, float duration)
    {
        float elapsedTime = 0f;
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // Start with alpha set to 0

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Fade in the object
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // Ensure it reaches the target alpha exactly
        canvasGroup.alpha = 1f;
    }

    //Trigger all the end game stuff
}