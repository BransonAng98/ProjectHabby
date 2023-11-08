using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeItem : MonoBehaviour
{

    [SerializeField] private ObjectFadeEffect fadeEffect;
    // Start is called before the first frame update

    private void Awake()
    {
        fadeEffect = GetComponent<ObjectFadeEffect>();
    }

    void Start()
    {
        fadeEffect.StartFading();   
    }
}
