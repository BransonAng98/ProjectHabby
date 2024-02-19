using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ERScoreManager : MonoBehaviour
{
    public float DistanceTravelled;
    public float DistanceToTarget;
    public TextMeshProUGUI DistanceDisplay;
    public TextMeshProUGUI TargetDistanceDisplay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DistanceDisplay.text = ""+ DistanceTravelled + "M";
        TargetDistanceDisplay.text = DistanceToTarget + "M";

    }
}
