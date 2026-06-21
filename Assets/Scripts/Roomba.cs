using UnityEngine;

public class RoombaEnemy : MonoBehaviour
{
    [Header("Impostazioni Base")]
    public Transform target;
    public float speed = 2f;
    public LayerMask groundLayer;
    public Animator animator;
    public float distanzaTeletrasporto = 3f;

    private AudioSource audioSource;
    private float soundDistance = 30f;
    private float volumeValueChanger = 1f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveTimer = 0f;
    private float randomDirection = 1f;
    private bool isCatOnTop = false;

    // --- LA NOVITÀ: IL CERTIFICATO DI MORTE ---
    private bool eMortoDefinitivamente = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        if (eMortoDefinitivamente)
        {
            this.enabled = false;
            return;
        }

        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void Update()
    {
        // --- IL CONGELAMENTO DURANTE I MINIGIOCHI (Solo Movimento) ---
        if (GameManager.Instance != null && GameManager.Instance.inMinigioco)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            // Continuiamo ad aggiornare il volume così senti il ronzio!
            VolumeChanger();
            return;
        }

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
            Vector3 destinazione = transform.position;
            if (target != null) destinazione = target.position;

            if (Mathf.Abs(destinazione.x - transform.position.x) > 0.1f)
            {
                float direction = Mathf.Sign(destinazione.x - transform.position.x);
                rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }

        VolumeChanger();
    }

    private void OnDisable()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (audioSource != null) audioSource.Stop();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!this.enabled || !gameObject.activeInHierarchy) return;

        if (collision.transform == target)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            isCatOnTop = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!this.enabled) return;

        if (GameManager.Instance != null && GameManager.Instance.inMinigioco)
        {
            return;
        }

        if (collision.transform == target)
        {
            Vector2 contactNormal = collision.GetContact(0).normal;

            if (contactNormal.y > -0.5f)
            {
                GameManager.Instance.PerdiVita();

                PlayerMovement scriptGatto = collision.gameObject.GetComponent<PlayerMovement>();
                if (scriptGatto != null)
                {
                    scriptGatto.SubisciKnockback(transform, distanzaTeletrasporto, 0.5f);
                }
            }
            else
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                isCatOnTop = true;
            }
        }
    }

    private void VolumeChanger()
    {
        if (audioSource == null || target == null) return;

        // Ho rimosso l'if che azzerava il volume nel minigioco!
        // Ora calcola sempre la distanza, che tu stia graffiando il divano o no.

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance < soundDistance && distance > 0)
        {
            audioSource.volume = Mathf.Clamp(volumeValueChanger / distance, 0f, 1f);
        }
        else
        {
            audioSource.volume = 0f;
        }
    }

    public void SpegnimentoDefinitivo()
    {
        eMortoDefinitivamente = true;
        this.enabled = false;
        if (audioSource != null) audioSource.Stop();
        if (transform.parent != null) transform.parent.tag = "Platform";
    }
}