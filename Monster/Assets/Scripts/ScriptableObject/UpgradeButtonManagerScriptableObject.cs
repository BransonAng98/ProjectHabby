using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "UIManagerScriptableObject", menuName = "ScriptableObject/UI")]
public class UpgradeButtonManagerScriptableObject : ScriptableObject
{
    public List<string> buttonNames = new List<string>();
}
