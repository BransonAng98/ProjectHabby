using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sandstorm : MonoBehaviour
{
    //Public Variables
    public List<GameObject> affectedEntity = new List<GameObject>();

    //Private Variables

    //Serialized Variables

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!affectedEntity.Contains(collision.gameObject))
        {

        }

        else
        {
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
