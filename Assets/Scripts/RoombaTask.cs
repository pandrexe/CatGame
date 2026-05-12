using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class RoombaMinigame : MonoBehaviour
{
    [Header("Sprite del Roomba")]
    public Sprite spriteAcceso;
    public Sprite spriteSpento;

    [Header("Audio del Minigioco")]
    public AudioClip suonoAspirapolvere;
    public AudioClip suonoSpegnimento;

    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private bool giaSpento = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = gameObject.AddComponent<AudioSource>();
        spriteRenderer.sprite = spriteAcceso;

        // Impostiamo l'audio ma NON LO FACCIAMO PARTIRE QUI!
        if (suonoAspirapolvere != null)
        {
            audioSource.clip = suonoAspirapolvere;
            audioSource.loop = true;
            audioSource.volume = 1f;
        }
    }

    void Update()
    {
        // Se è già stato spento o manca la clip, si ferma
        if (giaSpento || audioSource.clip == null) return;

        // Il minigioco capisce che è il SUO turno controllando se la telecamera è arrivata da lui
        bool stiamoGiocando = GameManager.Instance != null && GameManager.Instance.inMinigioco;
        bool telecameraVicina = Vector2.Distance(transform.position, Camera.main.transform.position) < 5f;

        // SE siamo nel minigioco E la telecamera è sul Roomba -> ACCENDI L'AUDIO!
        if (stiamoGiocando && telecameraVicina)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        // ALTRIMENTI (Siamo in giro per casa o in un altro minigioco) -> SPEGNI L'AUDIO!
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance != null && !GameManager.Instance.inMinigioco) return;
        if (giaSpento) return;

        giaSpento = true;
        spriteRenderer.sprite = spriteSpento;

        StartCoroutine(ConcludiMinigioco());
    }

    private IEnumerator ConcludiMinigioco()
    {
        yield return new WaitForSeconds(1f);
        // Quando lo colpisci, stoppa il rumore dell'aspirapolvere e fai partire lo schiaffo
        audioSource.Stop();
        if (suonoSpegnimento != null) audioSource.PlayOneShot(suonoSpegnimento);

        

        GameManager.Instance.VinciMinigioco();
    }
}