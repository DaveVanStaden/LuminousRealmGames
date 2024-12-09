using UnityEngine;

public class OrbitingRocks : MonoBehaviour
{
    [SerializeField] private Transform center; // The central magical crystal
    public float orbitRadius = 5f; // Radius of the circular motion
    public float orbitSpeed = 10f; // Speed of the orbit
    public float reverseTime = 5f; // Time in seconds to reverse the trajectory
    public float heightOffset = 0f; // Vertical offset for the rock's orbit
    public float initialAngleOffset = 0f; // Initial angle offset for the orbit

    private float angle = 0f; // Current angle of the rock around the circle
    private float direction = 1f; // Direction of the orbit (1 for clockwise, -1 for counter-clockwise)
    private float timer = 0f; // Timer to track when to reverse

    void Start()
    {
        // Initialize the starting angle with the offset
        angle = initialAngleOffset;
    }

    void Update()
    {
        if (center == null)
        {
            Debug.LogWarning("Center is not assigned!");
            return;
        }

        // Increment the angle based on orbit speed and direction
        angle += orbitSpeed * Time.deltaTime * direction;

        // Convert angle to radians for circular motion calculation
        float radian = angle * Mathf.Deg2Rad;

        // Calculate new position of the rock
        Vector3 newPosition = new Vector3(
            center.position.x + Mathf.Cos(radian) * orbitRadius,
            center.position.y + heightOffset,
            center.position.z + Mathf.Sin(radian) * orbitRadius
        );

        transform.position = newPosition;

        // Reverse direction after the specified time
        timer += Time.deltaTime;
        if (timer >= reverseTime)
        {
            direction *= -1; // Reverse direction
            timer = 0f; // Reset timer
        }
    }
}
