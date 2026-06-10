using UnityEngine;

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

    // --- IL SEGRETO DELLO SPAWN ---
    private Vector3 spawnPoint; // Memorizza la posizione X, Y, Z esatta di partenza
    private float direzioneOrizzontale = 1f;
    private float timerOnda = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Salviamo una volta per tutte la posizione esatta in cui hai messo l'uccello nell'Editor
        spawnPoint = transform.position;
    }

    // --- QUESTO SCATTA OGNI VOLTA CHE IL GATTO ENTRA NELLA STANZA ---
    void OnEnable()
    {
        // 1. Riportiamo l'uccello istantaneamente al suo punto di spawn originale
        transform.position = spawnPoint;

        // 2. Resettiamo il timer dell'onda a 0, così riparte da centro onda senza scatti visivi
        timerOnda = 0f;

        // 3. Facciamolo ripartire sempre verso destra (1f). Se preferisci sinistra metti -1f
        direzioneOrizzontale = 1f;

        Debug.Log($"[BirdEnemy] Gatto entrato nel corridoio. Uccello resettato allo Spawn Point: {spawnPoint}");
    }

    void Update()
    {
        // 1. MOVIMENTO ORIZZONTALE DIRETTO
        float nuovaX = transform.position.x + (direzioneOrizzontale * velocitaOrizzontale * Time.deltaTime);

        // Controllo dei limiti globali (immuni allo scale della stanza)
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

        // 2. MOVIMENTO VERTICALE (Onda legata tassativamente all'altezza dello spawnPoint originale)
        timerOnda += Time.deltaTime * velocitaOndaY;
        float nuovoY = spawnPoint.y + Mathf.Sin(timerOnda) * ampiezzaOndaY;

        // 3. APPLICAZIONE POSIZIONE
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

        // --- IL BLOCCO MINIGIOCO PER L'UCCELLO ---
        // Se sei nel minigioco delle tende, l'uccello non può farti danni!
        if (GameManager.Instance != null && GameManager.Instance.inMinigioco)
        {
            return; // Interrompe la funzione: niente danni, niente knockback!
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