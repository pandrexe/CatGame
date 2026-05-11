using UnityEngine;
using System.Collections;

public class TrashBinTask : MonoBehaviour
{
    [Header("Riferimenti Sandwich")]
    public SpriteRenderer spriteSacco;
    public Collider2D colliderSacco;
    public Transform puntoUscita; // Il punto in alto che segna l'uscita dal bidone

    [Header("Parametri")]
    public float velocitaTrascinamento = 1.0f;

    private bool taskFinito = false;
    private bool staTrascinando = false;
    private Vector3 ultimaPosizioneMouse;

    // Ci salviamo la posizione iniziale così, se lasci la presa, il sacco ricade dentro!
    private Vector3 posInizialeSacco;

    void Start()
    {
        if (spriteSacco != null)
        {
            posInizialeSacco = spriteSacco.transform.position;
        }
    }

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.inMinigioco || taskFinito)
            return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        // 1. INIZIO PRESA (Clicco sul sacco)
        if (Input.GetMouseButtonDown(0) && !staTrascinando)
        {
            if (colliderSacco != null && colliderSacco.OverlapPoint(mousePos2D))
            {
                staTrascinando = true;
                ultimaPosizioneMouse = mousePos;
            }
        }

        // 2. RILASCIO (Mollo il click)
        if (Input.GetMouseButtonUp(0) && staTrascinando)
        {
            staTrascinando = false;
        }

        // 3. TRASCINAMENTO (Tiro su)
        if (staTrascinando && Input.GetMouseButton(0))
        {
            float deltaY = mousePos.y - ultimaPosizioneMouse.y;

            // Muoviamo il sacco seguendo il mouse
            Vector3 nuovaPosizione = spriteSacco.transform.position + new Vector3(0, deltaY * velocitaTrascinamento, 0);

            // Blocchiamo il sacco per non farlo andare PIÙ IN BASSO del fondo del bidone
            if (nuovaPosizione.y < posInizialeSacco.y)
            {
                nuovaPosizione.y = posInizialeSacco.y;
            }

            spriteSacco.transform.position = nuovaPosizione;
            ultimaPosizioneMouse = mousePos;

            // Controllo Vittoria: Il punto più BASSO del sacco ha superato il punto di uscita?
            float baseSacco = spriteSacco.bounds.min.y;
            if (baseSacco >= puntoUscita.position.y)
            {
                FineTask();
            }
        }
        // 4. EFFETTO CADUTA (Se ho mollato il sacco prima di vincere, ricade giù!)
        else if (!staTrascinando && spriteSacco.transform.position.y > posInizialeSacco.y)
        {
            // Lerp riporta il sacco giù fluidamente
            spriteSacco.transform.position = Vector3.Lerp(spriteSacco.transform.position, posInizialeSacco, Time.deltaTime * 5f);
        }
    }

    void FineTask()
    {
        taskFinito = true;
        staTrascinando = false;

        // Spegniamo il collider così non lo clicchiamo più
        if (colliderSacco != null) colliderSacco.enabled = false;

        StartCoroutine(SequenzaVittoria());
    }

    private IEnumerator SequenzaVittoria()
    {
        // Diamogli un piccolo "slancio" finale per farlo schizzare fuori
        Rigidbody2D rb = spriteSacco.gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2f;
        // Spinta verso l'alto e leggermente a destra (come se lo stessimo sfilando)
        rb.AddForce(new Vector2(2f, 5f), ForceMode2D.Impulse);

        // Aspettiamo un po' per goderci l'animazione della busta che vola fuori
        yield return new WaitForSeconds(1.5f);

        GameManager.Instance.VinciMinigioco();
    }
}