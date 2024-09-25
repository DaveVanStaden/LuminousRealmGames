using UnityEngine;

public class ExploreMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;         
    [SerializeField] private float rotationSpeed = 300f;   
    [SerializeField] private float jumpForce = 5f;         
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerObj;
    [SerializeField] private Rigidbody rb;
    private bool isGrounded = true;

    // Increased damping for faster stopping
    [SerializeField] private float stopDamping = 10f;
    // Threshold to snap to a complete stop
    [SerializeField] private float stopThreshold = 0.1f;

    private Vector3 moveInput;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb.freezeRotation = true;
    }

    void Update()
    {
        HandleInput();
        HandleRotation();
        GroundCheck();
    }

    private void HandleInput()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        moveInput = new Vector3(moveHorizontal, 0, moveVertical).normalized;

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (moveInput.magnitude > 0)
        {
            // Calculate movement direction based on camera orientation
            Vector3 moveDirection = orientation.TransformDirection(moveInput) * moveSpeed;
            // Set linearVelocity directly for responsive movement
            rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
        }
        else
        {
            // Apply aggressive velocity damping to stop faster
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, stopDamping * Time.fixedDeltaTime);

            // If below threshold, force stop
            if (horizontalVelocity.magnitude < stopThreshold)
            {
                horizontalVelocity = Vector3.zero;  // Immediate stop
            }

            rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
        }
    }

    private void HandleRotation()
    {
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * moveVertical + orientation.right * moveHorizontal;

        if (inputDir != Vector3.zero)
        {
            playerObj.forward = Vector3.Lerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
    }

    private void GroundCheck()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f; 
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, 1.1f, groundLayer);
    }
}
