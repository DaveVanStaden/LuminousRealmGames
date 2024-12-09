using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorFilterTrigger : MonoBehaviour
{
    public Volume volume;               // Assign the volume in the inspector
    public Color targetColor = Color.blue; // The color to transition to
    public float transitionDuration = 2f; // Time for the transition in seconds

    private ColorAdjustments colorAdjustments;
    private Color originalColor;        // The original color to revert to
    private bool inTriggerZone = false; // Whether the player is in the trigger zone
    private float transitionProgress = 0f; // Tracks the progress of the transition (0 to 1)
    private bool isTransitioning = false;

    void Start()
    {
        // Get the Color Adjustments effect from the Volume profile
        if (volume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            originalColor = colorAdjustments.colorFilter.value; // Save the initial color
        }
        else
        {
            Debug.LogWarning("Color Adjustments not found in Volume profile.");
        }
    }

    void Update()
    {
        if (colorAdjustments != null && isTransitioning)
        {
            // Update the transition progress
            float direction = inTriggerZone ? 1 : -1; // Forward or backward
            transitionProgress += direction * Time.deltaTime / transitionDuration;

            // Clamp progress to [0, 1] range
            transitionProgress = Mathf.Clamp01(transitionProgress);

            // Interpolate between original and target colors based on progress
            colorAdjustments.colorFilter.value = Color.Lerp(originalColor, targetColor, transitionProgress);

            // Stop transitioning if we've reached the end of the transition
            if (transitionProgress == 0f || transitionProgress == 1f)
            {
                isTransitioning = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure this is the player
        {
            inTriggerZone = true;
            isTransitioning = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inTriggerZone = false;
            isTransitioning = true;
        }
    }
}