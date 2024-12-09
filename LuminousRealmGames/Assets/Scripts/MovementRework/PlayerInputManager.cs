using System;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private AnimatorManager animatorManager;
    private PlayerLocomotion playerLocomotion;
    private PlayerHealth playerHealth;
    private PauseMenu pauseMenu;

    public float cameraInputX;
    public float cameraInputY;
    
    public Vector2 movementInput;
    public Vector2 cameraInput;
    
    public float moveAmount;
    public float verticalInput;
    public float horizontalInput;

    public bool b_Input;
    public bool jumpInput;
    public bool glideInput;
    public bool crouchInput;
    public bool pauseInput;

    [Header("Jump Settings")]
    public bool jumpInputHeld; // True while jump button is held
    private float jumpCharge = 0f;

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerHealth = GetComponent<PlayerHealth>();
        pauseMenu = FindFirstObjectByType<PauseMenu>().GetComponent<PauseMenu>();
    }

    private void OnEnable()
    {
        if (playerInput == null)
        {
            playerInput = new PlayerInput();

            playerInput.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerInput.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            playerInput.PlayerMovement.B.performed += i => b_Input = true;
            playerInput.PlayerMovement.B.canceled += i => b_Input = false;
            playerInput.PlayerMovement.Jump.performed += i => glideInput = true;
            playerInput.PlayerMovement.Jump.canceled += i => glideInput = false;
            playerInput.PlayerMovement.Jump.performed += i => jumpInput = true;
            playerInput.PlayerMovement.Crouch.performed += i => crouchInput = true;
            playerInput.PlayerMovement.Crouch.canceled += i => crouchInput = false;
            
        }
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintingInput();
        HandleJumpInput();
        HandleCrouchInput();
        
    }



    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputY = cameraInput.y;
        cameraInputX = cameraInput.x;
        
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(0,moveAmount, playerLocomotion.isSprinting, playerHealth.currentState);
    }

    private void HandleSprintingInput()
    {
        if (b_Input && moveAmount > 0.5f)
        {
            playerLocomotion.isSprinting = true;
        }
        else
        {
            playerLocomotion.isSprinting = false;
        }
    }

    private void HandleJumpInput()
    {
        if (jumpInput)
        {
            playerInput.PlayerMovement.Jump.canceled += i => jumpInput = false;
            playerLocomotion.HandleJump(); // Initial jum
        }

        if (playerLocomotion.isChargingJump)
        {
            if (!jumpInput)
            {
                playerLocomotion.HandleJumpCharge();
            }
        }

        

    }

    private void HandleCrouchInput()
    {
        if (crouchInput)
        {
            playerLocomotion.isCrouching = true;
        }
        else
        {
            playerLocomotion.isCrouching = false;
        }
    }
}
