using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class RoombaMinigame : MonoBehaviour
{
    [Header("Sprite del Roomba")]
    public Sprite spriteAcceso;
    public Sprite spriteSpento;
    public AudioClip suonoSpegnimento;

    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private bool giaSpento = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = gameObject.AddComponent<AudioSource>();
        spriteRenderer.sprite = spriteAcceso;
    }

    // SOSTITUITO: Da OnMouseDown a OnTriggerEnter2D!
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Il nostro buttafuori
        if (GameManager.Instance != null && !GameManager.Instance.inMinigioco) return;
        
        if (giaSpento) return;

        // Se qualcosa entra nel trigger (e in questa stanza si muove solo la zampa!), spegni.
        giaSpento = true;
        spriteRenderer.sprite = spriteSpento;

        StartCoroutine(ConcludiMinigioco());
    }

    private IEnumerator ConcludiMinigioco()
    {
        yield return new WaitForSeconds(1f);
        if (suonoSpegnimento != null) audioSource.PlayOneShot(suonoSpegnimento);
        GameManager.Instance.VinciMinigioco();
    }
}