using UnityEngine;
using System.Collections;

public class LightSwitchTask : MonoBehaviour
{
    [Header("Riferimenti Manager")]
    public InteractableTask taskManager; // Obbligatorio!

    [Header("Riferimenti Click")]
    [Tooltip("Il Collider2D dell'interruttore da cliccare col mouse")]
    public Collider2D switchCollider;

    [Header("Oggetti Interruttore (Hierarchy)")]
    public GameObject interruttoreAccesoObj;
    public GameObject interruttoreSpentoObj;

    [Header("Audio")]
    public AudioClip suonoClick;

    private AudioSource audioSource;
    private bool taskFinito = false;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        // Reset del minigioco se si esce e rientra
        taskFinito = false;

        // All'inizio la luce/interruttore deve essere acceso (spento invisibile)
        if (interruttoreAccesoObj != null) interruttoreAccesoObj.SetActive(true);
        if (interruttoreSpentoObj != null) interruttoreSpentoObj.SetActive(false);
    }

    void Update()
    {
        // Se il task è già finito, ignora i click successivi
        if (taskFinito) return;

        // 1. CLICK DEL MOUSE (Tasto sinistro)
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Controlla se il mouse ha cliccato esattamente sopra il collider dell'interruttore
            if (switchCollider != null && switchCollider.OverlapPoint(mouseWorldPos))
            {
                PremiInterruttore();
            }
        }
    }

    private void PremiInterruttore()
    {
        taskFinito = true;

        // TOGGLE DEGLI OGGETTI: Spegni quello acceso, accendi quello spento!
        if (interruttoreAccesoObj != null) interruttoreAccesoObj.SetActive(false);
        if (interruttoreSpentoObj != null) interruttoreSpentoObj.SetActive(true);

        // Suono del "Click" (opzionale ma consigliato!)
        if (suonoClick != null && audioSource != null)
        {
            audioSource.PlayOneShot(suonoClick);
        }

        StartCoroutine(SequenzaVittoria());
    }

    private IEnumerator SequenzaVittoria()
    {
        // Aspetta esattamente 1 secondo per mostrare lo sprite cambiato
        yield return new WaitForSeconds(1f);

        // Vittoria!
        if (taskManager != null) taskManager.CompletaTask();
        if (GameManager.Instance != null) GameManager.Instance.VinciMinigioco();
    }
}