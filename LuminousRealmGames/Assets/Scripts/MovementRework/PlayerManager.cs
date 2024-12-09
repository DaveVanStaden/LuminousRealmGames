using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerInput inputManager;
    private PlayerLocomotion playerLocomotion;
    private CameraManager cameraManager;
    private PlayerHealth playerHealth;
    public PlayerHealth.PlayerState currentState;
    [SerializeField]private Animator animator;
    public bool pauseInput;
    private PauseMenu pauseMenu;


    public bool isInteracting;

    public PlayerInput PlayerInput => inputManager;
    private void Awake()
    {
        pauseMenu = FindFirstObjectByType<PauseMenu>().GetComponent<PauseMenu>();
        animator.GetComponent<Animator>();
        //playerLocomotion = GetComponent<PlayerLocomotion>();
        cameraManager = FindAnyObjectByType<CameraManager>();
        playerHealth = GetComponent<PlayerHealth>();
    }
    private void HandlePause()
    {
        if (pauseInput)
        {
            pauseMenu.TogglePause();
            pauseInput = false;
        }
    }
    private void OnEnable()
    {
        if (inputManager == null)
        {
            inputManager = new PlayerInput();
            inputManager.PlayerMovement.Pause.performed += i => pauseInput = true;
        }

        inputManager.Enable();
    }

    private void Update()
    {
        HandlePause();
        currentState = playerHealth.currentState;
        //inputManager.HandleAllInputs();
        if (playerHealth.currentState == PlayerHealth.PlayerState.Healed)
        {
            playerHealth.UpdatePlayerState(PlayerHealth.PlayerState.Healed);
            playerHealth.HandleHealedState();
        }
        else if (playerHealth.currentState == PlayerHealth.PlayerState.Injured)
        {
            playerHealth.UpdatePlayerState(PlayerHealth.PlayerState.Injured);
            playerHealth.HandleInjuredState();
        }
        if (playerHealth.health <= 0)
        {
            playerHealth.HandleDeath();
        }
    }

    private void FixedUpdate()
    {
        //playerLocomotion.HandleAllMovement();
        playerHealth.UpdateHealthUI();  // Update health display every physics frame
        playerHealth.UpdateScreenEffect();
    }

    private void LateUpdate()
    {
        //cameraManager.HandleAllCameraMovement();
        
        isInteracting = animator.GetBool("isInteracting");
        //playerLocomotion.isJumping = animator.GetBool("isJumping");
        //animator.SetBool("isGrounded",playerLocomotion.isGrounded);
    }
}
