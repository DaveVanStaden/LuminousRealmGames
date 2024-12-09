using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FloatingRockInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float pushForce = 5f; // Force applied when the player hits the rock

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Keep the rock floating unless disturbed
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Ensure the player has the "Player" tag
        {
            Vector3 forceDirection = collision.contacts[0].normal * -1; // Push away from the point of contact
            rb.isKinematic = false; // Make the rock respond to physics
            rb.AddForce(forceDirection * pushForce, ForceMode.Impulse);
        }
    }
}
