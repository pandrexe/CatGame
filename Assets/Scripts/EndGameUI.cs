using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndGameUI : MonoBehaviour
{
    public static EndGameUI Instance;

    [Header("Pannelli (Oggetti UI)")]
    public GameObject pannelloTempoScaduto;
    public GameObject pannelloVittoria;
    public GameObject pannelloTaskMancanti;
    public GameObject pannelloViteFinite;

    [Header("Testi Dinamici")]
    public TextMeshProUGUI testoTempoImpiegato;
    public TextMeshProUGUI testoListaMancanti;

    [Header("Gestione Audio Finali")]
    [Tooltip("Trascina qui l'AudioSource che contiene la musica di background del gioco")]
    public AudioSource musicaDiSottofondo;
    [Tooltip("Trascina qui un NUOVO AudioSource dedicato solo ai suoni di fine partita")]
    public AudioSource audioJingleFinali;

    [Space(10)]
    public AudioClip jingleVittoria;
    public AudioClip jingleTempoScaduto;
    public AudioClip jingleViteFinite;
    public AudioClip jingleTaskMancanti;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (pannelloTempoScaduto) pannelloTempoScaduto.SetActive(false);
        if (pannelloVittoria) pannelloVittoria.SetActive(false);
        if (pannelloTaskMancanti) pannelloTaskMancanti.SetActive(false);
        if (pannelloViteFinite) pannelloViteFinite.SetActive(false);
    }

    // --- LA MAGIA DELL'AUDIO ---
    private void FermaMusicaESuonaJingle(AudioClip jingle)
    {
        // 1. Zittiamo la musica del gioco
        if (musicaDiSottofondo != null)
        {
            musicaDiSottofondo.Stop();
        }

        // 2. Facciamo suonare la clip corretta (ignorando il timeScale a 0)
        if (audioJingleFinali != null && jingle != null)
        {
            audioJingleFinali.ignoreListenerPause = true;
            audioJingleFinali.PlayOneShot(jingle);
        }
    }

    // --- LE FUNZIONI PER APRIRE I PANNELLI ---

    public void MostraTempoScaduto()
    {
        Time.timeScale = 0f;
        FermaMusicaESuonaJingle(jingleTempoScaduto);
        if (pannelloTempoScaduto) pannelloTempoScaduto.SetActive(true);
    }

    public void MostraViteFinite()
    {
        Time.timeScale = 0f;
        FermaMusicaESuonaJingle(jingleViteFinite);
        if (pannelloViteFinite) pannelloViteFinite.SetActive(true);
    }

    public void MostraVittoria(string tempo)
    {
        Time.timeScale = 0f;
        FermaMusicaESuonaJingle(jingleVittoria);
        // Mantenuta la tua formattazione esatta!
        if (testoTempoImpiegato != null) testoTempoImpiegato.text = $"{tempo} MINUTES!";
        if (pannelloVittoria) pannelloVittoria.SetActive(true);
    }

    public void MostraTaskMancanti(string lista)
    {
        Time.timeScale = 0f;
        FermaMusicaESuonaJingle(jingleTaskMancanti);
        if (testoListaMancanti != null) testoListaMancanti.text = lista;
        if (pannelloTaskMancanti) pannelloTaskMancanti.SetActive(true);
    }

    public void Ricomincia()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (GameManager.Instance != null)
        {
            // 1. Cerchiamo l'AudioSource dentro al GameManager e lo stoppiamo PRIMA di distruggerlo
            AudioSource audioGioco = GameManager.Instance.GetComponent<AudioSource>();
            if (audioGioco != null) audioGioco.Stop();

            // 2. Ora possiamo distruggerlo in sicurezza
            Destroy(GameManager.Instance.gameObject);
            GameManager.Instance = null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TornaAlMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (GameManager.Instance != null)
        {
            // 1. Zittiamo la musica del gioco PRIMA di cambiare scena
            AudioSource audioGioco = GameManager.Instance.GetComponent<AudioSource>();
            if (audioGioco != null) audioGioco.Stop();

            // 2. Ora lo distruggiamo
            Destroy(GameManager.Instance.gameObject);
            GameManager.Instance = null;
        }

        SceneManager.LoadScene("MainMenu");
    }
}