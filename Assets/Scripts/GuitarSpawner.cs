using UnityEngine;

public class GuitarSpawner : MonoBehaviour
{
    [Header("Impostazioni Spawn")]
    [Tooltip("Trascina qui il PREFAB della nota musicale")]
    public GameObject prefabNota;
    [Tooltip("Ogni quanti secondi la chitarra deve sparare una nota?")]
    public float tempoTraGliSpari = 3f;
    [Tooltip("Velocità di volo della nota musicale")]
    public float velocitaNota = 5f;

    [Header("Audio")]
    [Tooltip("Inserisci qui il suono della chitarra che spara")]
    public AudioClip suonoSparo; // <--- ECCO LA TUA CLIP AUDIO

    private Transform gatto;
    private float timerSparo = 0f;
    private AudioSource audioSource;

    void Awake()
    {
        // Generiamo in automatico la "cassa acustica" sulla chitarra all'avvio
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.gatto != null)
        {
            gatto = GameManager.Instance.gatto.transform;
        }
    }

    void Update()
    {
        // Se siamo in un minigioco o il gatto non esiste, la chitarra smette di sparare
        if (gatto == null || (GameManager.Instance != null && GameManager.Instance.inMinigioco))
            return;

        timerSparo += Time.deltaTime;

        if (timerSparo >= tempoTraGliSpari)
        {
            SparaNota();
            timerSparo = 0f; // Resetta il timer
        }
    }

    void SparaNota()
    {
        if (prefabNota == null)
        {
            Debug.LogWarning("[GuitarSpawner] Manca il prefab della nota nell'Inspector!");
            return;
        }

        // --- RIPRODUZIONE AUDIO ---
        // Se hai assegnato un suono, fallo partire!
        if (suonoSparo != null && audioSource != null)
        {
            audioSource.PlayOneShot(suonoSparo);
        }

        // 1. Calcola la direzione verso il gatto
        Vector2 direzioneVersoGatto = gatto.position - transform.position;

        // 2. Genera la nota
        GameObject nuovaNota = Instantiate(prefabNota, transform.position, Quaternion.identity);

        // 3. Passa direzione e velocità alla nota
        MusicNote scriptNota = nuovaNota.GetComponent<MusicNote>();
        if (scriptNota != null)
        {
            scriptNota.InizializzaNota(direzioneVersoGatto, velocitaNota);
        }
    }
}