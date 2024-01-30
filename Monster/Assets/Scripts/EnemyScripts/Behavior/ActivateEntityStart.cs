using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateEntityStart : MonoBehaviour
{

    //Public Variables
    public GameManagerScript gameManager;

    //Private Variables
    private IActivatable entityScript;

    //Serialized Variables

    // Start is called before the first frame update
    void Start()
    {
        //External Checks
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerScript>();

        //Internal Checks
        if(entityScript == null)
        {
            entityScript = gameObject.GetComponent<IActivatable>();
            Debug.Log("Locating entityScript");
        }

        else
        {
            Debug.Log("entityScript has been located");
        }

        //Set Variables
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.gameStarted)
        {
            entityScript.Activate();
            this.enabled = false;
        }

        else
        {
            return;
        }
    }
}
