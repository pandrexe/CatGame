using UnityEngine;
using UnityEngine.SceneManagement;

public class RoombaEnemy : MonoBehaviour
{
    public enum StatoRoomba { InsegueGatto, RubaCuscino, RiportaCuscino }

    [Header("Impostazioni Base")]
    public Transform target;
    public float speed = 2f;
    public LayerMask groundLayer;
    public Animator animator;

    [Header("Meccanica Cuscino")]
    public StatoRoomba statoAttuale = StatoRoomba.InsegueGatto;
    public Transform cuscino;
    // Rimosso: public Transform baseCuscino; <-- NON SERVE PIÙ!
    public Transform carryPoint;
    public Collider2D triggerInterazione; 

    // Memoria del Roomba per ricordarsi dove stava il cuscino!
    private Vector3 posizioneInizialeCuscino; 
    private float velocitaBoost;
    private AudioSource audioSource;
    private float soundDistance = 30f;
    private float volumeValueChanger = 1f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveTimer = 0f;
    private float randomDirection = 1f;
    private bool isCatOnTop = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        velocitaBoost = speed * 2f;

        // Memorizziamo la posizione del cuscino al primo frame!
        if (cuscino != null)
        {
            posizioneInizialeCuscino = cuscino.position;
        }
    }

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
            bool isPoggiato = (cuscino != null && cuscino.parent == null);
            
            // 2. È stato spostato dalla sua posizione iniziale? (Basta che si sia mosso di 1 metro)
            bool spostatoDaInizio = (cuscino != null && Vector2.Distance(cuscino.position, posizioneInizialeCuscino) > 1f);

            // Se è a terra, NON è nel suo posto originale, e io stavo inseguendo il gatto... VADO A RUBARLO!
            if (isPoggiato && spostatoDaInizio && statoAttuale == StatoRoomba.InsegueGatto)
            {
                statoAttuale = StatoRoomba.RubaCuscino;
            }

            Vector3 destinazione = transform.position;
            float velocitaAttuale = speed;

            if (statoAttuale == StatoRoomba.RubaCuscino)
            {
                if (cuscino != null) destinazione = cuscino.position;
                velocitaAttuale = velocitaBoost;

                if (cuscino != null && Mathf.Abs(transform.position.x - cuscino.position.x) < 0.5f)
                {
                    RaccogliCuscino();
                }
            }
            else if (statoAttuale == StatoRoomba.RiportaCuscino)
            {
                // La destinazione ora è la memoria iniziale
                destinazione = posizioneInizialeCuscino; 
                velocitaAttuale = velocitaBoost;

                if (Mathf.Abs(transform.position.x - posizioneInizialeCuscino.x) < 0.5f)
                {
                    MollaCuscino();
                }
            }
            else
            {
                if (target != null) destinazione = target.position;
                velocitaAttuale = speed;
            }

            // Muovi verso la destinazione scelta
            if (Mathf.Abs(destinazione.x - transform.position.x) > 0.1f) // Evita che il roomba "tremi" quando arriva
            {
                float direction = Mathf.Sign(destinazione.x - transform.position.x);
                rb.linearVelocity = new Vector2(direction * velocitaAttuale, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }

        VolumeChanger();
    }

    private void RaccogliCuscino()
    {
        statoAttuale = StatoRoomba.RiportaCuscino;
        if (triggerInterazione != null) triggerInterazione.enabled = false;

        if (cuscino != null)
        {
            cuscino.SetParent(carryPoint);
            cuscino.localPosition = Vector3.zero;

            // --- NOVITÀ: SPEGNIAMO LA FISICA DEL CUSCINO ---
            Rigidbody2D rbCuscino = cuscino.GetComponent<Rigidbody2D>();
            if (rbCuscino != null)
            {
                rbCuscino.bodyType = RigidbodyType2D.Kinematic;
                rbCuscino.linearVelocity = Vector2.zero;
            }

            // Spegniamo il collider solido del cuscino per non farlo sbattere contro il Roomba
            Collider2D[] colliderCuscino = cuscino.GetComponents<Collider2D>();
            foreach (Collider2D col in colliderCuscino)
            {
                if (!col.isTrigger) col.enabled = false;
            }
            // ----------------------------------------------
        }
    }

   private void MollaCuscino()
    {
        statoAttuale = StatoRoomba.InsegueGatto;
        
        // Ho rimosso la riga "isCuscinoPiazzato = false;" !

        if (triggerInterazione != null) triggerInterazione.enabled = true;

        if (cuscino != null)
        {
            cuscino.SetParent(null);
            
            // Lo rimette esattamente alle coordinate X originali
            cuscino.position = new Vector3(posizioneInizialeCuscino.x, cuscino.position.y, cuscino.position.z);

            // RIACCENDE LA FISICA DEL CUSCINO (il trucco del simulated!)
            Rigidbody2D rbCuscino = cuscino.GetComponent<Rigidbody2D>();
            if (rbCuscino != null)
            {
                rbCuscino.simulated = true;
            }
        }
    }
    private void OnDisable()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero; 
        if(audioSource != null) audioSource.Stop();
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

        if (collision.transform == target)
        {
            Vector2 contactNormal = collision.GetContact(0).normal;

            if (contactNormal.y > -0.5f)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

        if (GameManager.Instance != null && GameManager.Instance.inMinigioco)
        {
            audioSource.volume = 1f;
            return;
        }
        float distance = Vector2.Distance(transform.position, target.position);
        if (distance < soundDistance && distance > 0)
        {
            audioSource.volume = Mathf.Clamp(volumeValueChanger/distance, 0f, 1f);
        }
        else
        {
            audioSource.volume = 0f;
        }
    }

    public void SpegnimentoDefinitivo()
    {
        this.enabled = false;
        if (transform.parent != null)
        {
            transform.parent.tag = "Platform";
        }
    }
}