using UnityEngine;

public class MusicNote : MonoBehaviour
{
    private Vector2 direzioneMovimento;
    private float velocitaNota;
    private float tempoDiVita = 5f;
    private float timer = 0f;

    // Questa funzione viene chiamata dalla Chitarra appena la nota viene generata
    public void InizializzaNota(Vector2 direzione, float velocita)
    {
        direzioneMovimento = direzione.normalized;
        velocitaNota = velocita;
    }

    void Update()
    {
        // Muove la nota in linea retta nella direzione calcolata alla nascita
        transform.Translate(direzioneMovimento * velocitaNota * Time.deltaTime, Space.World);

        // Timer di autodistruzione dopo 5 secondi
        timer += Time.deltaTime;
        if (timer >= tempoDiVita)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled) return;

        // Blocco minigioco universale
        if (GameManager.Instance != null && GameManager.Instance.inMinigioco) return;

        if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PerdiVita();
            }

            PlayerMovement scriptGatto = collision.gameObject.GetComponent<PlayerMovement>();
            if (scriptGatto != null)
            {
                scriptGatto.SubisciKnockback(transform, 3f, 0.5f);
            }

            // Distrugge la nota appena colpisce il gatto, per evitare che rimanga sopra di lui
            Destroy(gameObject);
        }
    }
}