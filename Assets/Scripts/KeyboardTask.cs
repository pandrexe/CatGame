using UnityEngine;
using System.Collections;

public class KeyboardTask : MonoBehaviour
{
    [System.Serializable]
    public class TastoTastiera
    {
        public string nomeTasto; // Es: "Tasto 1", "Tasto 2"
        public Collider2D colliderTasto;
    }

    [Header("Riferimento Manager (Obbligatorio)")]
    public InteractableTask taskManager;

    [Header("Audio Globale della Tastiera")]
    [Tooltip("Il rumore del tasto meccanico (uguale per tutti).")]
    public AudioClip suonoClickGenerico;
    [Tooltip("Il suono di errore quando si sbaglia la sequenza (uno solo per tutti).")]
    public AudioClip suonoErrore;
    [Tooltip("Il suono di vittoria quando si inserisce la password corretta.")]
    public AudioClip suonoVittoriaTask;

    [Header("Tutti i Tasti (Per fare rumore a caso)")]
    public TastoTastiera[] tuttiITasti;

    [Header("L'Obiettivo: La Password")]
    public Collider2D[] sequenzaVincente;

    private int indiceTastoAttuale = 0;
    private AudioSource audioSource;
    private bool taskFinito = false;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        indiceTastoAttuale = 0;
        taskFinito = false;
    }

    void Update()
    {
        if (taskFinito) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (MinigameCursor.Instance != null && MinigameCursor.Instance.contenitoreSingola != null)
            {
                Vector2 posZampa = MinigameCursor.Instance.contenitoreSingola.transform.position;

                for (int i = 0; i < tuttiITasti.Length; i++)
                {
                    if (tuttiITasti[i].colliderTasto != null && tuttiITasti[i].colliderTasto.OverlapPoint(posZampa))
                    {
                        // 1. Suona il click generico
                        if (suonoClickGenerico != null)
                        {
                            audioSource.PlayOneShot(suonoClickGenerico);
                        }

                        // 2. Controllo della sequenza della password
                        if (tuttiITasti[i].colliderTasto == sequenzaVincente[indiceTastoAttuale])
                        {
                            indiceTastoAttuale++;
                            Debug.Log($"Tasto corretto! Progressi: {indiceTastoAttuale}/{sequenzaVincente.Length}");

                            if (indiceTastoAttuale >= sequenzaVincente.Length)
                            {
                                FineTask();
                            }
                        }
                        else
                        {
                            // Se premiamo il tasto 1 (inizio della sequenza) ricominciamo lisci
                            if (tuttiITasti[i].colliderTasto == sequenzaVincente[0])
                            {
                                indiceTastoAttuale = 1;
                                Debug.Log("Hai ricominciato la password dal primo tasto.");
                            }
                            else
                            {
                                // Errore: azzera la sequenza e fa partire l'unico suono di errore globale
                                indiceTastoAttuale = 0;
                                Debug.Log("Password errata! Sequenza azzerata.");

                                if (suonoErrore != null)
                                {
                                    audioSource.PlayOneShot(suonoErrore);
                                }
                            }
                        }

                        break;
                    }
                }
            }
        }
    }

    private void FineTask()
    {
        taskFinito = true;
        Debug.Log("PASSWORD ACCETTATA! VITTORIA!");
  
        StartCoroutine(SequenzaVittoria());
    }

    private IEnumerator SequenzaVittoria()
    {
        yield return new WaitForSeconds(0.5f);

        if (suonoVittoriaTask != null)
        {
            audioSource.PlayOneShot(suonoVittoriaTask);
        }

        yield return new WaitForSeconds(1.0f);

        if (taskManager != null) taskManager.CompletaTask();
        if (GameManager.Instance != null) GameManager.Instance.VinciMinigioco();
    }
}
