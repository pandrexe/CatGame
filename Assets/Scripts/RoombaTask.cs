using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class RoombaMinigame : MonoBehaviour
{
    [Header("Sprite del Roomba")]
    public Sprite spriteAcceso;
    public Sprite spriteSpento;
    private AudioSource audioSource;
    public AudioClip suonoSpegnimento;

    private SpriteRenderer spriteRenderer;
    private bool giaSpento = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = gameObject.AddComponent<AudioSource>();
        spriteRenderer.sprite = spriteAcceso;
    }

    void OnMouseDown()
    {
        if (giaSpento)
        giaSpento = true;
        spriteRenderer.sprite = spriteSpento;

        StartCoroutine(ConcludiMinigioco());
    }

    private IEnumerator ConcludiMinigioco()
    {
        yield return new WaitForSeconds(1f);
        audioSource.PlayOneShot(suonoSpegnimento);
        GameManager.Instance.VinciMinigioco();

    }
}