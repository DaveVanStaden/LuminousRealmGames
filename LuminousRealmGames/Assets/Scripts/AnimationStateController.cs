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
    }

    void FixedUpdate()
    {
        GroundCheck();
    }

    private void GroundCheck() 
    {
        Vector3 rayOrigin = groundCheck.position;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, 0.2f, groundLayer);
    }

    void Update()
    {
        bool isTrotting = animator.GetBool(isTrottingHash);
        bool isIdle = animator.GetBool(isIdleHash);
        bool isJumping = animator.GetBool(isJumpingHash);
        bool isFalling = animator.GetBool(isFallingHash);
        bool isLanding = animator.GetBool(isLandingHash);

        // Check for movement input
        bool isMoving = Input.GetKey("a") || Input.GetKey("w") || Input.GetKey("d") || Input.GetKey("s") || 
                        Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        // Check for jump input
        bool isSpacePressed = Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump");
        bool isSpaceReleased = Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("Jump");

        // Movement Animation
        if (isMoving && isGrounded)
        {
            animator.SetBool(isTrottingHash, true);
            animator.SetBool(isIdleHash, false);
        }
        else if (!isMoving && isGrounded)
        {
            animator.SetBool(isTrottingHash, false);
            animator.SetBool(isIdleHash, true);
        }
        else if (isMoving && !isGrounded)
        {
            animator.SetBool(isTrottingHash, false);
            animator.SetBool(isIdleHash, false);
        }

        // Jumping Animation
        if (isSpacePressed && isGrounded)
        {
            animator.SetBool(isJumpingHash, true);
            animator.SetBool(isTrottingHash, false);
            animator.SetBool(isIdleHash, false);
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
            if (isFalling)
            {
                animator.SetBool(isFallingHash, false);
                animator.SetBool(isLandingHash, true);
            }
            else
            {
                animator.SetBool(isLandingHash, false);
            }
        }
    }
}
