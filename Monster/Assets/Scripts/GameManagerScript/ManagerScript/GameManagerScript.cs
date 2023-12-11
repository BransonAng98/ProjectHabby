using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManagerScript : MonoBehaviour
{
    public GameObject player; // Prefab of the player
    public GameObject meteorObject;
    public GameObject deployScreen;
    public bool gameStarted = false;
    public List<GameObject> obstacleList = new List<GameObject>();
   
    public AudioManagerScript audiomanager;
    //private GNAManager GNAManager;
    private LevelManager levelManager;
    public GameObject endScreen;
    public GameObject winScreen;
    public GameObject loseScreen;
    public TextMeshProUGUI levelText;

    //public TextMeshProUGUI GNAText;
    private PlayerHandler inputHandler;
    public bool isVictory;
    public bool hasActivated;

    //Meteor
    public GameObject playerStatusBars;
    public GameObject hitIndicator;
    //public List<Collider2D> playerLegs = new List<Collider2D>();
    public GameObject joystick;
    public ClockSystem clock;
    public GameObject scoreDisplay;

    private void Start()
    {
        Time.timeScale = 1f;
     
        audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        levelManager = GetComponent<LevelManager>();
        //GNAManager = GetComponent<GNAManager>();
        player.GetComponent<MeshRenderer>().enabled = false;
        //scoreDisplay.SetActive(false);
        deployScreen.SetActive(true);
        endScreen.SetActive(false);

        //AstarPath.active.Scan(); //scan the grid
        ScanAndInsert();
        DisableObstacles();
        DeactivatePlayer();
        
    }


    void Update()
    {
        if (!gameStarted && Input.anyKeyDown)
        {
            StartGame();
        }
    }

    public void TriggerEndScreen()
    {
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
        SceneManager.LoadScene("LevelSelectScene");
    }

    void DeactivatePlayer()
    {
        playerStatusBars.SetActive(false);
        hitIndicator.SetActive(false);
        joystick.SetActive(false);
        //foreach (Collider2D collider in playerLegs)
        //{
        //    collider.gameObject.SetActive(false);
        //}
    }

    public void ActivatePlayer()
    {
        Invoke("ActivateInput", 4.5f);
        //foreach (Collider2D collider in playerLegs)
        //{
        //    collider.gameObject.SetActive(true);
        //}
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
        audiomanager.PlayTap();
        // Call any other functions or actions to start your game
        deployScreen.SetActive(false);
        audiomanager.PlayBGM();
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


    //Trigger all the end game stuff
}