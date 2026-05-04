using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class GlassMinigame : MonoBehaviour
{
    [Header("Impostazioni Minigioco (Bordi Tavolo)")]
    public float distanzaCadutaDestra = 2.2f; 
    public float distanzaCadutaSinistra = 2.2f; 
    
    [Header("Controllo Cuscino (Raycast)")]
    [Tooltip("Quanto deve essere lungo il raggio che controlla il pavimento?")]
    public float lunghezzaRaycast = 5f;
    [Tooltip("Spunta vera se il raggio colpisce il cuscino!")]
    public bool cuscinoPiazzatoSottoBicchiere = false; 

    [Header("Audio")]
    public AudioClip suonoRottura;      
    public AudioClip suonoSalvataggio;  
    public AudioClip suonoBotto; 

    private bool giaCaduto = false;
    private AudioSource audioSource;
    private float posizioneInizialeX;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        posizioneInizialeX = transform.position.x;
    }

    void Update()
    {
        // Controlliamo sempre se il cuscino è sotto (anche prima e durante il minigioco!)
        ControllaCuscinoSotto();

        if (giaCaduto) return;
        if (GameManager.Instance != null && !GameManager.Instance.inMinigioco) return;

        float limiteVeroDestra = posizioneInizialeX + distanzaCadutaDestra;
        float limiteVeroSinistra = posizioneInizialeX - distanzaCadutaSinistra;

        if (transform.position.x > limiteVeroDestra || transform.position.x < limiteVeroSinistra)
        {
            giaCaduto = true;
            StartCoroutine(GestisciCaduta());
        }
    }

    private void ControllaCuscinoSotto()
    {
        // 1. Disegna la linea rossa nella visuale Scene (per farti vedere fin dove arriva)
        Debug.DrawRay(transform.position, Vector2.down * lunghezzaRaycast, Color.red);

        // 2. Spara un raggio che attraversa TUTTI i collider sulla sua strada
        RaycastHit2D[] oggettiColpiti = Physics2D.RaycastAll(transform.position, Vector2.down, lunghezzaRaycast);
        
        // Partiamo dal presupposto che il cuscino non ci sia
        bool trovatoCuscino = false;

        // 3. Controlliamo uno ad uno tutti gli oggetti trapassati dal laser
        foreach (RaycastHit2D hit in oggettiColpiti)
        {
            if (hit.collider != null && hit.collider.CompareTag("Cuscino"))
            {
                trovatoCuscino = true;
                break; // Trovato! Inutile continuare a controllare gli altri oggetti
            }
        }

        // 4. Aggiorniamo il nostro booleano
        cuscinoPiazzatoSottoBicchiere = trovatoCuscino;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!giaCaduto && suonoBotto != null)
        {
            audioSource.PlayOneShot(suonoBotto);
        }
    }

    private IEnumerator GestisciCaduta()
    {
        yield return new WaitForSeconds(1f);
        
        // 1. Cerchiamo il bicchiere VERO nel salotto
        Bicchiere bicchiereSalotto = Object.FindFirstObjectByType<Bicchiere>();
        
        // 2. Gli chiediamo se il cuscino è piazzato sotto di lui
        bool salvato = (bicchiereSalotto != null && bicchiereSalotto.cuscinoPiazzatoSotto);

        if (salvato)
        {
            if (suonoSalvataggio != null) audioSource.PlayOneShot(suonoSalvataggio);
            GameManager.Instance.VinciMinigioco();
        }
        else
        {
            if (suonoRottura != null) audioSource.PlayOneShot(suonoRottura);
            yield return new WaitForSeconds(1f);
            Debug.Log("CRASH! Hai svegliato il padrone! GAME OVER.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }


}