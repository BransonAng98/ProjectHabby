using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "UIManagerScriptableObject", menuName = "ScriptableObject/UI")]
public class UpgradeButtonManagerScriptableObject : ScriptableObject
{
    public List<GameObject> activatedButtons = new List<GameObject>();
}
