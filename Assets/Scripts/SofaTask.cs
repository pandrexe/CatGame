using UnityEngine;
using System.Collections;

public class ScratchSofaTask : MonoBehaviour
{
    [Header("Riferimenti Manager")]
    public InteractableTask taskManager; // Obbligatorio!

    [Tooltip("Metti il collider sull'oggetto padre (TaskDivano) così non scompare mai!")]
    public Collider2D colliderDivano;

    [Header("Gli Oggetti nella Hierarchy")]
    public GameObject divanoPulitoObj;
    public GameObject divanoSemigraffiatoObj;
    public GameObject divanoDistruttoObj;

    [Header("Impostazioni Difficoltà")]
    [Tooltip("Quanti click totali (alternando le zampe) servono per passare a Semi-graffiato?")]
    public int clickPerSemiGraffio = 2;
    [Tooltip("Quanti click totali servono per passare a Distrutto e vincere?")]
    public int clickPerDistruzione = 6;

    [Header("Audio")]
    public AudioClip suonoGraffio;

    private AudioSource audioSource;
    private bool taskFinito = false;

    // Logica delle zampe
    private int prossimoTastoAtteso = 0; // 0 = Sinistro, 1 = Destro
    private bool zampeInizializzate = false;

    // Contatore di gioco
    private int graffiAttuali = 0;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        // Reset totale del minigioco se rientri
        taskFinito = false;
        graffiAttuali = 0;
        prossimoTastoAtteso = 0;
        zampeInizializzate = false;

        // Imposta la visibilità corretta per l'inizio del minigioco
        if (divanoPulitoObj != null) divanoPulitoObj.SetActive(true);
        if (divanoSemigraffiatoObj != null) divanoSemigraffiatoObj.SetActive(false);
        if (divanoDistruttoObj != null) divanoDistruttoObj.SetActive(false);

        ImpostaVisibilitaZampe(true, false);
    }

    void Update()
    {
        if (taskFinito) return;

        if (!zampeInizializzate)
        {
            ImpostaVisibilitaZampe(true, false);
            prossimoTastoAtteso = 0;
            zampeInizializzate = true;
        }

        // 1. ZAMPA SINISTRA (Tasto Sinistro = 0)
        if (Input.GetMouseButtonDown(0) && prossimoTastoAtteso == 0)
        {
            if (MinigameCursor.Instance != null && MinigameCursor.Instance.zampaSx != null)
            {
                Vector2 posZampaSx = MinigameCursor.Instance.zampaSx.position;
                if (colliderDivano != null && colliderDivano.OverlapPoint(posZampaSx))
                {
                    EseguiGraffio();
                    prossimoTastoAtteso = 1;
                    ImpostaVisibilitaZampe(false, true);
                }
            }
        }
        // 2. ZAMPA DESTRA (Tasto Destro = 1)
        else if (Input.GetMouseButtonDown(1) && prossimoTastoAtteso == 1)
        {
            if (MinigameCursor.Instance != null && MinigameCursor.Instance.zampaDx != null)
            {
                Vector2 posZampaDx = MinigameCursor.Instance.zampaDx.position;
                if (colliderDivano != null && colliderDivano.OverlapPoint(posZampaDx))
                {
                    EseguiGraffio();
                    prossimoTastoAtteso = 0;
                    ImpostaVisibilitaZampe(true, false);
                }
            }
        }
    }

    private void EseguiGraffio()
    {
        if (suonoGraffio != null && audioSource != null)
        {
            audioSource.PlayOneShot(suonoGraffio);
        }

        graffiAttuali++;

        if (graffiAttuali >= clickPerDistruzione)
        {
            // FASE 2: DISTRUTTO (Spegni il semi, accendi il distrutto)
            if (divanoSemigraffiatoObj != null) divanoSemigraffiatoObj.SetActive(false);
            if (divanoDistruttoObj != null) divanoDistruttoObj.SetActive(true);

            FineTask();
        }
        else if (graffiAttuali >= clickPerSemiGraffio && graffiAttuali < clickPerDistruzione)
        {
            // FASE 1: SEMI-GRAFFIATO (Spegni il pulito, accendi il semi)
            if (divanoPulitoObj != null) divanoPulitoObj.SetActive(false);
            if (divanoSemigraffiatoObj != null) divanoSemigraffiatoObj.SetActive(true);
        }
    }

    private void ImpostaVisibilitaZampe(bool visibileSx, bool visibileDx)
    {
        if (MinigameCursor.Instance == null) return;
        if (MinigameCursor.Instance.zampaSx != null) MinigameCursor.Instance.zampaSx.GetComponent<SpriteRenderer>().enabled = visibileSx;
        if (MinigameCursor.Instance.zampaDx != null) MinigameCursor.Instance.zampaDx.GetComponent<SpriteRenderer>().enabled = visibileDx;
    }

    void FineTask()
    {
        taskFinito = true;
        ImpostaVisibilitaZampe(false, false);
        StartCoroutine(SequenzaVittoria());
    }

    private IEnumerator SequenzaVittoria()
    {
        yield return new WaitForSeconds(1f);

        if (taskManager != null) taskManager.CompletaTask();
        if (GameManager.Instance != null) GameManager.Instance.VinciMinigioco();
    }
}