using UnityEngine;

public class EndlessMovement : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float xMin = -5f;
    [SerializeField] private float xMax = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private float verticalVelocity = 0f;
    private bool isJumping = false;

    void Update()
    {
        HandleMovement();
        HandleJump();
        CheckGround();
    }

    private void HandleMovement()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        float horizontalInput = Input.GetAxis("Horizontal");
        float newXPosition = transform.position.x + horizontalInput * moveSpeed * Time.deltaTime;
        newXPosition = Mathf.Clamp(newXPosition, xMin, xMax);
        transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            verticalVelocity = jumpForce;
            isJumping = true;
        }

        if (isJumping)
        {
            verticalVelocity += gravity * Time.deltaTime; 
            transform.position += new Vector3(0, verticalVelocity * Time.deltaTime, 0);
        }
    }

    private void CheckGround()
    {
        Vector3 raycastStart = transform.position;
        Debug.DrawRay(raycastStart, Vector3.down * (groundCheckDistance + 0.1f), Color.red, 1f);

        RaycastHit hit;
        if (Physics.Raycast(raycastStart, Vector3.down, out hit, groundCheckDistance + 0.1f, groundLayer))
        {

            if (isJumping && verticalVelocity <= 0 && hit.distance <= groundCheckDistance)
            {
                verticalVelocity = 0f;
                isJumping = false;
                transform.position = new Vector3(transform.position.x, hit.point.y + 0.05f, transform.position.z);
            }
        }
    }
    
}