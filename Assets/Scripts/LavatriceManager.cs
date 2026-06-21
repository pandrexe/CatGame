using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LavatriceTask : MonoBehaviour
{
    [Header("Riferimenti Lavatrice")]
    public SpawnerBolle spawnerBolle;
    public Sprite spriteLavatriceSpenta;
    private SpriteRenderer spriteRenderer;

    [Header("Audio")]
    public AudioClip suonoLavatriceAccesa;
    public AudioClip suonoSpegnimento;
    private AudioSource audioSource;

    private bool giaSpenta = false;

    void Awake()
    {
        // MODIFICA: Visto che lo script è sul figlio, cerchiamo il SpriteRenderer nel PADRE!
        spriteRenderer = GetComponentInParent<SpriteRenderer>();

        audioSource = GetComponent<AudioSource>();

        if (suonoLavatriceAccesa != null)
        {
            audioSource.clip = suonoLavatriceAccesa;
            audioSource.loop = true;
        }
    }

    void OnEnable()
    {
        if (giaSpenta) return;

        if (audioSource != null && suonoLavatriceAccesa != null)
        {
            audioSource.Play();
        }

        if (spawnerBolle != null)
        {
            spawnerBolle.enabled = true;
        }
    }

    void OnDisable()
    {
        if (GameManager.Instance != null && GameManager.Instance.inMinigioco)
        {
            return;
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        if (spawnerBolle != null)
        {
            spawnerBolle.enabled = false;
        }
    }

    public void SpegniLavatrice()
    {
        if (giaSpenta) return;

        giaSpenta = true;

        if (spawnerBolle != null)
        {
            spawnerBolle.enabled = false;
        }

        if (spriteRenderer != null && spriteLavatriceSpenta != null)
        {
            spriteRenderer.sprite = spriteLavatriceSpenta;
        }

        if (audioSource != null)
        {
            audioSource.Stop();
            if (suonoSpegnimento != null)
            {
                audioSource.PlayOneShot(suonoSpegnimento);
            }
        }

        Debug.Log("Lavatrice Figlia spenta con successo!");
    }
}