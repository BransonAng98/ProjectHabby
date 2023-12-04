using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIScript : MonoBehaviour
{
    public Transform player;

    // Update is called once per frame
    void Update()
    {
        PositionUI();
    }

    void PositionUI()
    {
        Vector2 uiPos = new Vector2(player.transform.position.x, player.transform.position.y + 5.5f);
        // Get the player's position in world space and convert it to screen space
        Vector3 playerPositionScreenSpace = Camera.main.WorldToScreenPoint(uiPos);

        // Set the button's position to match the player's screen position
        transform.position = playerPositionScreenSpace;
    }
}
