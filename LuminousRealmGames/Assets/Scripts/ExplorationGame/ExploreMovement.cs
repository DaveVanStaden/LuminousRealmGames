using UnityEngine;

public class ExploreMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;     
    [SerializeField] private float sprintSpeed = 10f;  
    [SerializeField] private float rotationSpeed = 300f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerObj;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundCheck;
    private bool isGrounded = true;
    
// Increased damping for faster stopping
    [SerializeField] private float stopDamping = 10f;
// Threshold to snap to a complete stop
    [SerializeField] private float stopThreshold = 0.1f;

    [SerializeField] private float minJumpForce = 5f;   // Minimum jump force
    [SerializeField] private float maxJumpForce = 15f;  // Maximum jump force
    [SerializeField] private float maxChargeTime = 2f;  // Max time to fully charge the jump
    private float jumpChargeTime = 0f;                  // Time spent charging the jump
    private bool isChargingJump = false;       
// Glide variables
    [SerializeField] private float glideFallSpeed = 1f;  // How fast the player falls while gliding
    [SerializeField] private bool isGliding = false;  // Check if player is gliding

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
    }
    void FixedUpdate()
    {
        HandleMovement();
        GroundCheck();
    }

    private void HandleInput()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        moveInput = new Vector3(moveHorizontal, 0, moveVertical).normalized;

        // Start charging the jump if grounded and jump button is pressed
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isChargingJump = true;
        }

        // Continue charging while holding the jump button
        if (Input.GetButton("Jump") && isChargingJump)
        {
            jumpChargeTime += Time.deltaTime;
            jumpChargeTime = Mathf.Clamp(jumpChargeTime, 0f, maxChargeTime);  // Limit charge time
        }

        // Release the jump button to perform the jump
        if (Input.GetButtonUp("Jump") && isChargingJump && isGrounded)
        {
            PerformJump();
            isChargingJump = false;  // Reset charging state after jump
        }
    }
    private void PerformJump()
    {
        // Calculate jump force based on how long the player charged
        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, jumpChargeTime / maxChargeTime);

        // Apply the calculated jump force
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // Reset charge time after jumping
        jumpChargeTime = 0f;
    }

    private void HandleMovement()
    {
        // Check if sprinting
        float currentMoveSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        if (moveInput.magnitude > 0)
        {
            // Move the player based on input and sprint
            Vector3 moveDirection = orientation.TransformDirection(moveInput) * currentMoveSpeed;
            rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
        }
        else
        {
            // Smoothly stop the player's movement when not pressing keys
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, stopDamping * Time.fixedDeltaTime);

            if (horizontalVelocity.magnitude < stopThreshold)
            {
                horizontalVelocity = Vector3.zero;  // Snap to stop when slow enough
            }

            rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
        }

        // Glide logic: activate only when in air and no longer grounded
        if (!isGrounded && rb.linearVelocity.y < 0)
        {
            if (Input.GetButton("Jump"))  // Holding the jump button triggers the glide
            {
                StartGlide();
            }
            else
            {
                StopGlide();  // Stop gliding when the button is released
            }
        }

        // Adjust the fall speed while gliding
        if (isGliding)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -glideFallSpeed, rb.linearVelocity.z);
        }
    }
    private void StartGlide()
    {
        isGliding = true;
        // Set the fall speed to the glide speed
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, -glideFallSpeed, rb.linearVelocity.z);
    }
    private void StopGlide()
    {
        isGliding = false;
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
        Vector3 rayOrigin = groundCheck.position;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, 0.2f, groundLayer);
        
        if (isGrounded)
        {
            isGliding = false;  // Stop gliding when grounded
        }
    }
}
