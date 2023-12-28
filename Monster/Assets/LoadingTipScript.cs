using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingTipScript : MonoBehaviour
{
    public TextMeshProUGUI tips;
    public List<string> tipList = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        RandomizeMessage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void RandomizeMessage()
    {
        int random = Random.Range(0, 4 + 1);
        tips.text = tipList[random];
    }
}
