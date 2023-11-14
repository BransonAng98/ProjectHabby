using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUltimateButton : MonoBehaviour
{

    public PlayerHandler inputHandler;
    public Image radialFill;
    private Button button;
    public UltimateButtonScript ultButton;

    // Start is called before the first frame update
    void Start()
    {
        inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        button = GetComponent<Button>();
    }

    void CheckInteractivity()
    {
        if(inputHandler.currentUltimateCharge != inputHandler.playerData.maxUltimateCharge)
        {
            button.interactable = false;
        }

        else { button.interactable = true; }
    }

    public void ActivateUltimate()
    {
        if(inputHandler.currentUltimateCharge == inputHandler.playerData.maxUltimateCharge)
        {
            //inputHandler.ultimating = true;
        }
        else
        {
            return;
        }
    }

    //void UpdateFill()
    //{
    //    float simplifiedUltAmount = (inputHandler.currentUltimateCharge / inputHandler.playerSO.maxUltimateCharge) % 100.0f;
    //    radialFill.fillAmount = simplifiedUltAmount;
    //}

    private void Update()
    {
        CheckInteractivity();
    }
}
