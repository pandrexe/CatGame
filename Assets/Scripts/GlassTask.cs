using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class GlassMinigame : MonoBehaviour
{
    [Header("Impostazioni Minigioco (Bordi Tavolo)")]
    [Tooltip("Distanza verso DESTRA per farlo cadere (es: 2.71)")]
    public float distanzaCadutaDestra = 2.71f; 
    [Tooltip("Distanza verso SINISTRA per farlo cadere (es: 2.71)")]
    public float distanzaCadutaSinistra = 2.71f; 
    
    [Header("Condizione di Vittoria (Puzzle)")]
    [Tooltip("Se è true, il bicchiere cade sul morbido. Se è false, si rompe e perdi.")]
    public bool cuscinoPiazzato = false; 

    [Header("Audio")]
    public AudioClip suonoRottura;      
    public AudioClip suonoSalvataggio;  

    private Vector3 offset;
    private bool giaCaduto = false;
    private AudioSource audioSource;

    private float posizioneInizialeX;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        posizioneInizialeX = transform.position.x;
    }

    void OnMouseDown()
    {
        if (giaCaduto) return;

        if (GameManager.Instance != null && !GameManager.Instance.inMinigioco) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - mousePosition;
    }

    void OnMouseDrag()
    {
        if (giaCaduto) return;
        if (GameManager.Instance != null && !GameManager.Instance.inMinigioco) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePosition.x + offset.x, mousePosition.y + offset.y, transform.position.z);
    }

    void Update()
    {
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