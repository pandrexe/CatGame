using UnityEngine;

public class TastoLavatriceTask : MonoBehaviour
{
    [Header("Riferimenti")]
    public Collider2D colliderTasto;
    public InteractableTask taskLavatrice; // Cambiato: Punta all'InteractableTask del figlio!

    private bool taskFinito = false;

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.inMinigioco || taskFinito)
            return;

        // Click del mouse
        if (Input.GetMouseButtonDown(0))
        {
            // ORA CONTROLLIAMO LA ZAMPA SINGOLA, NON LA SX!
            if (MinigameCursor.Instance != null && MinigameCursor.Instance.contenitoreSingola != null)
            {
                // Prendiamo la posizione della Zampa Singola
                Vector2 posZampa = MinigameCursor.Instance.contenitoreSingola.transform.position;

                if (colliderTasto != null && colliderTasto.OverlapPoint(posZampa))
                {
                    PremiTasto();
                }
                else
                {
                    Debug.Log("Cliccato, ma fuori dal tasto!");
                }
            }
        }
    }

    private void PremiTasto()
    {
        taskFinito = true;

        // 1. Diciamo al task di completarsi (Questo attiverà AUTOMATICAMENTE il UnityEvent!)
        if (taskLavatrice != null)
        {
            taskLavatrice.CompletaTask();
        }

        // 2. Diciamo al GameManager di chiudere il minigioco e rimettere la telecamera sul gatto
        if (GameManager.Instance != null)
        {
            GameManager.Instance.VinciMinigioco();
        }
    }
}