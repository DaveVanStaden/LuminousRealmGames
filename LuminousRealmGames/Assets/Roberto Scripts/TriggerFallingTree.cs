using UnityEngine;

public class FallingTreeTrigger : MonoBehaviour
{
    public Rigidbody[] fallingTrees; // Assign the falling tree Rigidbody components in the Inspector

    void OnTriggerEnter(Collider other)
    {
        // Log the name of the object that entered the trigger
        Debug.Log($"{other.name} entered the trigger.");

        // Check if the object that entered the trigger has the tag "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected! Activating falling trees.");

            foreach (Rigidbody tree in fallingTrees)
            {
                if (tree != null)
                {
                    tree.useGravity = true; // Enable gravity on the falling trees
                    // Optional: If you want to make the tree fall immediately, you can add a force
                    tree.AddForce(Vector3.down * 10f, ForceMode.Impulse); // Adjust the force as needed

                    Debug.Log($"{tree.gameObject.name} is now falling.");
                }
            }
        }
    }
}
