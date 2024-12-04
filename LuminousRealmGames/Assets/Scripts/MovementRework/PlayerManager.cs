using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerInputManager inputManager;
    private PlayerLocomotion playerLocomotion;
    private CameraManager cameraManager;
    private PlayerHealth playerHealth;
    public PlayerHealth.PlayerState currentState;
    [SerializeField]private Animator animator;

    public bool isInteracting;
    private void Awake()
    {
        animator.GetComponent<Animator>();
        //inputManager = GetComponent<PlayerInputManager>();
        //playerLocomotion = GetComponent<PlayerLocomotion>();
        cameraManager = FindAnyObjectByType<CameraManager>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
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
