using UnityEngine;

public class RoombaEnemy : MonoBehaviour
{

    public Transform target;
    public float speed = 2f;
    public float jumpForce = 2f;
    public LayerMask groundLayer;
    public Animator animator;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool shouldJump;

    private float moveTimer = 0f;
    private float randomDirection = 1f;

    private bool isCatOnTop = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);

        float verticalDifference = target.position.y - transform.position.y;

        if (isGrounded)
        {
            if (isCatOnTop)
            {
                moveTimer -= Time.deltaTime;
                if (moveTimer <= 0f)
                {
                    randomDirection = Random.Range(0, 2) == 0 ? -1f : 1f;
                    moveTimer = 2f;
                }
                
                Vector2 edgeCheckPosition = new Vector2(transform.position.x + (randomDirection * 0.6f), transform.position.y);
                bool isEdgeAhead = !Physics2D.Raycast(edgeCheckPosition, Vector2.down, 1f, groundLayer);

                if (isEdgeAhead)
                {
                    randomDirection *= -1f;
                    moveTimer = 2f;
                }

                rb.linearVelocity = new Vector2(randomDirection * (speed * 0.5f), rb.linearVelocity.y);
                shouldJump = false;
            }
            else
            {
                RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, groundLayer);
                float direction = Mathf.Sign(target.position.x - transform.position.x);
                rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
                if (verticalDifference > 1.5f && platformAbove.collider)
                {
                    shouldJump = true;
                }
                else
                {
                    shouldJump = false;
                }
            }
             
        }
    }

       private void FixedUpdate()
        {
            if (shouldJump && isGrounded)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                animator.SetTrigger("Jump");
                shouldJump = false;
            }
        }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform == target)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            isCatOnTop = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!gameObject.activeInHierarchy) return;

        if (collision.transform == target)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            isCatOnTop = false;
        }
    }

}
