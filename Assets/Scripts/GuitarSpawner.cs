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
    public AudioClip suonoSparo;

    private Transform gatto;
    private float timerSparo = 0f;
    private AudioSource audioSource;

    // --- IL LUCCHETTO ---
    private bool eMortoDefinitivamente = false;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // --- INTERCETTA IL ROOM TRIGGER ---
    void OnEnable()
    {
        if (eMortoDefinitivamente)
        {
            this.enabled = false; // Se la spina è staccata, si rifiuta di riaccendersi!
        }
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
        if (gatto == null || (GameManager.Instance != null && GameManager.Instance.inMinigioco))
            return;

        timerSparo += Time.deltaTime;

        if (timerSparo >= tempoTraGliSpari)
        {
            SparaNota();
            timerSparo = 0f;
        }
    }

    void SparaNota()
    {
        if (prefabNota == null)
        {
            Debug.LogWarning("[GuitarSpawner] Manca il prefab della nota nell'Inspector!");
            return;
        }

        if (suonoSparo != null && audioSource != null)
        {
            audioSource.PlayOneShot(suonoSparo);
        }

        Vector2 direzioneVersoGatto = gatto.position - transform.position;
        GameObject nuovaNota = Instantiate(prefabNota, transform.position, Quaternion.identity);

        MusicNote scriptNota = nuovaNota.GetComponent<MusicNote>();
        if (scriptNota != null)
        {
            scriptNota.InizializzaNota(direzioneVersoGatto, velocitaNota);
        }
    }

    // --- LA FUNZIONE DA CHIAMARE ALLA VITTORIA ---
    public void SpegnimentoDefinitivo()
    {
        eMortoDefinitivamente = true;
        this.enabled = false;
    }
}