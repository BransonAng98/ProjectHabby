using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GNAManager : MonoBehaviour
{

    public ResourceScriptableObject goldData;
    public TextMeshProUGUI goldCounter;
    //public ScoreManagerScript scoremanager;

    // Start is called before the first frame update
    void Start()
    {
        //goldData.inGameGNA = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(goldCounter.text != null)
        {
            goldCounter.text = "" + goldData.currentGold;
        }

    }

    public void UpdateTotal()
    {
        //goldData.currentGNA += scoremanager.goldearned;
    }

    public void GainGNA(int amount)
    {
        //goldData.inGameGNA += amount;
    }
}
