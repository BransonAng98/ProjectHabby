using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetUpgradePurchase : MonoBehaviour
{
    public PlayerStatScriptableObject playerData;
    public ResourceScriptableObject resourceData;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ResetUpgrade()
    {
        playerData.upgradeLevel = 1;
        PlayerPrefs.DeleteAll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
