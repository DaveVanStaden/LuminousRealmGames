using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerVersionTwo : MonoBehaviour
{
    [Header("Movement Settings")] // Movement parameters
    public float normalSpeed = 5f;
    public float dyingSpeed = 2f;
    public float sprintSpeed = 8f;
    public float jumpForce = 5f;
    public float chargedJumpForce = 10f;
    public float glideSpeed = 2f;
    public float glideFallSpeed = 1f;

    [Header("State Durations")] // Duration parameters for different states
    public float dyingStateDuration = 10f;
    public float powerUpDuration = 10f;

    // === Private Variables ===
    private float stateTimer;             // Tracks remaining time in current state
    private Rigidbody rb;                 // Reference to the player's Rigidbody
    private bool isGrounded;              // True when the player is on the ground
    private bool isChargingJump;          // True while charging a jump
    private bool isGliding;               // True while gliding
    private float chargedJumpCharge;      // Accumulated charge for charged jump

    private Vector2 moveInput;            // Stores movement input from the player
    private bool isSprinting;             // True when the sprint button is held
    private bool jumpPressed;             // True when the jump button is pressed
    private bool glidePressed;            // True when the glide button is pressed

    public enum PlayerState { Dying, PowerUp } // Player states
    private PlayerState currentState;

    private PlayerInput playerInput;      // Reference to the PlayerInput component

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        SwitchToState(PlayerState.Dying); // Start in the "Dying" state
    }

    void Update()
    {
        HandleStateTimers(); // Handle transitions based on the state timer

        // Execute behavior based on the current state
        switch (currentState)
        {
            case PlayerState.Dying:
                HandleDyingState();
                break;

            case PlayerState.PowerUp:
                HandlePowerUpState();
                break;
        }
    }

    // Handles player behavior in the "Dying" state
    void HandleDyingState()
    {
        Move(dyingSpeed); // Move at reduced speed

        // Perform a regular jump if grounded
        if (jumpPressed && isGrounded)
        {
            Jump(jumpForce);
        }

        // Charge and release a charged jump
        if (jumpPressed)
        {
            isChargingJump = true;
            chargedJumpCharge += Time.deltaTime; // Accumulate jump charge over time
        }

        if (!jumpPressed && isChargingJump)
        {
            Jump(Mathf.Min(chargedJumpForce, chargedJumpCharge * chargedJumpForce)); // Execute charged jump
            isChargingJump = false;
            chargedJumpCharge = 0f; // Reset jump charge
        }
    }

    // Handles player behavior in the "Power-Up" state
    void HandlePowerUpState()
    {
        // Move at sprint speed or normal speed based on input
        float moveSpeed = isSprinting ? sprintSpeed : normalSpeed;
        Move(moveSpeed);

        // Perform a jump if grounded
        if (jumpPressed && isGrounded)
        {
            Jump(jumpForce);
        }

        // Glide while in the air
        if (glidePressed && !isGrounded)
        {
            Glide();
        }
        else if (isGliding)
        {
            isGliding = false; // Stop gliding if the button is released
        }
    }

    // Generic movement logic
    void Move(float speed)
    {
        Vector3 movement = new Vector3(moveInput.x, 0.0f, moveInput.y) * speed * Time.deltaTime;
        rb.MovePosition(transform.position + movement); // Update player position
    }

    // Apply upward force for jumping
    void Jump(float force)
    {
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        isGrounded = false; // Player is no longer grounded after jumping
    }

    // Apply forces to simulate gliding
    void Glide()
    {
        isGliding = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, -glideFallSpeed, rb.linearVelocity.z); // Control vertical fall speed
        rb.AddForce(transform.forward * glideSpeed, ForceMode.Force); // Apply forward gliding force
    }

    // Manage state transitions based on the timer
    void HandleStateTimers()
    {
        stateTimer -= Time.deltaTime; // Decrease timer each frame

        if (stateTimer <= 0)
        {
            if (currentState == PlayerState.Dying)
            {
                Debug.Log("Player has died."); // Log player death
            }
            else if (currentState == PlayerState.PowerUp)
            {
                Debug.Log("Power-Up has ended. Switching to Dying State.");
                SwitchToState(PlayerState.Dying); // Transition to "Dying" state
            }
        }
    }

    // Switch between player states and set timers
    void SwitchToState(PlayerState newState)
    {
        currentState = newState;

        // Set timer based on the new state
        stateTimer = newState == PlayerState.Dying ? dyingStateDuration : powerUpDuration;
    }

    // Detect collisions to determine if the player is grounded
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // Apply damage to reduce state timer
    public void TakeDamage(float damage)
    {
        stateTimer -= damage;

        // Transition to "Dying" state if Power-Up ends due to damage
        if (stateTimer <= 0 && currentState == PlayerState.PowerUp)
        {
            Debug.Log("Power-Up ended due to damage. Switching to Dying State.");
            SwitchToState(PlayerState.Dying);
        }
    }

    // Activate the "Power-Up" state
    public void ActivatePowerUp()
    {
        SwitchToState(PlayerState.PowerUp);
    }

    // === Input Action Handlers ===
    // Handle movement input
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Handle sprint input
    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.performed;
    }

    // Handle jump input
    public void OnJump(InputAction.CallbackContext context)
    {
        jumpPressed = context.performed;
    }

    // Handle glide input
    public void OnGlide(InputAction.CallbackContext context)
    {
        glidePressed = context.performed;
    }
}
