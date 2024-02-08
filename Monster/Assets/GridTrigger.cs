using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTrigger : MonoBehaviour
{
    public BoxCollider2D entitycollider;
    public GridSpawner gridGeneratorScript;
    [SerializeField] bool isTriggered;
    // Start is called before the first frame update

    void Start()
    {

    }
    private void Awake()
    {
        entitycollider = GetComponent<BoxCollider2D>();
        gridGeneratorScript = GameObject.FindGameObjectWithTag("GridSpawner").GetComponent<GridSpawner>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isTriggered = true;
            Debug.Log("CheckpointHit");
            if (isTriggered == true)
            {
                gridGeneratorScript.Checkpoint++;
                isTriggered = false;
            }

        }

        if (collision.gameObject.tag == "GridRemover")
        {
            Debug.Log("GridRemoverHit");
            gridGeneratorScript.activeGrids.Remove(gameObject);
            // Destroy the object
            Destroy(gameObject);
        }
    }
}
