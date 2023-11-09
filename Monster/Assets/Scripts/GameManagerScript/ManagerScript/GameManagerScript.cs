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
    public Animator barAnim;
    public bool gameStarted = false;
    public List<GameObject> obstacleList = new List<GameObject>();
    public MeteorScript meteor;
    public AudioManagerScript audiomanager;
    private GNAManager GNAManager;
    private LevelManager levelManger;
    public GameObject endScreen;
    public GameObject winScreen;
    public GameObject loseScreen;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI GNAText;
    private PlayerHandler inputHandler;
    public bool isVictory;

    private void Start()
    {

        
        Time.timeScale = 1f;
        deployScreen.SetActive(true);

        player.GetComponent<MeshRenderer>().enabled = false;
        barAnim.SetBool("RevealGame", false);
        AstarPath.active.Scan(); //scan the grid
        ScanAndInsert();
        DisableObstacles();
        meteor = GameObject.Find("Meteor").GetComponent<MeteorScript>();
        audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();


        endScreen.SetActive(false);
        levelManger = GetComponent<LevelManager>();
        GNAManager = GetComponent<GNAManager>();
        inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
    }


    void Update()
    {
        if (!gameStarted && Input.anyKeyDown)
        {
            audiomanager.PlayTap();
            StartGame();
        }
    }

    public void TriggerEndScreen()
    {
        inputHandler.canMove = false;
        Time.timeScale = 0f;
        levelText.text = "" + levelManger.levelData.cityLevel;
        GNAText.text = "" + GNAManager.gnaData.inGameGNA;
        endScreen.SetActive(true);
        if (isVictory == false)
        {
            winScreen.SetActive(false);
        }

        else
        {
            loseScreen.SetActive(false);
        }
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene("LevelSelectScene");
    }

    public void CloseBar()
    {
        barAnim.SetBool("GameRevealed", true);
    }

    public void OpenBar()
    {
        barAnim.SetBool("RevealGame", true);
    }
    void StartGame()
    {
        // Call any other functions or actions to start your game
        deployScreen.SetActive(false);

        meteor.isMoving = true;
        meteor.MeteorMovingSFX();
        audiomanager.PlayBGM();
        StartCoroutine(audiomanager.PlayRandomScreaming());
        StartCoroutine(audiomanager.StartTimer(1f));

        // Set the game as started
        gameStarted = true;
        OpenBar();

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