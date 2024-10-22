using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    private PlayerInputManager inputManager;
    private PlayerManager playerManager;
    private AnimatorManager animatorManager;
    
    private Vector3 moveDirection;
    private Transform cameraObject;
    private Rigidbody playerRB;

    [Header("Falling")] 
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffSet = 0.5f;
    public LayerMask groundLayer;
    
    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;
    public bool isCrouching;

    [Header("Movement Speeds")]
    [SerializeField] private float runningSpeed = 5f;
    [SerializeField] private float walkingSpeed = 1.5f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float rotationSpeed = 15;
    [SerializeField] private float crouchSpeed = 0.5f;

    [Header("Adrenaline Variables")] 
    [SerializeField] private float adrenalineDuration = 2f;
    [SerializeField] private float adrenalineSpeed = 15f;
    public bool canUseAdrenaline = true;
    public bool isAdrenalineActive;

    [Header("Jump Speeds")] 
    [SerializeField] private float gravityIntensity = -5f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float wallJumpOffForce = 5f;
    
    [Header("Jump Settings")]
    [SerializeField] private int maxJumps = 2; 
    private int currentJumpCount = 0;
    
    [Header("Gliding Settings")]
    [SerializeField] private float glideFallSpeed = 0.2f; 
    public bool isGliding = false;
    [Header("Crouch Settings")] 
    private CapsuleCollider collider;
    
    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerManager = GetComponent<PlayerManager>();
        inputManager = GetComponent<PlayerInputManager>();
        collider = GetComponent<CapsuleCollider>();
        playerRB = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        AdjustSpeedBasedOnState();
        IsGliding();
        HandleFallingAndLanding();
        if (playerManager.isInteracting)
            return;
        HandleCrouch();
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        moveDirection = new Vector3(cameraObject.forward.x,0f,cameraObject.forward.z) * inputManager.verticalInput;
        moveDirection += cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;
        if (isCrouching)
        {
            moveDirection *= crouchSpeed;
        }
        else if (isSprinting && playerManager.currentState == PlayerHealth.PlayerState.Healed)
        {
            moveDirection *= sprintSpeed;
        } else if (isSprinting && playerManager.currentState == PlayerHealth.PlayerState.Injured && canUseAdrenaline)
        {
            StartCoroutine(AdrenalineBoost());
        }
        else
        {
            if (inputManager.moveAmount >= 0.5f)
            {
                moveDirection *= runningSpeed;
            }
            else
            {
                moveDirection *= walkingSpeed;
            }
        }

        if (isAdrenalineActive)
        {
            moveDirection *= adrenalineSpeed;
        }

        Vector3 movementVelocity = new Vector3(moveDirection.x,playerRB.linearVelocity.y,moveDirection.z);
        playerRB.linearVelocity = movementVelocity;
    }
    private void AdjustSpeedBasedOnState()
    {
        if (playerManager.currentState == PlayerHealth.PlayerState.Healed)
        {
            walkingSpeed = 1.5f; 
            sprintSpeed = 5f; 
            canUseAdrenaline = true; 
        }
        else if (playerManager.currentState == PlayerHealth.PlayerState.Injured)
        {
            walkingSpeed = 0.5f; 
            sprintSpeed = 3f; 
        }
    }

    private IEnumerator AdrenalineBoost()
    {
        canUseAdrenaline = false; 
        isAdrenalineActive = true;
        yield return new WaitForSeconds(adrenalineDuration);
        isAdrenalineActive = false;
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }
        
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation =
            Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
{
    RaycastHit hit;
    Vector3 rayCastOrigin = transform.position;
    rayCastOrigin.y += rayCastHeightOffSet;
    
    if (!isGrounded && !isJumping)
    {
        if (!playerManager.isInteracting)
        {
            animatorManager.PlayTargetAnimation("Descending", false);
        }

        inAirTimer += Time.deltaTime;
        
        playerRB.AddForce(transform.forward * leapingVelocity, ForceMode.Acceleration);
        
        if (isGliding && playerManager.currentState == PlayerHealth.PlayerState.Healed)
        {
            playerRB.AddForce(-Vector3.up * glideFallSpeed);
        }
        else
        {
            playerRB.AddForce(-Vector3.up * fallingVelocity * inAirTimer, ForceMode.Acceleration);
        }
    }
    
    if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, 0.5f, groundLayer))
    {
        if (!isGrounded && playerManager.isInteracting)
        {
            animatorManager.PlayTargetAnimation("Landing", true);
        }
        
        inAirTimer = 0;
        isGrounded = true;
        playerManager.isInteracting = false;
    }
    else
    {
        isGrounded = false;
    }
    
}
    private bool IsNearWall(out Vector3 wallNormal)
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        float wallDistance = 1.0f; // Adjust the distance for wall detection
        Vector3[] directions = { transform.forward, transform.right, -transform.right }; // Check front and sides

        foreach (var direction in directions)
        {
            if (Physics.Raycast(rayCastOrigin, direction, out hit, wallDistance))
            {
                if (hit.collider.CompareTag("Wall")) // Make sure the wall has the correct tag
                {
                    wallNormal = hit.normal; // Return the wall normal
                    return true; // Wall detected
                }
            }
        }

        wallNormal = Vector3.zero; // No wall detected
        return false;
    }

    private void HandleCrouch()
    {
        if (isCrouching)
        {
            transform.localScale = new Vector3(1, 0.5f, 1);
            collider.height = 0.2f;
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            collider.height = 0.4f;
        }
    }

public void HandleJump()
{
    // If the player is grounded, allow the initial jump
    if (isGrounded && !isCrouching)
    {
        animatorManager.animator.SetBool("isJumping", true);
        animatorManager.PlayTargetAnimation("Jump", false);
        float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
        Vector3 playerVelocity = moveDirection;
        playerVelocity.y = jumpingVelocity;
        playerRB.velocity = playerVelocity; // Set the velocity for the jump

        inAirTimer = 0; // Reset air timer
        isGliding = false; // Stop gliding when performing a jump
        currentJumpCount = 1; // Set to 1 to indicate the player has jumped once
    }
    else if (IsNearWall(out Vector3 wallNormal)&& playerManager.currentState != PlayerHealth.PlayerState.Injured)
    {
        animatorManager.animator.SetBool("isJumping", true);
        animatorManager.PlayTargetAnimation("Jump", false);

        // Calculate jump velocity away from the wall
        float wallJumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
        Vector3 jumpDirection = moveDirection + (wallNormal * wallJumpOffForce);
        jumpDirection.y = wallJumpingVelocity;
        playerRB.velocity = jumpDirection; // Set the velocity for the wall jump

        inAirTimer = 0; // Reset air timer for wall jump
        isGliding = false; // Stop gliding when wall jumping
        // Do not increment currentJumpCount for wall jump
    }
    // Allow double jump only if the player is in the air and hasn't reached the max jump count
    else if (currentJumpCount > 0 && currentJumpCount < maxJumps && playerManager.currentState != PlayerHealth.PlayerState.Injured)
    {
        animatorManager.animator.SetBool("isJumping", true);
        animatorManager.PlayTargetAnimation("Jump", false);
        float doubleJumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
        Vector3 playerVelocity = moveDirection;
        playerVelocity.y = doubleJumpingVelocity;
        playerRB.velocity = playerVelocity; // Set the velocity for the double jump

        inAirTimer = 0; // Reset air timer to ensure smooth double jump
        isGliding = false; // Stop gliding when performing a double jump
        currentJumpCount++; // Increment the jump count to 2 for the double jump
    }
    // Wall jump if the player is near a wall, regardless of jump count

}

    private void IsGliding()
    {
        // Activate gliding only if the player is in the air
        if (!isGrounded && inputManager.glideInput)
        {
            isGliding = true; // Activate gliding
        }
        else if (isGliding && !inputManager.glideInput)
        {
            isGliding = false; // Deactivate gliding when the jump input is released
        }
    }

}
