using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class BasketballTask : MonoBehaviour
{
    [Header("Riferimento Manager (Obbligatorio)")]
    public InteractableTask taskManager;

    [Header("Riferimenti Canestro")]
    public Collider2D colliderAnello;
    [Tooltip("Trascina qui l'oggetto 'ferro_canestro_0' che farà da maschera (quello con Order Layer 4).")]
    public GameObject spriteFerroOverlay;

    [Header("Audio")]
    public AudioClip suonoCiakCanestro;

    [Header("Impostazioni Lancio")]
    public float moltiplicatoreForza = 12f;
    public float scalaMinima = 0.5f;
    public float tempoPerRimpicciolire = 0.8f;
    public float tolleranzaCaduta = 2f;

    private Rigidbody2D rb;
    private AudioSource audioSource;

    private Vector3 posInizialePalla;
    private Vector3 scalaIniziale;
    private Vector2 puntoInizioClick;
    private bool staCaricando = false;
    private bool haLanciato = false;
    private bool taskFinito = false;

    private float yIniziale;
    private float tempoInVolo = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        posInizialePalla = transform.position;
        scalaIniziale = transform.localScale;
    }

    void OnEnable()
    {
        ResetPalla();
        taskFinito = false;
    }

    void Update()
    {
        if (taskFinito) return;

        if (!haLanciato)
        {
            GestisciSwipe();
        }
        else
        {
            GestisciProfondita();

            // --- LA TUA LOGICA GENIALE PER IL FERRO ---
            if (spriteFerroOverlay != null)
            {
                // Se la palla sale, spegniamo l'overlay (si vedrà il ferro disegnato dietro)
                if (rb.linearVelocity.y > 0 && spriteFerroOverlay.activeSelf)
                {
                    spriteFerroOverlay.SetActive(false);
                }
                // Se la palla scende, riaccendiamo l'overlay in primo piano!
                else if (rb.linearVelocity.y < 0 && !spriteFerroOverlay.activeSelf)
                {
                    spriteFerroOverlay.SetActive(true);
                }
            }

            // Controllo tiro sbagliato: se cade sotto la tolleranza, si resetta e riprovi
            if (rb.linearVelocity.y < 0 && transform.position.y < (yIniziale - tolleranzaCaduta))
            {
                ResetPalla();
            }
        }
    }

    private void GestisciSwipe()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (GetComponent<Collider2D>().OverlapPoint(mouseWorldPos))
            {
                staCaricando = true;
                puntoInizioClick = mouseWorldPos;
            }
        }

        if (Input.GetMouseButtonUp(0) && staCaricando)
        {
            staCaricando = false;
            Vector2 puntoFineClick = mouseWorldPos;

            Vector2 vettoreLancio = puntoFineClick - puntoInizioClick;

            if (vettoreLancio.magnitude > 0.1f)
            {
                haLanciato = true;

                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 2f;
                rb.AddForce(vettoreLancio * moltiplicatoreForza, ForceMode2D.Impulse);
            }
        }
    }

    private void GestisciProfondita()
    {
        tempoInVolo += Time.deltaTime;

        float percentualeCompletamento = Mathf.Clamp01(tempoInVolo / tempoPerRimpicciolire);

        float nuovaScala = Mathf.Lerp(scalaIniziale.x, scalaMinima, percentualeCompletamento);
        transform.localScale = new Vector3(nuovaScala, nuovaScala, 1f);
    }

    private void ResetPalla()
    {
        haLanciato = false;
        staCaricando = false;
        tempoInVolo = 0f;

        transform.position = posInizialePalla;
        transform.localScale = scalaIniziale;

        // Assicuriamoci che il ferro sia acceso di default prima del lancio
        if (spriteFerroOverlay != null)
        {
            spriteFerroOverlay.SetActive(true);
        }

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        yIniziale = transform.position.y;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (taskFinito || !haLanciato) return;

        if (collision == colliderAnello)
        {
            if (rb.linearVelocity.y < 0)
            {
                taskFinito = true;
                StartCoroutine(SequenzaVittoria());
            }
        }
    }

    private IEnumerator SequenzaVittoria()
    {
        if (suonoCiakCanestro != null && audioSource != null)
        {
            audioSource.PlayOneShot(suonoCiakCanestro);
        }

        yield return new WaitForSeconds(1f);

        if (taskManager != null) taskManager.CompletaTask();
        if (GameManager.Instance != null) GameManager.Instance.VinciMinigioco();
    }
}