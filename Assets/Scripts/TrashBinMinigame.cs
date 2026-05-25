using UnityEngine;
using System.Collections;

public class TrashBinTask : MonoBehaviour
{
    [Header("Riferimenti")]
    public SpriteRenderer spriteSacco;
    public Collider2D colliderSacco;
    public Transform puntoUscita; 
    public InteractableTask taskManager; // Obbligatorio!

    [Header("Parametri")]
    public float velocitaTrascinamento = 1.0f;

    private bool taskFinito = false;
    private bool staTrascinando = false;
    private Vector3 ultimaPosizioneMouse;
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
        if (taskFinito) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        if (Input.GetMouseButtonDown(0) && !staTrascinando)
        {
            if (colliderSacco != null && colliderSacco.OverlapPoint(mousePos2D))
            {
                staTrascinando = true;
                ultimaPosizioneMouse = mousePos;
            }
        }

        if (Input.GetMouseButtonUp(0) && staTrascinando)
        {
            staTrascinando = false;
        }

        if (staTrascinando && Input.GetMouseButton(0))
        {
            float deltaY = mousePos.y - ultimaPosizioneMouse.y;
            Vector3 nuovaPosizione = spriteSacco.transform.position + new Vector3(0, deltaY * velocitaTrascinamento, 0);

            if (nuovaPosizione.y < posInizialeSacco.y)
            {
                nuovaPosizione.y = posInizialeSacco.y;
            }

            spriteSacco.transform.position = nuovaPosizione;
            ultimaPosizioneMouse = mousePos;

            float baseSacco = spriteSacco.bounds.min.y;
            if (baseSacco >= puntoUscita.position.y)
            {
                FineTask();
            }
        }
        else if (!staTrascinando && spriteSacco.transform.position.y > posInizialeSacco.y)
        {
            spriteSacco.transform.position = Vector3.Lerp(spriteSacco.transform.position, posInizialeSacco, Time.deltaTime * 5f);
        }
    }

    void FineTask()
    {
        taskFinito = true;
        staTrascinando = false;

        if (colliderSacco != null) colliderSacco.enabled = false;

        StartCoroutine(SequenzaVittoria());
    }

    private IEnumerator SequenzaVittoria()
    {
        Rigidbody2D rb = spriteSacco.gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2f;
        rb.AddForce(new Vector2(2f, 5f), ForceMode2D.Impulse);

        yield return new WaitForSeconds(1.5f);

        if (taskManager != null) taskManager.CompletaTask();
        GameManager.Instance.VinciMinigioco();
    }
}