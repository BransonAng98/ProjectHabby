using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDataHandler : MonoBehaviour
{
    public string buttonID;
    public int upgradeTier;
    [SerializeField] int buttonState;
    PlayerStatScriptableObject playerData;
    Button thisGO;

    // Start is called before the first frame update
    void Start()
    {
        thisGO = GetComponent<Button>();
        buttonState = PlayerPrefs.GetInt(buttonID);
        CheckForActive();
    }

    void CheckForActive()
    {
        if(upgradeTier != playerData.upgradeLevel)
        {
            thisGO.interactable = false;
        }

        else
        {
            if (buttonState == 1)
            {
                thisGO.interactable = false;
            }

            else
            {
                return;
            }
        }
    }

    public void SaveData()
    {
        //1 is for inactive
        PlayerPrefs.SetInt(buttonID, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
