using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject player; // Prefab of the player
    public GameObject deployScreen;
    public Animator barAnim;
    private bool gameStarted = false;
    private bool gameRevealed = false;
    public GameObject enemySpawner;
    public List<GameObject> obstacleList = new List<GameObject>();
    private void Start()
    {
        Time.timeScale = 0;
        deployScreen.SetActive(true);
        player = GameObject.Find("Player");
      
        player.GetComponent<SpriteRenderer>().enabled = false;
        barAnim.SetBool("RevealGame", false);
        AstarPath.active.Scan(); //scan the grid
        ScanAndInsert();
        DisableObstacles();

        enemySpawner.SetActive(false);
    }


    void Update()
    {
        if (!gameStarted && Input.anyKeyDown)
        {
            StartGame();
        }
    }

    public void CloseBar()
    {

            barAnim.SetBool("GameRevealed", true);
            Debug.Log("ShowGame");
        

    }

    public void OpenBar()
    {
       
            barAnim.SetBool("RevealGame", true);

            gameRevealed = true;
        
    }
    void StartGame()
    {
        Time.timeScale = 1;
        

        // Optionally, set the player's parent or any other initialization

       

        // Call any other functions or actions to start your game
        deployScreen.SetActive(false);

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
}
