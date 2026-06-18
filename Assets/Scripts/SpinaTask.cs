using UnityEngine;
using System.Collections;

public class UnplugTask : MonoBehaviour
{
    [Header("Riferimenti Manager")]
    public InteractableTask taskManager; // Obbligatorio!

    [Header("Riferimenti Spina")]
    [Tooltip("Il Collider2D della spina (serve solo per poterla cliccare col mouse)")]
    public Collider2D spinaCollider;

    [Tooltip("Opzionale: Crea un oggetto vuoto FISGIO alla spina (es. sulla punta) e trascinalo qui per decidere il punto esatto che deve superare il traguardo. Se lo lasci vuoto, userà il centro della spina.")]
    public Transform puntoSpina;

    [Header("Traguardo (Usa Transform)")]
    [Tooltip("Trascina qui l'oggetto vuoto posizionato a destra che fa da linea di arrivo")]
    public Transform traguardoTransform;

    [Header("Audio")]
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

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 1. CLICK DEL MOUSE
        if (Input.GetMouseButtonDown(0))
        {
            if (spinaCollider != null && spinaCollider.OverlapPoint(mouseWorldPos))
            {
                staTrascinando = true;
                offsetX = spinaCollider.transform.position.x - mouseWorldPos.x;
            }
        }

        // 2. TRASCINAMENTO
        if (staTrascinando && Input.GetMouseButton(0))
        {
            float nuovaX = mouseWorldPos.x + offsetX;

            // Limite sinistro (non entra nel muro)
            nuovaX = Mathf.Max(nuovaX, startX);

            // Applichiamo il movimento bloccando Y e Z
            spinaCollider.transform.position = new Vector3(nuovaX, fixedY, fixedZ);

            // 3. --- NUOVO CONTROLLO MATEMATICO DEL TRAGUARDO ---
            if (traguardoTransform != null)
            {
                // Se hai impostato un punto specifico della spina usa quello, altrimenti usa il centro del GameObject
                float xAttualeSpina = (puntoSpina != null) ? puntoSpina.position.x : spinaCollider.transform.position.x;

                // Se la X della spina ha superato (o è uguale) alla X del traguardo... VITTORIA!
                if (xAttualeSpina >= traguardoTransform.position.x)
                {
                    StaccaSpina();
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            staTrascinando = false;
        }
    }

    private void StaccaSpina()
    {
        taskFinito = true;
        staTrascinando = false;
        Debug.Log("Spina staccata superando la linea del Transform!");

        if (suonoStacco != null && audioSource != null)
        {
            audioSource.PlayOneShot(suonoStacco);
        }

        StartCoroutine(SequenzaVittoria());
    }

    private IEnumerator SequenzaVittoria()
    {
        yield return new WaitForSeconds(1f);

        if (taskManager != null) taskManager.CompletaTask();
        if (GameManager.Instance != null) GameManager.Instance.VinciMinigioco();
    }
}