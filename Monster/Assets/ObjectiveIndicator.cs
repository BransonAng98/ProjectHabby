using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveIndicator : MonoBehaviour
{
    public GameObject thiefParent;
    public RectTransform eggIcon;

    [SerializeField] public float distDiff;
    private Thief thiefController;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        thiefController = GameObject.FindGameObjectWithTag("Thief").GetComponent<Thief>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        TurnOffIcon();
    }

    void TurnOffIcon()
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(thiefController.transform.position);
        bool isVisible = viewportPosition.x >= 0 && viewportPosition.x <= 1 && viewportPosition.y >= 0 && viewportPosition.y <= 1 && viewportPosition.z >= 0;

        // Toggle the visibility of objectToToggle based on whether objectToCheck is visible
        eggIcon.gameObject.SetActive(!isVisible);
    }
}
