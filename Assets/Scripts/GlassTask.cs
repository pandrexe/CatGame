using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class GlassMinigame : MonoBehaviour
{
    [Header("Impostazioni Minigioco")]
    public float distanzaCadutaDestra = 2.2f;
    public float distanzaCadutaSinistra = 2.2f;

    [Header("Audio")]
    public AudioClip suonoRottura;
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
        if (giaCaduto) return;
        if (GameManager.Instance != null && !GameManager.Instance.inMinigioco) return;

        float limiteVeroDestra = posizioneInizialeX + distanzaCadutaDestra;
        float limiteVeroSinistra = posizioneInizialeX - distanzaCadutaSinistra;

        // Cade = Vittoria! Non c'è più il fallimento del pavimento
        if (transform.position.x > limiteVeroDestra || transform.position.x < limiteVeroSinistra)
        {
            giaCaduto = true;
            StartCoroutine(GestisciVittoria());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!giaCaduto && suonoBotto != null) audioSource.PlayOneShot(suonoBotto);
    }

    private IEnumerator GestisciVittoria()
    {
        yield return new WaitForSeconds(1f);
        if (suonoRottura != null) audioSource.PlayOneShot(suonoRottura);

        yield return new WaitForSeconds(1f);
        GameManager.Instance.VinciMinigioco();
    }
}