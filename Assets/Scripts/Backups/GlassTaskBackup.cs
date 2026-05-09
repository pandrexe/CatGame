/*
using UnityEngine;
using System.Collections;
// Abbiamo persino rimosso "using UnityEngine.SceneManagement;" perché non serve più qui!

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class GlassMinigame : MonoBehaviour
{
    [Header("Impostazioni Minigioco (Bordi Tavolo)")]
    public float distanzaCadutaDestra = 2.2f;
    public float distanzaCadutaSinistra = 2.2f;

    // RIMOSSA TUTTA LA SEZIONE DEL RAYCAST! Ora questo script pensa solo a cadere.

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
        // RIMOSSO IL CONTROLLO DEL RAYCAST NELL'UPDATE!

        if (giaCaduto) return;
        if (GameManager.Instance != null && !GameManager.Instance.inMinigioco) return;

        float limiteVeroDestra = posizioneInizialeX + distanzaCadutaDestra;
        float limiteVeroSinistra = posizioneInizialeX - distanzaCadutaSinistra;

        // Controlla se il bicchiere è stato sbattuto fuori dai limiti
        if (transform.position.x > limiteVeroDestra || transform.position.x < limiteVeroSinistra)
        {
            giaCaduto = true;
            StartCoroutine(GestisciCaduta());
        }
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
            yield return new WaitForSeconds(1f);
            GameManager.Instance.VinciMinigioco();
        }
        else
        {
            if (suonoRottura != null) audioSource.PlayOneShot(suonoRottura);
            yield return new WaitForSeconds(1f);

            // --- ECCO LA MODIFICA: Ora passa per il sistema Roguelite! ---
            GameManager.Instance.FallisciTask();
        }
    }
}
*/