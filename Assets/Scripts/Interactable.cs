using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("UI Interazione")]
    public GameObject testoInterazioneUI;

    // --- LA MAGIA DELL'ARRAY ---
    [Header("Feedback Visivo")]
    [Tooltip("Inserisci qui la quantità di sprite e trascinali negli slot")]
    public SpriteRenderer[] spritesDaEvidenziare;

    [Tooltip("Il colore che prenderanno quando il gatto è vicino")]
    public Color coloreHighlight = Color.yellow;

    // Usiamo un array anche per i colori, caso mai gli sprite avessero colori base diversi!
    private Color[] coloriOriginali;

    protected bool gattoVicino = false;
    protected bool puoInteragire = true;

    protected virtual void Start()
    {
        if (testoInterazioneUI != null) testoInterazioneUI.SetActive(false);

        // Salviamo i colori originali di TUTTI gli sprite nell'array all'avvio
        if (spritesDaEvidenziare != null && spritesDaEvidenziare.Length > 0)
        {
            coloriOriginali = new Color[spritesDaEvidenziare.Length];
            for (int i = 0; i < spritesDaEvidenziare.Length; i++)
            {
                if (spritesDaEvidenziare[i] != null)
                {
                    coloriOriginali[i] = spritesDaEvidenziare[i].color;
                }
            }
        }
    }

    protected virtual void Update()
    {
        if (gattoVicino && puoInteragire && Input.GetKeyDown(KeyCode.E))
        {
            if (testoInterazioneUI != null) testoInterazioneUI.SetActive(false);

            // Appena interagisci, spegne le luci
            SpegniHighlight();

            EseguiInterazione();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && puoInteragire)
        {
            gattoVicino = true;
            if (testoInterazioneUI != null) testoInterazioneUI.SetActive(true);

            // ACCENDIAMO IL GIALLO SU TUTTI!
            if (spritesDaEvidenziare != null)
            {
                foreach (SpriteRenderer sprite in spritesDaEvidenziare)
                {
                    if (sprite != null) sprite.color = coloreHighlight;
                }
            }
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gattoVicino = false;
            if (testoInterazioneUI != null) testoInterazioneUI.SetActive(false);

            // SPEGNIAMO IL GIALLO SU TUTTI!
            SpegniHighlight();
        }
    }

    private void SpegniHighlight()
    {
        // Ripristina il colore originale di ogni singolo sprite
        if (spritesDaEvidenziare != null && coloriOriginali != null)
        {
            for (int i = 0; i < spritesDaEvidenziare.Length; i++)
            {
                if (spritesDaEvidenziare[i] != null)
                {
                    spritesDaEvidenziare[i].color = coloriOriginali[i];
                }
            }
        }
    }

    // Il metodo che i figli dovranno personalizzare
    protected abstract void EseguiInterazione();
}