using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonDataHandler : MonoBehaviour
{
    public string buttonID;
    public int upgradeTier;
    [SerializeField] int buttonState;
    public PlayerStatScriptableObject playerData;
    public ResourceScriptableObject resourceData;
    Button thisGO;
    public GameObject secondFrame;
    public Button secondButton;
    public TextMeshProUGUI upgradeText;
    public int upgradeCost;

    public GameObject purchaseImage;
    // Start is called before the first frame update
    void Start()
    {
        upgradeCost = int.Parse(upgradeText.text);
        thisGO = GetComponent<Button>();
        purchaseImage.SetActive(false);
        buttonState = PlayerPrefs.GetInt(buttonID);
        secondFrame.SetActive(false);
        CheckForActive();
    }

    void CheckForActive()
    {
        if(upgradeTier != playerData.upgradeLevel)
        {
            thisGO.interactable = false;
            if(buttonState == 1)
            {
                purchaseImage.SetActive(true);
            }
        }

        else
        {
            if (buttonState == 1)
            {
                thisGO.interactable = false;
            }

            else
            {
                thisGO.interactable = true;
            }
        }
    }

    public void SaveData()
    {
        //1 is for inactive
        PlayerPrefs.SetInt(buttonID, 1);
    }

    public void OpenSecondFrame()
    {
        if (!secondFrame.activeSelf)
        {
            secondFrame.SetActive(true);
            thisGO.interactable = false;
        }
    }

    public void CheckSecondButtonActive()
    {
        if (resourceData.currentGold > upgradeCost)
        {
            secondButton.interactable = true;
        }

        else
        {
            return;
        }
    }

    public void TriggerUpgrade(int id)
    {
        if(playerData.upgradeLevel < 10)
        {
            playerData.upgradeLevel++;
        }

        SaveData();
        secondButton.interactable = false;
        secondFrame.SetActive(false);
        purchaseImage.SetActive(true);
        switch (id)
        {
            case 1:
                playerData.maxhealth += 10;
                break;

            case 2:
                playerData.speed += 1;
                break;

            case 3:
                playerData.attackDamage += 1;
                break;

            case 4:
                if (playerData.ultimateLevel == 3)
                {
                    Debug.Log("Reach Max Ult Level");
                    return;
                }

                else
                {
                    playerData.ultimateLevel += 1;
                }
                break;

        }
    }

    public void CloseSecondScreen()
    {
        secondFrame.SetActive(false);
        thisGO.interactable = true;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForActive();
    }
}
