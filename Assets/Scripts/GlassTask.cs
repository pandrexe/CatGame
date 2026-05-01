using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))] // Ora esigiamo la fisica!
public class GlassMinigame : MonoBehaviour
{
    [Header("Impostazioni Minigioco (Bordi Tavolo)")]
    public float distanzaCadutaDestra = 2.71f; 
    public float distanzaCadutaSinistra = 2.71f; 
    
    [Header("Condizione di Vittoria (Puzzle)")]
    public bool cuscinoPiazzato = false; 

    [Header("Audio")]
    public AudioClip suonoRottura;      
    public AudioClip suonoSalvataggio;  
    public AudioClip suonoBotto; // Opzionale: il rumore quando la zampa colpisce il vetro

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
        if (giaCaduto) return;
        if (GameManager.Instance != null && !GameManager.Instance.inMinigioco) return;

        float limiteVeroDestra = posizioneInizialeX + distanzaCadutaDestra;
        float limiteVeroSinistra = posizioneInizialeX - distanzaCadutaSinistra;

        // L'Update fa il suo lavoro: controlla se il bicchiere è stato sbattuto fuori dai limiti!
        if (transform.position.x > limiteVeroDestra || transform.position.x < limiteVeroSinistra)
        {
            giaCaduto = true;
            StartCoroutine(GestisciCaduta());
        }
    }

    // Aggiungiamo un feedback sonoro quando diamo lo "schiaffo" al bicchiere
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!giaCaduto && suonoBotto != null)
        {
            audioSource.PlayOneShot(suonoBotto);
        }
    }

    private IEnumerator GestisciCaduta()
    {
        yield return new WaitForSeconds(0.5f);
        if (cuscinoPiazzato)
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