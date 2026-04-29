using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private CatAudio catAudio;
    private Rigidbody2D currentPlatformRb;
    BoxCollider2D playerCollider;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 5f;

    private float currentSpeed;
    private float horizontalMovement;
    private bool isRunning;
    private bool isMoving;
    private bool isGrounded;
    private bool isOnPlatform;

    [Header("Jumping")]
    [SerializeField] private float jumpPower = 7f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPosition;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    [SerializeField] private LayerMask groundLayer;

    [Header("Gravity")]
    [SerializeField] private float baseGravity = 1f;
    [SerializeField] private float fallMultiplier = 2f;
    [SerializeField] private float maxFallSpeed = 5f;

    void Start()
    {
        isRunning = false;
        currentSpeed = walkSpeed;

        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (catAudio == null) catAudio = GetComponent<CatAudio>();
        if (playerCollider == null) playerCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        isGrounded = IsGrounded();
        isMoving = Mathf.Abs(horizontalMovement) > 0;
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isRunning", isRunning && isMoving);
        animator.SetBool("isGrounded", isGrounded);
        Fall();
    }

    private void FixedUpdate()
    {
        float targetSpeedX = horizontalMovement * currentSpeed;

        if (isGrounded && currentPlatformRb != null)
        {
            targetSpeedX += currentPlatformRb.linearVelocity.x;
        }

        rb.linearVelocity = new Vector2(targetSpeedX, rb.linearVelocity.y);

        Gravity();
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

        currentSpeed = isRunning ? runSpeed : walkSpeed;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
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

    public void Drop(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && isOnPlatform && playerCollider.enabled)
        {
            StartCoroutine(DisablePlayerCollider(0.25f));
        }
    }

    private IEnumerator DisablePlayerCollider(float duration)
    {
        playerCollider.enabled = false;
        yield return new WaitForSeconds(duration);
        playerCollider.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = false;
        }
    }

    public void Meow(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded)
        {
            animator.SetTrigger("Meow");
            if (catAudio != null)
            {
                catAudio.PlayMeow();
            }
        }
    }

    private bool IsGrounded()
    {
        Collider2D groundCollider = Physics2D.OverlapBox(groundCheckPosition.position, groundCheckSize, 0f, groundLayer);

        if (groundCollider != null)
        {
            groundCollider.TryGetComponent(out currentPlatformRb);
            return true;
        }

        currentPlatformRb = null;
        return false;
    }

    public void OnDrawGizmosSelected()
    {
        if (groundCheckPosition != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheckPosition.position, groundCheckSize);
        }
    }

    public void Fall()
    {
        if(rb.linearVelocity.y < 0 && !isGrounded)
        {
            animator.SetBool("isFalling", true);
        }
        else
        {
            animator.SetBool("isFalling", false);
        }
    }

  
}