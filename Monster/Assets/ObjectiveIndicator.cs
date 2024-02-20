using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveIndicator : MonoBehaviour
{
    public GameObject thiefParent;
    public RectTransform eggIcon;
    public float maxDist = 10f;
    public Vector2 minScale;
    public Vector2 maxScale;

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
        float normalizedDistance = Mathf.Clamp01(distDiff / thiefController.distanceTravelled);

        // Define an additional speed multiplier to control how fast the UI scales
        float speedMultiplier = 5f; // Adjust this value to control the speed of scaling

        // Invert the normalized distance to reverse the scaling direction
        normalizedDistance = 1f - normalizedDistance;

        // Interpolate between the min and max scale based on the inverted normalized distance, with increased speed
        Vector2 targetScale = Vector2.Lerp(minScale, maxScale, Mathf.Pow(normalizedDistance, speedMultiplier));

        // Apply the target scale to the UI image
        eggIcon.localScale = targetScale;
    }

    void TurnOffIcon()
    {
        // Check if the entity's position is inside the camera's view
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(thiefParent.transform.position);
        bool isVisible = viewportPosition.x >= 0 && viewportPosition.x <= 1 && viewportPosition.y >= 0 && viewportPosition.y <= 1 && viewportPosition.z >= 0;

        // Toggle the entity's visibility
        eggIcon.gameObject.SetActive(!isVisible); // Toggle the visibility based on whether the object is outside the view
    }
}
