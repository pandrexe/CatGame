using UnityEngine;
using System.Collections;

public class BirdEnemy : MonoBehaviour
{
    [Header("Limiti di Movimento")]
    [Tooltip("Trascina un oggetto vuoto posizionato a sinistra del corridoio")]
    public Transform limiteSinistro;
    [Tooltip("Trascina un oggetto vuoto posizionato a destra del corridoio")]
    public Transform limiteDestro;

    [Header("Parametri Volo Orizzontale")]
    public float velocitaOrizzontale = 6f;

    [Header("Parametri Volo Verticale (Onda)")]
    public float ampiezzaOndaY = 0.8f;
    public float velocitaOndaY = 4f;

    [Header("Grafica")]
    [Tooltip("Spunta questo se lo sprite originale del tuo uccello guarda a DESTRA di base.")]
    public bool spriteGuardaADestraAllInizio = false;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSourceUccello;

    // --- IL SEGRETO DELLO SPAWN ---
    private Vector3 spawnPoint;
    private float direzioneOrizzontale = 1f;
    private float timerOnda = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSourceUccello = GetComponent<AudioSource>();

        spawnPoint = transform.position;
    }

    // --- QUESTO SCATTA OGNI VOLTA CHE IL GATTO ENTRA NELLA STANZA ---
    void OnEnable()
    {
        transform.position = spawnPoint;
        timerOnda = 0f;
        direzioneOrizzontale = 1f;

        Debug.Log($"[BirdEnemy] Gatto entrato nel corridoio. Uccello resettato allo Spawn Point: {spawnPoint}");

        if (audioSourceUccello != null)
        {
            audioSourceUccello.Play();
        }
    }

    // --- QUESTO SCATTA OGNI VOLTA CHE IL GATTO ESCE DALLA STANZA ---
    void OnDisable()
    {
        if (audioSourceUccello != null)
        {
            audioSourceUccello.Stop();
        }
    }

    void Update()
    {
        // --- IL CONGELAMENTO DURANTE I MINIGIOCHI (Solo Movimento) ---
        if (GameManager.Instance != null && GameManager.Instance.inMinigioco)
        {
            // Blocchiamo l'Update qui: l'uccello si ferma immobile nell'esatto punto in cui si trova,
            // il timer dell'onda non avanza, ma l'audio continua a cantare in sottofondo!
            return;
        }

        float nuovaX = transform.position.x + (direzioneOrizzontale * velocitaOrizzontale * Time.deltaTime);

        if (limiteDestro != null && nuovaX >= limiteDestro.position.x)
        {
            nuovaX = limiteDestro.position.x;
            direzioneOrizzontale = -1f;
        }
        else if (limiteSinistro != null && nuovaX <= limiteSinistro.position.x)
        {
            nuovaX = limiteSinistro.position.x;
            direzioneOrizzontale = 1f;
        }

        GestisciFlipGrafica();

        timerOnda += Time.deltaTime * velocitaOndaY;
        float nuovoY = spawnPoint.y + Mathf.Sin(timerOnda) * ampiezzaOndaY;

        transform.position = new Vector3(nuovaX, nuovoY, transform.position.z);
    }

    private void GestisciFlipGrafica()
    {
        if (spriteRenderer == null) return;

        if (direzioneOrizzontale > 0)
        {
            spriteRenderer.flipX = spriteGuardaADestraAllInizio;
        }
        else
        {
            spriteRenderer.flipX = !spriteGuardaADestraAllInizio;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled) return;

        if (GameManager.Instance != null && GameManager.Instance.inMinigioco)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PerdiVita();
            }

            PlayerMovement scriptGatto = collision.gameObject.GetComponent<PlayerMovement>();
            if (scriptGatto != null)
            {
                scriptGatto.SubisciKnockback(transform, 3f, 0.5f);
            }
        }
    }
}