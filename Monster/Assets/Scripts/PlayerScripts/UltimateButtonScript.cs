using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UltimateButtonScript : MonoBehaviour
{
    public Transform player;
    public PlayerHandler playerHandler;
    public PlayerStatScriptableObject playerData;
    public PlayerVFXManager vfxManager;
    public bool ultimateReady;

    bool isSlowed;
    private Button button;

    //Double Tap
    [SerializeField] private float doubleTapThreshold;
    private float lastTapTime;
    private bool tapOccured;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        vfxManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerVFXManager>();
        button = GetComponent<Button>();
    }


    void CheckForActivation()
    {
        if(playerHandler.currentUltimateCharge == playerData.maxUltimateCharge)
        {
            ultimateReady = true;
            button.interactable = true;
            vfxManager.ultiRdyVFX.SetActive(true);
        }

        else
        {
            ultimateReady = false;
            button.interactable = false;
            vfxManager.ultiRdyVFX.SetActive(false);
        }
    }

    public void ActivateUltimate()
    {
        playerHandler.enableInput = false;
        if (playerHandler.currentUltimateCharge == playerHandler.playerData.maxUltimateCharge)
        {
            playerHandler.DisableMovement(0);
        }
    }

    public void TriggerDoubleTap()
    {
        //if (tapOccured && (Time.time - lastTapTime) < doubleTapThreshold)
        //{
        //    ActivateUltimate();
        //    tapOccured = false;
        //}
        //else
        //{
        //    tapOccured = true;
        //    lastTapTime = Time.time;
        //    Invoke(nameof(ResetTap), doubleTapThreshold);
        //}
    }

    private void ResetTap()
    {
        tapOccured = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            CheckForActivation();

            // Get the player's position in world space and convert it to screen space
            Vector3 playerPositionScreenSpace = Camera.main.WorldToScreenPoint(player.position);

            // Set the button's position to match the player's screen position
            transform.position = playerPositionScreenSpace;
        }
    }
}
