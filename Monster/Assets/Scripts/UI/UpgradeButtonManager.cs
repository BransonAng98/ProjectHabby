using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButtonManager : MonoBehaviour
{
    public PlayerStatScriptableObject playerData;
    public UpgradeButtonManagerScriptableObject menuButtonData;
    public int priceCost;
    [SerializeField] bool canPurchase;

    // Start is called before the first frame update
    private void Awake()
    {
        CheckIfButtonIsActive();
    }

    void Start()
    {
    }

    void CheckIfButtonIsActive()
    {
        foreach(var buttons in menuButtonData.activatedButtons)
        {
            if (buttons != null)
            {
                buttons.interactable = false;
            }
        }
    }

    public void CheckForSufficientFunds(TextMeshProUGUI text)
    {
        priceCost = int.Parse(text.text);
        //Check if player has enough money to afford the upgrade
    }

    public void StoreButtonData(Button clickedButton)
    {
        if (canPurchase)
        {
            if (!menuButtonData.activatedButtons.Contains(clickedButton))
            {
                menuButtonData.activatedButtons.Add(clickedButton);
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
                    playerData.health += 10;
                    break;

                case 2:
                    playerData.speed += 1;
                    break;

                case 3:
                    playerData.attackDamage += 1;
                    break;

                case 4:
                    playerData.ultimateLevel += 1;
                    break;

            }
        }

        else
        {
            return;
        }
    }
}
