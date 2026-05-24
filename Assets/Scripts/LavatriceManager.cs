using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LavatriceTask : MonoBehaviour // Cambiato: ora è MonoBehaviour normale!
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

    void Start() // Diventa un normale Start
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (suonoLavatriceAccesa != null)
        {
            audioSource.clip = suonoLavatriceAccesa;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    // Questa funzione verrà chiamata DIRETTAMENTE dalle "Azioni Alla Vittoria" nell'Inspector!
    public void SpegniLavatrice()
    {
        if (giaSpenta) return;

        giaSpenta = true;

        // 1. BLOCCA LE BOLLE
        if (spawnerBolle != null)
        {
            spawnerBolle.enabled = false;
        }

        // 2. CAMBIA LA GRAFICA
        if (spriteRenderer != null && spriteLavatriceSpenta != null)
        {
            spriteRenderer.sprite = spriteLavatriceSpenta;
        }

        // 3. GESTISCI L'AUDIO
        if (audioSource != null)
        {
            audioSource.Stop();
            if (suonoSpegnimento != null)
            {
                audioSource.PlayOneShot(suonoSpegnimento);
            }
        }

        Debug.Log("Lavatrice Spenta con successo tramite UnityEvent!");
    }
}