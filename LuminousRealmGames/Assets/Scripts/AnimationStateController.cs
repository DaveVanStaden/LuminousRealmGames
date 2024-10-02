using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    // int isStrafingRightHash;
    // int isStrafingLeftHash;
    int isTrottingHash;
    int isIdleHash;
    int isJumpingHash;
    int isHoveringHash;
    int isFallingHash;
    int isLandingHash;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isGrounded = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
       
        if (isGrounded)
        {
            animator.SetBool(isFallingHash, false);
            animator.SetBool(isLandingHash, true);
        }
    }

    // Update is called once per frame
    void Update()
    {

        bool isTrotting = animator.GetBool(isTrottingHash);
        bool isIdle = animator.GetBool(isIdleHash);
        bool isJumping = animator.GetBool(isJumpingHash);
        bool isHovering = animator.GetBool(isHoveringHash);
        bool isFalling = animator.GetBool(isFallingHash);
        bool isLanding = animator.GetBool(isLandingHash);

        bool isMoving = Input.GetKey("a") || Input.GetKey("w") || Input.GetKey("d") || Input.GetKey("s");
        bool isSpacePressed = Input.GetKeyDown(KeyCode.Space);
        bool isSpaceHold = Input.GetKey(KeyCode.Space);
        bool isSpaceReleased = Input.GetKeyUp(KeyCode.Space);

        if (isMoving && !isSpaceHold)
        {
            animator.SetBool(isTrottingHash, true);
            animator.SetBool(isIdleHash, false);
        } 
        else
        {
            animator.SetBool(isTrottingHash, false);
            animator.SetBool(isIdleHash, true);
        }

        if (isSpacePressed)
        {
            animator.SetBool(isJumpingHash, true);
            animator.SetBool(isTrottingHash, false);
            animator.SetBool(isIdleHash, false); 
        }
        else 
        {
            animator.SetBool(isJumpingHash, false);
        }

        if (isSpaceHold && !isGrounded) 
        {
            animator.SetBool(isHoveringHash, true);
            animator.SetBool(isFallingHash, false);
        }
        else
        {
            animator.SetBool(isHoveringHash, false);
        }

        if (isSpaceReleased)
        {
            animator.SetBool(isFallingHash, true);
        }
        if (isSpaceReleased && isGrounded) 
        {
            animator.SetBool(isFallingHash, false);
            animator.SetBool(isLandingHash, true);
        }
    }
}
