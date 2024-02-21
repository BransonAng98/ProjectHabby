using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ERGameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private int hitCounter;
    [SerializeField] int currentCounter;
    public Thief helicopter;
    void Start()
    {
        helicopter = GameObject.FindGameObjectWithTag("Thief").GetComponent<Thief>();
        hitCounter = helicopter.posID;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ModifyHitCounter(bool status, int changeAmt)
    {
        if (status)
        {
            Debug.Log("Thief gets further");
            hitCounter += changeAmt;
            if(hitCounter > 5)
            {
                hitCounter = 5;
                //Do game over condition in thief script
            }
        }

        else
        {
            Debug.Log("Thief gets closer");
            hitCounter -= changeAmt;
            if (hitCounter < 0)
            {
                hitCounter = 0;
            }
        }

        if(currentCounter != hitCounter)
        {
            //Move the helicopter accordingly
            helicopter.ModifyID(hitCounter);
            currentCounter = hitCounter;
        }

        else
        {
            return;
        }
    }
}
