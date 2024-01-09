using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveDisplay : MonoBehaviour
{

    public TextMeshProUGUI text;
    public MiniGameLandmark landmark;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        landmark = GameObject.FindGameObjectWithTag("Landmark").GetComponent<MiniGameLandmark>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(landmark != null)
        {
            text.text = "Tap To Destroy the " + landmark.enemyData.enemyName;
        }

        else
        {
            text.text = "Objective completed!";
        }
    }
}
