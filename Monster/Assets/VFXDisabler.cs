using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXDisabler : MonoBehaviour
{
    [SerializeField] private float disableTime = 1f;

    private void OnEnable()
    {
        Invoke(nameof(DisableObj), disableTime);
    }

    void DisableObj()
    {
        gameObject.SetActive(false);
    }
}
