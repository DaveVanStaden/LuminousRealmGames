using UnityEngine;

public class PlayerControllerVersionOne : MonoBehaviour
{
    [Header("Movement Settings")] // Group for movement-related parameters
    public float normalSpeed = 5f;
    public float dyingSpeed = 2f;
    public float sprintSpeed = 8f;
    public float jumpForce = 5f;
    public float chargedJumpForce = 10f;
    public float glideSpeed = 2f;
    public float glideFallSpeed = 1f;

    [Header("State Durations")] // Group for state duration settings
    public float dyingStateDuration = 10f; // Time spent in "Dying" state
    public float powerUpDuration = 10f;   // Duration of "Power-Up" state

    // Private variables
    private float stateTimer;             // Timer to track state durations
    private Rigidbody rb;                 // Reference to the Rigidbody component
    private bool isGrounded;              // Whether the player is on the ground
    private bool isChargingJump;          // Whether the player is charging a jump
    private bool isGliding;               // Whether the player is gliding
    private float chargedJumpCharge;      // Accumulated charge for a jump

    // Player state enumeration
    public enum PlayerState { Dying, PowerUp }
    private PlayerState currentState;

    void Start()
    {
        rb = GetComponent<Rigidbody>();   // Initialize Rigidbody
        SwitchToState(PlayerState.Dying); // Start in the "Dying" state
    }

    void Update()
    {
        HandleStateTimers();              // Update state timer and switch states if needed

        // Handle behavior based on the current state
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

    // Handles behavior in the "Dying" state
    void HandleDyingState()
    {
        Move(dyingSpeed); // Move at reduced speed

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump(jumpForce); // Perform a normal jump
        }

        // Charging and executing a charged jump
        if (Input.GetButton("Jump"))
        {
            isChargingJump = true;
            chargedJumpCharge += Time.deltaTime; // Increase charge over time
        }

        if (Input.GetButtonUp("Jump") && isChargingJump)
        {
            Jump(Mathf.Min(chargedJumpForce, chargedJumpCharge * chargedJumpForce)); // Perform charged jump
            isChargingJump = false;
            chargedJumpCharge = 0f; // Reset charge
        }
    }

    // Handles behavior in the "Power-Up" state
    void HandlePowerUpState()
    {
        // Allow sprinting or normal movement
        float moveSpeed = Input.GetButton("Sprint") ? sprintSpeed : normalSpeed;
        Move(moveSpeed);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump(jumpForce); // Perform a normal jump
        }

        // Handle gliding while in the air
        if (Input.GetButton("Glide") && !isGrounded)
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
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calculate and apply movement
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical) * speed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);
    }

    // Applies a vertical force for jumping
    void Jump(float force)
    {
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        isGrounded = false;
    }

    // Applies forces to simulate gliding behavior
    void Glide()
    {
        isGliding = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, -glideFallSpeed, rb.linearVelocity.z);
        rb.AddForce(transform.forward * glideSpeed, ForceMode.Force);
    }

    // Manages state transitions based on the timer
    void HandleStateTimers()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0)
        {
            if (currentState == PlayerState.Dying)
            {
                Debug.Log("Player has died."); // Handle death scenario
            }
            else if (currentState == PlayerState.PowerUp)
            {
                Debug.Log("Power-Up has ended. Switching to Dying State.");
                SwitchToState(PlayerState.Dying); // Transition back to "Dying" state
            }
        }
    }

    // Switches to a specified player state
    void SwitchToState(PlayerState newState)
    {
        currentState = newState;

        // Set appropriate timer based on the state
        stateTimer = newState == PlayerState.Dying ? dyingStateDuration : powerUpDuration;
    }

    // Handles collision events
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // Mark player as grounded
        }
    }

    // Reduces state timer when the player takes damage
    public void TakeDamage(float damage)
    {
        stateTimer -= damage;

        if (stateTimer <= 0 && currentState == PlayerState.PowerUp)
        {
            Debug.Log("Power-Up ended due to damage. Switching to Dying State.");
            SwitchToState(PlayerState.Dying); // Transition to "Dying" state
        }
    }

    // Activates the "Power-Up" state
    public void ActivatePowerUp()
    {
        SwitchToState(PlayerState.PowerUp);
    }
}
