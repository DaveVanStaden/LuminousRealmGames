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
            moveSpeed = 4f;     // Set to a faster speed when injured
            sprintSpeed = 7f;   // Adjust sprint speed when healed
        }
        else if (playerHealth.currentState == PlayerHealth.PlayerState.Injured)
        {
            moveSpeed = 6f;     // Set to a faster speed when injured
            sprintSpeed = 12f;  // Increase sprint speed when injured
        }
    }

    private void HandleInput()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        moveInput = new Vector3(moveHorizontal, 0, moveVertical).normalized;
        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isChargingJump = true;
            jumpChargeTime = 0f;  
        }
        
        if (Input.GetButton("Jump") && isChargingJump)
        {
            jumpChargeTime += Time.deltaTime;
            jumpChargeTime = Mathf.Clamp(jumpChargeTime, 0f, maxChargeTime);  
        }
        
        if (Input.GetButtonUp("Jump") && isChargingJump)
        {
            PerformJump();
            isChargingJump = false;  
        }
        
        if (Input.GetButtonDown("Jump") && !isGrounded && jumpCount < maxJumps && !hasWallJumped)
        {
            PerformJump();  
            jumpCount++;
        }
    }

    private void PerformJump()
    {
        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, jumpChargeTime / maxChargeTime);
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); 
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        jumpChargeTime = 0f;
        if (!isGrounded)
        {
            jumpCount++; 
        }
        hasWallJumped = false;
        
        isGliding = false;
    }

    private void HandleMovement()
    {
        float currentMoveSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

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

        if (!isGrounded && rb.linearVelocity.y < 0)
        {
            if (Input.GetButton("Jump"))
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
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, 0.2f, groundLayer);

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
                    
                    if (!hasWallJumped)
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

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * moveVertical + orientation.right * moveHorizontal;

        if (inputDir != Vector3.zero)
        {
            playerObj.forward = Vector3.Lerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
    }
}