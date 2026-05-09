using UnityEngine;

public class ToiletPaperTask : MonoBehaviour
{
    [Header("Riferimenti")]
    public Transform spriteCartaGigante; // Il foglio lunghissimo

    [Header("Parametri Scorrimento")]
    public float yPuntoFinale = -10f;    // Altezza Y a cui il foglio e considerato finito
    public float velocitaTrascinamento = 1.0f;

    private bool taskFinito = false;
    private Vector3 ultimaPosizioneMouse;

    void Update()
    {
        // Usa il tuo GameManager per verificare lo stato
        if (GameManager.Instance == null || !GameManager.Instance.inMinigioco || taskFinito) 
            return;

        // Inizio click (Zampa acchiappa il foglio)
        if (Input.GetMouseButtonDown(0))
        {
            ultimaPosizioneMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        // Trascinamento attivo (Tiri giu il foglio)
        if (Input.GetMouseButton(0))
        {
            Vector3 posizioneCorrenteMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // Calcola quanto si e mosso il mouse (differenza Y)
            float deltaY = ultimaPosizioneMouse.y - posizioneCorrenteMouse.y;

            // Se trasciniamo verso il basso (deltaY positivo)
            if (deltaY > 0) 
            {
                // Sposta il foglio gigante verso il basso
                spriteCartaGigante.position += new Vector3(0, -deltaY * velocitaTrascinamento, 0);
                
                // Se la carta ha superato il punto limite, hai vinto
                if (spriteCartaGigante.position.y <= yPuntoFinale)
                {
                    FineTask();
                }
            }
            
            ultimaPosizioneMouse = posizioneCorrenteMouse;
        }
    }

    void FineTask()
    {
        taskFinito = true;
        Debug.Log("Task Carta Igienica completato con successo!");

        // Chiama il metodo del TUO GameManager per uscire dal POV
        GameManager.Instance.VinciMinigioco();
    }
}