using UnityEngine;
using System.Collections;

public class PianoTask : MonoBehaviour
{
    [System.Serializable]
    public class TastoPiano
    {
        public string nomeNota;
        public Collider2D colliderTasto;
        public AudioClip suonoNota;
    }

    [Header("Riferimento Manager (Obbligatorio)")]
    public InteractableTask taskManager;

    [Header("Tutti i Tasti (Per strimpellare)")]
    public TastoPiano[] tuttiITasti;

    [Header("L'Obiettivo: La Scala Musicale")]
    public Collider2D[] sequenzaVincente;

    // --- NOVITÀ: SUONI DI STATO ---
    [Header("Audio del Minigioco")]
    [Tooltip("Il suono di 'errore' (es. un buu o un suono sgraziato) quando si sbaglia la nota.")]
    public AudioClip suonoErrore;
    [Tooltip("Il jingle o suono di vittoria che parte APPARENA completi l'intera scala.")]
    public AudioClip suonoVittoriaTask;

    private int indiceNotaAttuale = 0;
    private AudioSource audioSource;
    private bool taskFinito = false;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        indiceNotaAttuale = 0;
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
                        // 1. Suona SEMPRE la nota che il gatto ha fisicamente calpestato
                        if (tuttiITasti[i].suonoNota != null)
                        {
                            audioSource.PlayOneShot(tuttiITasti[i].suonoNota);
                        }

                        // 2. Controllo della sequenza
                        if (tuttiITasti[i].colliderTasto == sequenzaVincente[indiceNotaAttuale])
                        {
                            indiceNotaAttuale++;
                            Debug.Log($"Nota corretta! Progressi: {indiceNotaAttuale}/{sequenzaVincente.Length}");

                            if (indiceNotaAttuale >= sequenzaVincente.Length)
                            {
                                FineTask();
                            }
                        }
                        else
                        {
                            // --- LOGICA DI ERRORE MODIFICATA ---
                            // Se il gatto preme il primo Do della scala, intende ricominciare da capo. 
                            // In quel caso NON facciamo partire il suono di errore, resettiamo solo a 1.
                            if (tuttiITasti[i].colliderTasto == sequenzaVincente[0])
                            {
                                indiceNotaAttuale = 1;
                                Debug.Log("Hai ricominciato la scala dal primo Do.");
                            }
                            else
                            {
                                // Ha premuto una nota completamente fuori sequenza: SUONA L'ERRORE!
                                indiceNotaAttuale = 0;
                                Debug.Log("Nota sbagliata! Sequenza azzerata.");

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
        Debug.Log("SCALA COMPLETATA! VITTORIA!");
  
        StartCoroutine(SequenzaVittoria());
    }

    private IEnumerator SequenzaVittoria()
    {
        
        yield return new WaitForSeconds(1.0f);


        if (suonoVittoriaTask != null)
        {
            audioSource.PlayOneShot(suonoVittoriaTask);
        }

        yield return new WaitForSeconds(1.0f);

        if (taskManager != null) taskManager.CompletaTask();
        if (GameManager.Instance != null) GameManager.Instance.VinciMinigioco();
    }
}