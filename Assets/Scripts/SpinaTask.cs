using UnityEngine;
using System.Collections;

public class UnplugTask : MonoBehaviour
{
    [Header("Riferimenti Manager")]
    public InteractableTask taskManager; // Obbligatorio!

    [Header("Riferimenti Spina")]
    [Tooltip("Il Collider2D della spina (serve per poterla cliccare con la zampa)")]
    public Collider2D spinaCollider;

    [Tooltip("Opzionale: Crea un oggetto vuoto FISSO alla spina (es. sulla punta) e trascinalo qui per decidere il punto esatto che deve superare il traguardo. Se lo lasci vuoto, userà il centro della spina.")]
    public Transform puntoSpina;

    [Header("Traguardo (Usa Transform)")]
    [Tooltip("Trascina qui l'oggetto vuoto posizionato a destra che fa da linea di arrivo")]
    public Transform traguardoTransform;

    [Header("Audio")]
    [Tooltip("Il suono della spina che viene estratta dalla presa")]
    public AudioClip suonoStacco;

    private AudioSource audioSource;
    private bool taskFinito = false;
    private bool staTrascinando = false;
    private float offsetX;

    // Blocchi per asse Y e Z
    private float startX;
    private float fixedY;
    private float fixedZ;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (spinaCollider != null)
        {
            startX = spinaCollider.transform.position.x;
            fixedY = spinaCollider.transform.position.y;
            fixedZ = spinaCollider.transform.position.z;
        }
    }

    void OnEnable()
    {
        taskFinito = false;
        staTrascinando = false;

        if (spinaCollider != null)
        {
            spinaCollider.transform.position = new Vector3(startX, fixedY, fixedZ);
        }
    }

    void Update()
    {
        if (taskFinito) return;

        // --- SISTEMATO: Usiamo la ZAMPA personalizzata al posto del mouse di Windows ---
        if (MinigameCursor.Instance != null && MinigameCursor.Instance.contenitoreSingola != null)
        {
            // Prendiamo la posizione esatta della zampa nel mondo di gioco
            Vector2 posZampa = MinigameCursor.Instance.contenitoreSingola.transform.position;

            // 1. CLICK INIZIALE SULLA SPINA
            if (Input.GetMouseButtonDown(0))
            {
                if (spinaCollider != null && spinaCollider.OverlapPoint(posZampa))
                {
                    staTrascinando = true;
                    offsetX = spinaCollider.transform.position.x - posZampa.x;
                }
            }

            // 2. TRASCINAMENTO CONTINUO
            if (staTrascinando && Input.GetMouseButton(0))
            {
                float nuovaX = posZampa.x + offsetX;

                // Limite sinistro (non fa incastrare la spina dentro al muro a sinistra)
                nuovaX = Mathf.Max(nuovaX, startX);

                // Applichiamo il movimento bloccando Y e Z (la spina si muove solo a destra)
                spinaCollider.transform.position = new Vector3(nuovaX, fixedY, fixedZ);

                // 3. CONTROLLO MATEMATICO DEL TRAGUARDO
                if (traguardoTransform != null)
                {
                    float xAttualeSpina = (puntoSpina != null) ? puntoSpina.position.x : spinaCollider.transform.position.x;

                    // Se la spina supera il traguardo... VITTORIA!
                    if (xAttualeSpina >= traguardoTransform.position.x)
                    {
                        StaccaSpina();
                    }
                }
            }
        }

        // Se rilasci il click, smetti di trascinare
        if (Input.GetMouseButtonUp(0))
        {
            staTrascinando = false;
        }
    }

    private void StaccaSpina()
    {
        taskFinito = true;
        staTrascinando = false;
        Debug.Log("Spina staccata con successo!");

        // --- FA PARTIRE IL SUONO METALLICO/ELETTRICO DEL DISTACCO ---
        if (suonoStacco != null && audioSource != null)
        {
            audioSource.PlayOneShot(suonoStacco);
        }

        StartCoroutine(SequenzaVittoria());
    }

    private IEnumerator SequenzaVittoria()
    {
        // Aspetta 1 secondo per goderti l'audio prima di chiudere il minigioco
        yield return new WaitForSeconds(1f);

        if (taskManager != null) taskManager.CompletaTask();
        if (GameManager.Instance != null) GameManager.Instance.VinciMinigioco();
    }
}