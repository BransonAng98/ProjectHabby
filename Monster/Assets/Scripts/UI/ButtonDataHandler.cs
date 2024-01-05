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
    public bool secondFrameOn;
    public GameObject purchaseImage;
    public GameObject disableIcon;
    public GameObject purchaseVFXPf;
    public Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        upgradeCost = int.Parse(upgradeText.text);
        thisGO = GetComponent<Button>();
        purchaseImage.SetActive(false);
        disableIcon.SetActive(false);
        buttonState = PlayerPrefs.GetInt(buttonID);
        secondFrame.SetActive(false);
        CheckForActive();
    }

    void CheckForActive()
    {
        
        if(upgradeTier > playerData.upgradeLevel)
        {
            thisGO.interactable = false;    
            if(buttonState == 1)
            {
                disableIcon.SetActive(false);
                purchaseImage.SetActive(true);
            }
            else
            {
                disableIcon.SetActive(true);
            }
        }

        else
        {
            if (buttonState == 1)
            {
                disableIcon.SetActive(false);
                purchaseImage.SetActive(true);
                thisGO.interactable = false;
            }

            else
            {
                disableIcon.SetActive(false);
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
            secondFrameOn = true;
            secondFrame.SetActive(true);
            thisGO.interactable = false;
            CheckSecondButtonActive();
        }
    }

    public void CheckSecondButtonActive()
    {
        if (resourceData.currentGold >= upgradeCost)
        {
            secondButton.interactable = true;
        }

        else
        {
            secondButton.interactable = false;
        }
    }

    public void TriggerUpgrade(int id)
    {
        if(playerData.upgradeLevel < 10)
        {
            playerData.upgradeLevel++;
            PlayerPrefs.SetInt("PlayerUpgradeLevel", playerData.upgradeLevel);
        }

        SaveData();
        GameObject purchaseVFX = Instantiate(purchaseVFXPf, secondButton.transform.position, Quaternion.identity);
        purchaseVFX.transform.SetParent(canvas.transform, false);

        Destroy(purchaseVFX, 5f);
        resourceData.currentGold -= upgradeCost;
        PlayerPrefs.SetInt("PlayerMoney", resourceData.currentGold);
        secondButton.interactable = false;
        secondFrame.SetActive(false);
        purchaseImage.SetActive(true);
        disableIcon.SetActive(false);
        switch (id)
        {
            case 1:
                playerData.maxhealth += 10;
                PlayerPrefs.SetInt("PlayerHealth", playerData.maxhealth);
                break;

            case 2:
                playerData.speed += 1;
                PlayerPrefs.SetFloat("PlayerMovement", playerData.speed);
                break;

            case 3:
                playerData.attackDamage += 1;
                PlayerPrefs.SetFloat("PlayerAttackDamage", playerData.attackDamage);
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
                    PlayerPrefs.SetInt("PlayerUltimateLevel", playerData.ultimateLevel);
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

    private void OnDestroy()
    {
        PlayerPrefs.Save();
    }
}
