using UnityEngine;
using System.Collections;

public class CurtainsTask : MonoBehaviour
{
    [Header("Riferimenti Manager")]
    public InteractableTask taskManager;

    [Header("Le Tende del Minigioco")]
    public Collider2D tendaSX_Minigioco;
    public Collider2D tendaDX_Minigioco;

    [Header("I Trigger di Vittoria")]
    [Tooltip("Un collider trigger posizionato al centro, dove deve arrivare la tenda SX")]
    public Collider2D zonaVittoriaSX;
    [Tooltip("Un collider trigger posizionato al centro, dove deve arrivare la tenda DX")]
    public Collider2D zonaVittoriaDX;

    [Header("Audio")]
    public AudioClip suonoScorrimentoTenda;

    private bool sxBloccata = false;
    private bool dxBloccata = false;
    private bool taskFinito = false;

    private Transform tendaInTrascinamento;
    private float offsetX;
    private float startXSX;
    private float startXDX;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // Disattivato all'inizio per sicurezza
        
        if (tendaSX_Minigioco != null) startXSX = tendaSX_Minigioco.transform.position.x;
        if (tendaDX_Minigioco != null) startXDX = tendaDX_Minigioco.transform.position.x;
    }

    void OnEnable()
    {
        if (tendaSX_Minigioco != null) tendaSX_Minigioco.transform.position = new Vector3(startXSX, tendaSX_Minigioco.transform.position.y, tendaSX_Minigioco.transform.position.z);
        if (tendaDX_Minigioco != null) tendaDX_Minigioco.transform.position = new Vector3(startXDX, tendaDX_Minigioco.transform.position.y, tendaDX_Minigioco.transform.position.z);

        sxBloccata = false;
        dxBloccata = false;
        taskFinito = false;
        tendaInTrascinamento = null;

        // Ci assicuriamo che l'audio sia muto all'avvio
        if (audioSource != null) audioSource.Stop();
    }

    void Update()
    {
        if (taskFinito) return;

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 1. PRENDI LA TENDA E FAI PARTIRE IL SUONO
        if (Input.GetMouseButtonDown(0))
        {
            if (!sxBloccata && tendaSX_Minigioco != null && tendaSX_Minigioco.OverlapPoint(mouseWorldPos))
            {
                tendaInTrascinamento = tendaSX_Minigioco.transform;
                offsetX = tendaSX_Minigioco.transform.position.x - mouseWorldPos.x;
                RiproduciSuonoScorrimento();
            }
            else if (!dxBloccata && tendaDX_Minigioco != null && tendaDX_Minigioco.OverlapPoint(mouseWorldPos))
            {
                tendaInTrascinamento = tendaDX_Minigioco.transform;
                offsetX = tendaDX_Minigioco.transform.position.x - mouseWorldPos.x;
                RiproduciSuonoScorrimento();
            }
        }

        // 2. TRASCINA LA TENDA (Con i muri invisibili)
        if (Input.GetMouseButton(0) && tendaInTrascinamento != null)
        {
            float nuovaX = mouseWorldPos.x + offsetX;

            if (tendaInTrascinamento == tendaSX_Minigioco.transform)
            {
                // Muro invisibile: la tenda SX non va oltre la sua zona di vittoria a destra
                nuovaX = Mathf.Clamp(nuovaX, startXSX, zonaVittoriaSX.transform.position.x);
            }
            else if (tendaInTrascinamento == tendaDX_Minigioco.transform)
            {
                // Muro invisibile: la tenda DX non va oltre la sua zona di vittoria a sinistra
                nuovaX = Mathf.Clamp(nuovaX, zonaVittoriaDX.transform.position.x, startXDX);
            }

            tendaInTrascinamento.position = new Vector3(nuovaX, tendaInTrascinamento.position.y, tendaInTrascinamento.position.z);
        }

        // 3. RILASCIA IL MOUSE, FERMA IL SUONO E CONTROLLA LA VITTORIA
        if (Input.GetMouseButtonUp(0) && tendaInTrascinamento != null)
        {
            FermaSuonoScorrimento();
            ControllaVittoriaRilascio();
            tendaInTrascinamento = null;
        }
    }

    // --- NUOVE FUNZIONI AUDIO ---

    private void RiproduciSuonoScorrimento()
    {
        if (suonoScorrimentoTenda != null && audioSource != null)
        {
            audioSource.clip = suonoScorrimentoTenda;
            audioSource.loop = true; // Il suono si ripete mentre trascini
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    private void FermaSuonoScorrimento()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void ControllaVittoriaRilascio()
    {
        // Se la tenda viene rilasciata DENTRO al collider trigger, si blocca lì dov'è!
        if (tendaInTrascinamento == tendaSX_Minigioco.transform && tendaSX_Minigioco.bounds.Intersects(zonaVittoriaSX.bounds))
        {
            BloccaTenda(true);
        }
        else if (tendaInTrascinamento == tendaDX_Minigioco.transform && tendaDX_Minigioco.bounds.Intersects(zonaVittoriaDX.bounds))
        {
            BloccaTenda(false);
        }
    }

    private void BloccaTenda(bool isSinistra)
    {
        // Nessun teletrasporto (snap)! Semplicemente segnaliamo la tenda come bloccata.
        if (isSinistra)
        {
            sxBloccata = true;
            Debug.Log("Tenda Sinistra bloccata in posizione!");
        }
        else
        {
            dxBloccata = true;
            Debug.Log("Tenda Destra bloccata in posizione!");
        }

        if (sxBloccata && dxBloccata)
        {
            taskFinito = true;
            Debug.Log("Minigioco Tende Completato!");
            StartCoroutine(ChiusuraMinigioco());
        }
    }

    private IEnumerator ChiusuraMinigioco()
    {
        yield return new WaitForSeconds(1f);
        if (taskManager != null) taskManager.CompletaTask();
        if (GameManager.Instance != null) GameManager.Instance.VinciMinigioco();
    }
}