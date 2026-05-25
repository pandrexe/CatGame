using UnityEngine;

public class TastoLavatriceTask : MonoBehaviour
{
    [Header("Riferimenti")]
    public Collider2D colliderTasto;
    public InteractableTask taskLavatrice; 

    private bool taskFinito = false;

    void Update()
    {
        // Tolto tutto il papiro di controlli! Se siamo qui, il task è attivo.
        if (taskFinito) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (MinigameCursor.Instance != null && MinigameCursor.Instance.contenitoreSingola != null)
            {
                Vector2 posZampa = MinigameCursor.Instance.contenitoreSingola.transform.position;

                if (colliderTasto != null && colliderTasto.OverlapPoint(posZampa))
                {
                    PremiTasto();
                }
            }
        }
    }

    private void PremiTasto()
    {
        taskFinito = true;

        if (taskLavatrice != null)
        {
            taskLavatrice.CompletaTask(); // Questo spegnerà automaticamente questo script!
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.VinciMinigioco();
        }
    }
}