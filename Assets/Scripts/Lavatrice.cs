using UnityEngine;

// Eredita da Interactable, così hai già gratis la UI e il tasto E!
[RequireComponent(typeof(AudioSource))]
public class LavatriceManager : Interactable
{
    [Header("Riferimenti Lavatrice")]
    public SpawnerBolle spawnerBolle; // Trascina qui lo script SpawnerBolle
    private SpriteRenderer spriteRenderer;

    [Header("Audio")]
    public AudioClip suonoLavatriceAccesa;
    public AudioClip suonoSpegnimento;
    private AudioSource audioSource;

    private bool giaSpenta = false;

    protected override void Start()
    {
        // Chiama lo start di Interactable per spegnere la UI all'inizio
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        // Facciamo partire il suono continuo della lavatrice
        if (suonoLavatriceAccesa != null)
        {
            audioSource.clip = suonoLavatriceAccesa;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    // Questo scatta in automatico quando il gatto è vicino e premi 'E' (grazie a Interactable!)
    protected override void EseguiInterazione()
    {
        if (giaSpenta) return;

        // Qui è dove fai partire il tuo minigioco vero e proprio tramite il GameManager
        // Esempio: GameManager.Instance.AvviaMinigioco(TaskType.SpegniLavatrice);

        Debug.Log("Minigioco Lavatrice Avviato!");

        // NOTA: Se il minigioco non è ancora pronto e vuoi testare lo spegnimento subito,
        // puoi chiamare direttamente la funzione SpegniLavatrice() qui sotto.
    }

    // Questa funzione la chiamerai quando il gatto VINCE il minigioco
    public void SpegniLavatrice()
    {
        if (giaSpenta) return;

        giaSpenta = true;
        puoInteragire = false; // Non fa più apparire la scritta "Premi E"

        // 1. BLOCCA LE BOLLE (spegne lo script SpawnerBolle)
        if (spawnerBolle != null)
        {
            spawnerBolle.enabled = false;
        }

        // 3. GESTISCI L'AUDIO
        audioSource.Stop(); // Ferma il loop
        if (suonoSpegnimento != null)
        {
            audioSource.PlayOneShot(suonoSpegnimento); // Suona il "clack" di spegnimento
        }

        // 4. COMUNICA LA VITTORIA AL GIOCO
        if (GameManager.Instance != null)
        {
            GameManager.Instance.VinciMinigioco();
        }
    }
}