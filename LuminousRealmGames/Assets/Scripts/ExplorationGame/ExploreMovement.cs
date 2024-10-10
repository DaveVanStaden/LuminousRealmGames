using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    [SerializeField] private float rotationSpeed = 300f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerObj;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private InputManager inputManager; // Reference to the InputManager

    private bool isGrounded = true;
    private bool hasWallJumped = false;
    private int jumpCount = 0;
    private int maxJumps = 2;

    [SerializeField] private float stopDamping = 10f;
    [SerializeField] private float stopThreshold = 0.1f;

    [SerializeField] private float minJumpForce = 5f;
    [SerializeField] private float maxJumpForce = 15f;
    [SerializeField] private float maxChargeTime = 2f;
    private float jumpChargeTime = 0f;
    private bool isChargingJump = false;

    [SerializeField] private float glideFallSpeed = 1f;
    private bool isGliding = false;

    [SerializeField] private float adrenalineSpeed = 15f; 
    [SerializeField] private float adrenalineDuration = 2f; 
    private bool canUseAdrenaline = false; 
    private bool isAdrenalineActive = false; 

    private Vector3 moveInput;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb.freezeRotation = true;
    }

    void Update()
    {
        AdjustSpeedBasedOnState();
        HandleInput();
        HandleRotation();
    }

    void FixedUpdate()
    {
        HandleMovement();
        GroundCheck();
        WallCheck();
    }

    private void AdjustSpeedBasedOnState()
    {
        if (playerHealth.currentState == PlayerHealth.PlayerState.Healed)
        {
            moveSpeed = 5f; // Set to a normal speed when healed
            sprintSpeed = 10f; // Adjust sprint speed when healed
            canUseAdrenaline = true; // Enable adrenaline usage again when healed
        }
        else if (playerHealth.currentState == PlayerHealth.PlayerState.Injured)
        {
            moveSpeed = 3f; // Set to a slower speed when injured
            sprintSpeed = 6f; // Decrease sprint speed when injured
        }
    }

    private void HandleInput()
    {
        Vector2 input = inputManager.GetMoveInput();
        moveInput = new Vector3(input.x, 0, input.y).normalized;

        // Start charging the jump if grounded
        if (isGrounded && inputManager.GetJumpInputDown())
        {
            isChargingJump = true;
            jumpChargeTime = 0f;
        }

        // Charge the jump while holding the button
        if (isChargingJump && inputManager.GetJumpInput())
        {
            jumpChargeTime += Time.deltaTime;
            jumpChargeTime = Mathf.Clamp(jumpChargeTime, 0f, maxChargeTime);
        }

        // Release the jump button to perform the charged jump
        if (isChargingJump && !inputManager.GetJumpInput())
        {
            PerformJump(); // Apply the charged jump force for the first jump
            isChargingJump = false;
        }

        // Handle double jump or wall jump logic only if the player is healed
        if (playerHealth.currentState == PlayerHealth.PlayerState.Healed)
        {
            // Allow double jump when not grounded
            if (!isGrounded && inputManager.GetJumpInputDown() && jumpCount < maxJumps && !hasWallJumped)
            {
                PerformJump(); // Normal jump for wall jumping or double jump
                jumpCount++;
            }
        }

        // Handle Adrenaline Input
        if (playerHealth.currentState == PlayerHealth.PlayerState.Injured && 
            canUseAdrenaline && 
            inputManager.GetSprintInput())
        {
            // Activate Adrenaline Boost
            StartCoroutine(AdrenalineBoost());
        }
        else if (playerHealth.currentState == PlayerHealth.PlayerState.Healed)
        {
            // Reset canUseAdrenaline when healed
            canUseAdrenaline = true; 
        }
    }
    private IEnumerator AdrenalineBoost()
    {
        canUseAdrenaline = false; // Disable further adrenaline usage
        isAdrenalineActive = true; // Set the flag to indicate that adrenaline is active
        float originalMoveSpeed = moveSpeed; // Save the original speed
        moveSpeed = adrenalineSpeed; // Apply the adrenaline speed

        yield return new WaitForSeconds(adrenalineDuration); // Wait for the duration of the boost

        // Reset speed back to the original
        moveSpeed = originalMoveSpeed; 
        isAdrenalineActive = false; // Reset the adrenaline active flag
    }

    public void ResetAdrenaline()
    {
        // Only reset when healed
        isAdrenalineActive = false; // Deactivate adrenaline boost
        AdjustSpeedBasedOnState(); // Call the existing method to adjust speeds
    }
    private void PerformJump()
    {
        // Calculate the jump force based on the charging time
        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, jumpChargeTime / maxChargeTime);

        // Reset the vertical velocity before applying the jump force to avoid stacking forces
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Apply the calculated jump force
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // Reset jump charge variables
        jumpChargeTime = 0f;
        isChargingJump = false;
        hasWallJumped = false;

        // Allow gliding only if the player is healed
        if (playerHealth.currentState == PlayerHealth.PlayerState.Healed)
        {
            isGliding = false;
        }

        // Increment jump count if the player is not grounded
        if (!isGrounded)
        {
            jumpCount++;
        }
    }

    private void HandleMovement()
    {
        float currentMoveSpeed = moveSpeed; // Default to moveSpeed

        // If adrenaline is active, use the adrenaline speed
        if (isAdrenalineActive) // Check if adrenaline is currently active
        {
            currentMoveSpeed = adrenalineSpeed; // Set to adrenaline speed during boost
        }
        else if (inputManager.GetSprintInput() && playerHealth.currentState == PlayerHealth.PlayerState.Healed)
        {
            currentMoveSpeed = sprintSpeed; // Normal sprinting when healed
        }

        if (moveInput.magnitude > 0)
        {
            Vector3 moveDirection = orientation.TransformDirection(moveInput) * currentMoveSpeed;
            rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
        }
        else
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, stopDamping * Time.fixedDeltaTime);

            if (horizontalVelocity.magnitude < stopThreshold)
            {
                horizontalVelocity = Vector3.zero;
            }

            rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
        }

        // Only allow gliding if the player is healed
        if (!isGrounded && rb.linearVelocity.y < 0 && playerHealth.currentState == PlayerHealth.PlayerState.Healed)
        {
            if (inputManager.GetJumpInput())
            {
                StartGlide();
            }
            else
            {
                StopGlide();
            }
        }

        if (isGliding)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -glideFallSpeed, rb.linearVelocity.z);
        }
    }

    private void StartGlide()
    {
        isGliding = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, -glideFallSpeed, rb.linearVelocity.z);
    }

    private void StopGlide()
    {
        isGliding = false;
    }

    private void GroundCheck()
    {
        Vector3 rayOrigin = groundCheck.position;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, 0.4f, groundLayer);

        if (isGrounded)
        {
            isGliding = false;
            jumpCount = 0;
            hasWallJumped = false;
        }
    }

    private void WallCheck()
    {
        if (!isGrounded)
        {
            Vector3[] directions = { transform.forward, -transform.forward, transform.right, -transform.right };
            float wallCheckDistance = 0.5f;

            foreach (var direction in directions)
            {
                if (Physics.Raycast(wallCheck.position, direction, wallCheckDistance, wallLayer))
                {
                    if (!hasWallJumped && playerHealth.currentState == PlayerHealth.PlayerState.Healed)
                    {
                        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                        PerformJump();
                        jumpCount++;
                        hasWallJumped = true;
                        break;
                    }
                }
            }
        }
    }

    private void HandleRotation()
    {
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        Vector2 input = inputManager.GetMoveInput();
        Vector3 inputDir = new Vector3(input.x, 0, input.y);
        if (inputDir != Vector3.zero)
        {
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
    }
}