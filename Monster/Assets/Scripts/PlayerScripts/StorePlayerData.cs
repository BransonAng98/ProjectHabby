using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorePlayerData : MonoBehaviour
{
    private void OnDestroy()
    {
        //Saves the value of player whenever they leave the upgrade scene
        PlayerPrefs.Save();
    }
}
