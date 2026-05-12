using UnityEngine;

public class AlarmTask : MonoBehaviour
{
    [Header("Riferimenti")]
    public Collider2D colliderSveglia; // Trascina qui il collider della sveglia grande

    private bool taskFinito = false;

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.inMinigioco || taskFinito)
            return;

        // Se il giocatore clicca il tasto sinistro del mouse
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            // Controlliamo che la freccia (o la zampa) sia FISICAMENTE sopra la sveglia
            if (colliderSveglia != null && colliderSveglia.OverlapPoint(mousePos2D))
            {
                FineTask();
            }
        }
    }

    void FineTask()
    {
        taskFinito = true;
        GameManager.Instance.VinciMinigioco();
    }
}