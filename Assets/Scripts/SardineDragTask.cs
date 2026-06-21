using UnityEngine;

public class SardineDragTask : MonoBehaviour
{
    public InteractableTask taskManager;
    public Transform sardina; 
    public float raggioPresa = 1.5f; 

    [Header("Audio")]
    [Tooltip("Suono viscido o di presa quando il gatto clicca sulla sardina")]
    public AudioClip suonoPresaSardina;
    [Tooltip("Suono di successo (miagolio o gnam) quando la sardina esce dallo schermo")]
    public AudioClip suonoVittoriaSardina;

    private bool isDragging = false;
    private bool taskFinito = false;
    private AudioSource audioSource;

    void Awake()
    {
        // Creiamo l'AudioSource in automatico
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        
        if (!GameManager.Instance.inMinigioco)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.LogWarning("Sardina: Non posso fare nulla perché GameManager.Instance.inMinigioco è FALSE!");
            }
            return;
        }

        if (taskFinito) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // Click del mouse
        if (Input.GetMouseButtonDown(0))
        {
            if (sardina == null)
            {
                Debug.LogError("Sardina: ERRORE! L'oggetto della Sardina non è assegnato nell'Inspector dello script!");
                return;
            }

            float distanza = Vector2.Distance(mousePos, sardina.position);
            
            Debug.Log("Sardina: Click rilevato! Mouse: " + mousePos + " | Sardina: " + sardina.position + " | Distanza: " + distanza + " | Raggio Richiesto: " + raggioPresa);

            if (distanza <= raggioPresa)
            {
                isDragging = true;
                Debug.Log("Sardina: PRESA CON SUCCESSO! Sto trascinando...");

                // --- AUDIO: Suona quando la afferri ---
                if (suonoPresaSardina != null && audioSource != null)
                {
                    audioSource.PlayOneShot(suonoPresaSardina);
                }
            }
            else
            {
                Debug.LogWarning("Sardina: Cliccato troppo lontano! Aumenta il valore di 'Raggio Presa' nell'Inspector.");
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                isDragging = false;
                Debug.Log("Sardina: Lasciata!");
            }
        }

        if (isDragging && sardina != null)
        {
            sardina.position = mousePos;

            Vector3 viewportPos = Camera.main.WorldToViewportPoint(sardina.position);

            if (viewportPos.x <= 0.05f || viewportPos.x >= 0.95f || 
                viewportPos.y <= 0.05f || viewportPos.y >= 0.95f)
            {
                Debug.Log("Sardina: Rilevato bordo dello schermo! Viewport: " + viewportPos);
                FineTask();
            }
        }
    }

    private void FineTask()
    {
        taskFinito = true;
        isDragging = false;
        
        if (sardina != null)
        {
            sardina.gameObject.SetActive(false);
        }

        Debug.Log("Sardina portata fuori! Task completato.");

        // --- AUDIO: Suona il jingle di successo/miagolio ---
        if (suonoVittoriaSardina != null && audioSource != null)
        {
            audioSource.PlayOneShot(suonoVittoriaSardina);
        }

        if (taskManager != null) 
        {
            taskManager.CompletaTask();
        }
        else
        {
            Debug.LogError("Sardina: ATTENZIONE! Manca il riferimento a Task Manager nell'Inspector!");
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.VinciMinigioco();
        }
    }
}