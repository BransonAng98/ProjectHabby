using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButtonManager : MonoBehaviour
{
    public ResourceScriptableObject resourceData;
    public PlayerStatScriptableObject playerData;
    public UpgradeButtonManagerScriptableObject menuButtonData;
    public int priceCost;
    [SerializeField] bool canPurchase;

    // Start is called before the first frame update
    private void Awake()
    {
        CheckIfButtonIsActive();
    }

    void CheckIfButtonIsActive()
    {
        foreach(var buttons in menuButtonData.activatedButtons)
        {
            if (buttons != null)
            {
                Button chosenButton = buttons.GetComponent<Button>();
                if(chosenButton != null)
                {
                    chosenButton.interactable = false;
                }
            }
        }
    }

    public void CheckForSufficientFunds(TextMeshProUGUI text)
    {
        priceCost = int.Parse(text.text);
        //Check if player has enough money to afford the upgrade
        if(resourceData.currentGold >= priceCost)
        {
            canPurchase = true;
            resourceData.currentGold -= priceCost;
        }

        else
        {
            canPurchase = false;
            Debug.Log("Not enough cash poor fool!");
            return;
        }
    }

    public void StoreButtonData(GameObject clickedButton)
    {
        if (canPurchase)
        {
            if (!menuButtonData.activatedButtons.Contains(clickedButton))
            {
                menuButtonData.activatedButtons.Add(clickedButton);
                Button chosenButton = clickedButton.GetComponent<Button>();
                if (chosenButton != null)
                {
                    chosenButton.interactable = false;
                }
            }

            else
            {
                return;
            }
        }

        else
        {
            return;
        }
    }

    public void TriggerUpgrade(int id)
    {
        if (canPurchase)
        {
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
                    if(playerData.ultimateLevel == 3)
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

        else
        {
            return;
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            playerData.maxhealth = 150;
            playerData.speed = 7;
            playerData.attackDamage = 5;
            playerData.ultimateLevel = 2;
            menuButtonData.activatedButtons.Clear();
        }
    }
}
