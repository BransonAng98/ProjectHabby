using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityDisplayScript : MonoBehaviour
{
    public GameObject rushText;
    public GameObject smashText;

    public Canvas canvas;
   public void UsingAbility1()
    {
        GameObject spawnText = Instantiate(rushText);
        spawnText.transform.SetParent(canvas.transform, false);
    }

    public void UsingAbility2()
    {
        GameObject spawnText = Instantiate(smashText);
        spawnText.transform.SetParent(canvas.transform, false);
    }
}
