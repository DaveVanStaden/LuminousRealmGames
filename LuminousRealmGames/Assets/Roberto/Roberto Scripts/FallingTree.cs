using UnityEngine;

public class FallingTree : MonoBehaviour
{
    [Tooltip("The speed at which the tree falls.")]
    public float fallSpeed = 30f;

    [Tooltip("The angle at which the tree is considered 'fallen'.")]
    public float fallAngle = 90f;

    private bool isFalling = false;

    void Update()
    {
        if (isFalling)
        {
            // Rotate the tree along the x-axis to simulate falling
            transform.Rotate(Vector3.right * fallSpeed * Time.deltaTime);

            // Check if the tree has reached the desired fall angle
            if (transform.eulerAngles.x >= fallAngle)
            {
                isFalling = false; // Stop falling after reaching the angle
                // Stop the rotation exactly at the angle
                Vector3 currentRotation = transform.eulerAngles;
                currentRotation.x = fallAngle;
                transform.eulerAngles = currentRotation;
            }
        }
    }

    // Call this method to trigger the falling effect
    public void StartFalling()
    {
        isFalling = true;
    }
}
