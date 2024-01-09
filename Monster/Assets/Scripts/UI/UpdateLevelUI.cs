using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateLevelUI : MonoBehaviour
{
    TextMeshProUGUI levelIndicator;
    public LevelManagerScriptableObject levelData;
    public PlayerStatScriptableObject playerData;

    // Start is called before the first frame update
    void Start()
    {
        levelIndicator = GetComponent<TextMeshProUGUI>();
        levelIndicator.text = "France - " + playerData.levelProgress.ToString();
    }
}
