using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveIndicator : MonoBehaviour
{
    public GameObject thiefParent;
    public RectTransform eggIcon;
    public float maxDist;
    public float scaleFactor = 5f;
    public Vector2 minScale = new Vector2(0.1f, 0.1f);
    public Vector2 maxScale = new Vector2(3f, 3f);

    [SerializeField] public float distDiff;
    private PlayerEndlessRunnerController runnerController;
    private Thief thiefController;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        runnerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEndlessRunnerController>();
        thiefController = GameObject.FindGameObjectWithTag("Thief").GetComponent<Thief>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateDist();
        TurnOffIcon();
    }

    void CalculateDist()
    {
        // Calculate the distance between enemy and player
        distDiff = thiefController.distanceTravelled - runnerController.distanceTravelled;

        // Map the distance to a scale value between 0 and 1
        float normalizedDistance = Mathf.Clamp01(distDiff / thiefController.distanceTravelled) * scaleFactor;

        // Interpolate between the min and max scale based on the normalized distance
        Vector2 targetScale = Vector2.Lerp(minScale, maxScale, 1f - normalizedDistance); // Inverse lerp since we want the UI to scale up as distance decreases

        // Apply the target scale to the UI image
        eggIcon.localScale = targetScale;
    }

    void TurnOffIcon()
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(thiefParent.transform.position);
        bool isVisible = viewportPosition.x >= 0 && viewportPosition.x <= 1 && viewportPosition.y >= 0 && viewportPosition.y <= 1 && viewportPosition.z >= 0;

        // Toggle the entity's visibility
        eggIcon.gameObject.SetActive(isVisible);
    }
}
