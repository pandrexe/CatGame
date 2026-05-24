using UnityEngine;

public class BollaLavatrice : MonoBehaviour
{
    [Header("Impostazioni Movimento")]
    public float velocitaSalita = 2f;
    public float ampiezzaZigZag = 1f; // Quanto va a destra e sinistra
    public float frequenzaZigZag = 2f; // Quanto velocemente fa lo zig-zag

    [Header("Impostazioni Knockback")]
    public float distanzaSbalzo = 2f;
    public float durataStun = 0.5f;

    private Vector3 posizioneIniziale;
    private float tempoDiVita = 0f;

    void Start()
    {
        // Ci salviamo da dove è partita per calcolare lo zig-zag correttamente
        posizioneIniziale = transform.position;

        // TRUCCO PRO: Distruggiamo la bolla in automatico dopo 10 secondi.
        // Così se vola fuori dallo schermo non rimane all'infinito a consumare RAM!
        Destroy(gameObject, 10f);
    }

    void Update()
    {
        tempoDiVita += Time.deltaTime;

        // Calcoliamo lo zig-zag usando la funzione matematica del Seno (Mathf.Sin)
        float nuovaX = posizioneIniziale.x + Mathf.Sin(tempoDiVita * frequenzaZigZag) * ampiezzaZigZag;
        float nuovaY = transform.position.y + (velocitaSalita * Time.deltaTime);

        transform.position = new Vector3(nuovaX, nuovaY, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Se tocca il gatto
        if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PerdiVita();
            }

            // --- NOVITÀ: IL CALCIO DELLA BOLLA ---
            // Andiamo a prendere lo script del movimento dal gatto che abbiamo appena colpito
            PlayerMovement scriptGatto = collision.GetComponent<PlayerMovement>();

            if (scriptGatto != null)
            {
                // Lanciamo il knockback passandogli:
                // transform -> la posizione di questa bolla (per capire se lanciare il gatto a dx o sx)
                // distanzaSbalzo -> di quanto si sposta
                // durataStun -> per quanto tempo rimane frizzato
                scriptGatto.SubisciKnockback(transform, distanzaSbalzo, durataStun);
            }
            // -------------------------------------

            // Distruggiamo la bolla appena colpisce il gatto
            Destroy(gameObject);
        }
    }
}