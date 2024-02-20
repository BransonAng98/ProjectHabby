using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMoveUpScript : MonoBehaviour
{
    [SerializeField]
    private float movespeed;
    public GridSpawner gridspawnerScript;
    public PlayerEndlessRunnerController player;

    // Start is called before the first frame update
    void Start()
    {
        gridspawnerScript = GameObject.Find("GridSpawner").GetComponent<GridSpawner>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEndlessRunnerController>();

    }

    // Update is called once per frame
    void Update()
    {
        if (player.canMove)
        {
            moveGrid();
        }
    }

    public void moveGrid()
    {
        movespeed = gridspawnerScript.gridSpeed;
        transform.Translate(Vector2.down * movespeed * Time.deltaTime);
    }
}
