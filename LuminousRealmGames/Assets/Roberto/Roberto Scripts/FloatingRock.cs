using UnityEngine;

public class FloatingRock : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Floating Settings")]
    public float floatSpeed = 1f; // Speed of the floating motion
    public float floatHeight = 0.5f; // Maximum height of the floating motion

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position; // Store the starting position
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
