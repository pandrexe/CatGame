using UnityEngine;
using UnityEngine.SceneManagement;

public class RoombaEnemy : MonoBehaviour
{
    public Transform target;
    public float speed = 2f;
    public LayerMask groundLayer;
    public Animator animator;

    private Rigidbody2D rb;
    private bool isGrounded;

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

        if (isCatOnTop)
        {
            if (isGrounded)
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
            }
        }
        else
        {
            float direction = Mathf.Sign(target.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!gameObject.activeInHierarchy) return;

        if (collision.transform == target)
        {
            Debug.Log("Il gatto è saltato via dalla testa del Roomba!");
            rb.bodyType = RigidbodyType2D.Dynamic;
            isCatOnTop = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform == target)
        {
            Vector2 contactNormal = collision.GetContact(0).normal;

            if (contactNormal.y > -0.5f)
            {
                Debug.Log("Il gatto è stato investito di lato! GAME OVER.");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                Debug.Log("Il gatto è atterrato sano e salvo sulla testa!");
                rb.bodyType = RigidbodyType2D.Kinematic;
                isCatOnTop = true;
            }
        }
    }
}