using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Tooltip("Time before the platform starts falling after being touched by the player.")]
    public float delayBeforeFall = 1.0f;

    [Tooltip("Material when the platform is touched.")]
    public Material touchedMaterial;  // Assign in Inspector

    [Tooltip("Material when the platform starts falling.")]
    public Material fallingMaterial;  // Assign in Inspector

    private bool playerTouched = false;
    private Renderer platformRenderer;
    private Rigidbody rb;

    void Start()
    {
        platformRenderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;  // Disable gravity initially so the platform doesn't fall
        rb.isKinematic = true;  // Set to kinematic to prevent physics interactions
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !playerTouched)
        {
            playerTouched = true;
            platformRenderer.material = touchedMaterial;  // Change to touched material
            StartCoroutine(FallAfterDelay());
        }
    }

    IEnumerator FallAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeFall);  // Wait for the delay

        platformRenderer.material = fallingMaterial;  // Change to falling material
        rb.isKinematic = false;  // Disable kinematic to allow falling
        rb.useGravity = true;  // Enable gravity after the delay
    }
}
