using UnityEngine;
using System.Collections; // FONDAMENTALE: Ci serve per usare le Coroutine (IEnumerator)!

public class ToiletPaperTask : MonoBehaviour
{
    [Header("Riferimenti")]
    public SpriteRenderer spriteCartaGigante;
    public Collider2D colliderCarta;
    public Transform puntoDistacco;

    [Header("Parametri")]
    public float velocitaTrascinamento = 1.0f;

    private bool taskFinito = false;
    private Vector3 ultimaPosizioneMouse;
    private bool staTrascinando = false;

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.inMinigioco || taskFinito)
            return;

        // 1. INIZIO CLICK
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            if (colliderCarta != null && colliderCarta.OverlapPoint(mousePos2D))
            {
                staTrascinando = true;
                ultimaPosizioneMouse = mousePos;
            }
        }

        // 2. RILASCIO
        if (Input.GetMouseButtonUp(0))
        {
            staTrascinando = false;
        }

        // 3. TRASCINAMENTO
        if (Input.GetMouseButton(0) && staTrascinando)
        {
            Vector3 posizioneCorrenteMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float deltaY = ultimaPosizioneMouse.y - posizioneCorrenteMouse.y;

            if (deltaY > 0)
            {
                spriteCartaGigante.transform.position += new Vector3(0, -deltaY * velocitaTrascinamento, 0);

                float bordoSuperioreCarta = spriteCartaGigante.bounds.max.y;

                if (bordoSuperioreCarta <= puntoDistacco.position.y)
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
        staTrascinando = false;

        // Invece di vincere subito, avviamo la sequenza finale (Caduta + Pausa)
        StartCoroutine(SequenzaCadutaVittoria());
    }

    private IEnumerator SequenzaCadutaVittoria()
    {
        // 1. Spegniamo il collider della carta così non possiamo più cliccarla per sbaglio
        if (colliderCarta != null) colliderCarta.enabled = false;

        // 2. Aggiungiamo un Rigidbody2D al volo per farla cadere realisticamente!
        Rigidbody2D rbCaduta = spriteCartaGigante.gameObject.AddComponent<Rigidbody2D>();
        rbCaduta.bodyType = RigidbodyType2D.Dynamic;
        rbCaduta.gravityScale = 3f; // Mettiamo 3 così cade velocemente senza sembrare una piuma

        // 3. Aspettiamo 1 secondo pieno
        yield return new WaitForSeconds(1f);

        // 4. Ora che la carta è caduta e abbiamo visto il rotolo vuoto, chiudiamo il minigioco!
        GameManager.Instance.VinciMinigioco();
    }
}