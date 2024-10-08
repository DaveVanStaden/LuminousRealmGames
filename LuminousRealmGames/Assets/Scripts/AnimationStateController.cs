using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    int isTrottingHash;
    int isIdleHash;
    int isJumpingHash;
    int isHoveringHash;
    int isFallingHash;
    int isLandingHash;
    int isChargingHash;
    int isWalkingHash;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isGrounded = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        isTrottingHash = Animator.StringToHash("isTrotting");
        isIdleHash = Animator.StringToHash("isIdle");
        isJumpingHash = Animator.StringToHash("isJumping");
        isHoveringHash = Animator.StringToHash("isHovering");
        isFallingHash = Animator.StringToHash("isFalling");
        isLandingHash = Animator.StringToHash("isLanding");
        isChargingHash = Animator.StringToHash("isCharging");
        isWalkingHash = Animator.StringToHash("isWalking");
    }

    void FixedUpdate()
    {
        GroundCheck();
    }

    private void GroundCheck() 
    {
        Vector3 rayOrigin = groundCheck.position;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, 0.4f, groundLayer);
    }

    void Update()
    {
        bool isTrotting = animator.GetBool(isTrottingHash);
        bool isIdle = animator.GetBool(isIdleHash);
        bool isJumping = animator.GetBool(isJumpingHash);
        bool isFalling = animator.GetBool(isFallingHash);
        bool isLanding = animator.GetBool(isLandingHash);
        bool isCharging = animator.GetBool(isChargingHash);
        bool isHovering = animator.GetBool(isHoveringHash);
        bool isWalking = animator.GetBool(isWalkingHash);

        // Check for movement input
        bool isMoving = Input.GetKey("a") || Input.GetKey("w") || Input.GetKey("d") || Input.GetKey("s") ||
                        Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Check for jump input
        bool isSpacePressed = Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump");
        bool isSpaceReleased = Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("Jump");
        bool isSpaceHeld = Input.GetKey(KeyCode.Space);

        // Movement Animation
        if (isMoving && isGrounded)
        {
            animator.SetBool(isWalkingHash, true);
            animator.SetBool(isIdleHash, false);
        }
        else if (!isMoving && isGrounded)
        {
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isTrottingHash, false);
            animator.SetBool(isIdleHash, true);
        }
        else if (isMoving && !isGrounded)
        {
            animator.SetBool(isTrottingHash, false);
            animator.SetBool(isIdleHash, false);
            animator.SetBool(isWalkingHash, false);
        }

        //Sprinting Animation
        if (isMoving && isGrounded && isSprinting)
        {
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isTrottingHash, true);
        }
        else if (isMoving && isGrounded && !isSprinting)
        {
            animator.SetBool(isWalkingHash, true);
            animator.SetBool(isTrottingHash, false);
        }
        //JumpCharge Animation
        if (isSpaceHeld && isGrounded)
        {
            animator.SetBool(isChargingHash, true);
            animator.SetBool(isTrottingHash, false);
            animator.SetBool(isIdleHash, false);
            animator.SetBool(isWalkingHash, false);
        }
        else 
        {
            animator.SetBool(isChargingHash, false);
        }

        // Hovering Animation
        if (isSpaceHeld && !isGrounded)
        {
            animator.SetBool(isHoveringHash, true);
            animator.SetBool(isJumpingHash, false);
            animator.SetBool(isFallingHash, false);
        }
        else if (!isSpaceHeld && !isGrounded)
        {
            animator.SetBool(isHoveringHash, false);
            animator.SetBool(isFallingHash, true);
        }

        // Jumping Animation
        if (isSpacePressed && isGrounded)
        {
            animator.SetBool(isJumpingHash, true);
            animator.SetBool(isTrottingHash, false);
            animator.SetBool(isIdleHash, false);
            animator.SetBool(isWalkingHash, false);
        }

        // If already jumping, do not re-enter jumping animation on button hold
        if (isJumping && !isGrounded)
        {
            animator.SetBool(isJumpingHash, false);
            animator.SetBool(isFallingHash, true);
        }

        // Reset jump and falling animations based on grounded state
        if (isGrounded)
        {
            if (isFalling || isHovering)
            {
                animator.SetBool(isFallingHash, false);
                animator.SetBool(isHoveringHash, false);
                animator.SetBool(isLandingHash, true);
            }
            else
            {
                animator.SetBool(isLandingHash, false);
            }
        }
    }
}
