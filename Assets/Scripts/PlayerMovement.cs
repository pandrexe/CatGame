using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody2D rb;
    public Animator animator;

    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float currentSpeed;
    float horizontalMovement;
    public bool isRunning;
    public bool isMoving;
    public bool isGrounded;

    [Header("Jumping")]
    public float jumpPower = 7f;


    [Header("Ground Check")]
    public Transform groundCheckPosition;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity = 1f;
    public float fallMultiplier = 2f;
    public float maxFallSpeed = 5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isRunning = false;
        isGrounded = true;
        currentSpeed = walkSpeed;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * currentSpeed, rb.linearVelocity.y);
        animator.SetBool("isMoving", Mathf.Abs(horizontalMovement) > 0);
        isMoving = Mathf.Abs(horizontalMovement) > 0;
        animator.SetBool("isRunning", isRunning && isMoving);
        Gravity();
        isGrounded = IsGrounded();
        animator.SetBool("isGrounded", isGrounded);
    }

    private void Gravity()
        {
        if (rb.linearVelocity.y < 0)
        {
            animator.SetFloat("yVelocity", rb.linearVelocity.y);
            rb.gravityScale = baseGravity * fallMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            animator.SetFloat("yVelocity", rb.linearVelocity.y);
            rb.gravityScale = baseGravity;
        }
    }



    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
        animator.SetFloat("inputX", horizontalMovement);
        if (horizontalMovement != 0)
        {
            animator.SetFloat("lastInputX", horizontalMovement);
        }
       
    }

    public void Run(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isRunning = !isRunning;
        }
        if (isRunning)
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }
        
    }

    public void Jump(InputAction.CallbackContext context)
    {
            if (context.performed && IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                animator.SetTrigger("Jump");
        }
            else if (context.canceled)
            {
                if (rb.linearVelocity.y > 0)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                }
            }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheckPosition.position, groundCheckSize, 0f, groundLayer);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPosition.position, groundCheckSize);
    }

}
