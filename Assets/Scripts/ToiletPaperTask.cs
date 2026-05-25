using UnityEngine;
using System.Collections;

public class ToiletPaperTask : MonoBehaviour
{
    [Header("Riferimenti")]
    public SpriteRenderer spriteCartaGigante;
    public Collider2D colliderCarta;
    public Transform puntoDistacco;
    public InteractableTask taskManager; // Obbligatorio!

    [Header("Parametri")]
    public float velocitaTrascinamento = 1.0f;

    private bool taskFinito = false;
    private Vector3 ultimaPosizioneMouse;
    private bool staTrascinando = false;

    private int prossimoTastoAtteso = 0;
    private int tastoInUso = -1;
    private bool zampeInizializzate = false;

    void Update()
    {
        if (taskFinito) return;

        if (!zampeInizializzate)
        {
            ImpostaVisibilitaZampe(true, false); 
            prossimoTastoAtteso = 0;
            zampeInizializzate = true;
        }

        if (!staTrascinando)
        {
            if (Input.GetMouseButtonDown(0) && prossimoTastoAtteso == 0)
            {
                if (MinigameCursor.Instance != null && MinigameCursor.Instance.zampaSx != null)
                {
                    Vector2 posZampaSx = MinigameCursor.Instance.zampaSx.position;
                    if (colliderCarta != null && colliderCarta.OverlapPoint(posZampaSx))
                    {
                        staTrascinando = true;
                        tastoInUso = 0;
                        prossimoTastoAtteso = 1;
                        ultimaPosizioneMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1) && prossimoTastoAtteso == 1)
            {
                if (MinigameCursor.Instance != null && MinigameCursor.Instance.zampaDx != null)
                {
                    Vector2 posZampaDx = MinigameCursor.Instance.zampaDx.position;
                    if (colliderCarta != null && colliderCarta.OverlapPoint(posZampaDx))
                    {
                        staTrascinando = true;
                        tastoInUso = 1;
                        prossimoTastoAtteso = 0;
                        ultimaPosizioneMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    }
                }
            }
        }

        if (tastoInUso != -1 && Input.GetMouseButtonUp(tastoInUso))
        {
            if (tastoInUso == 0) ImpostaVisibilitaZampe(false, true);
            else if (tastoInUso == 1) ImpostaVisibilitaZampe(true, false);

            staTrascinando = false;
            tastoInUso = -1;
        }

        if (staTrascinando && tastoInUso != -1 && Input.GetMouseButton(tastoInUso))
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

    private void ImpostaVisibilitaZampe(bool visibileSx, bool visibileDx)
    {
        if (MinigameCursor.Instance == null) return;
        if (MinigameCursor.Instance.zampaSx != null) MinigameCursor.Instance.zampaSx.GetComponent<SpriteRenderer>().enabled = visibileSx;
        if (MinigameCursor.Instance.zampaDx != null) MinigameCursor.Instance.zampaDx.GetComponent<SpriteRenderer>().enabled = visibileDx;
    }

    void FineTask()
    {
        taskFinito = true;
        staTrascinando = false;

        if (tastoInUso == 0) ImpostaVisibilitaZampe(true, false);
        else if (tastoInUso == 1) ImpostaVisibilitaZampe(false, true);
        else ImpostaVisibilitaZampe(false, false);

        tastoInUso = -1;
        StartCoroutine(SequenzaCadutaVittoria());
    }

    private IEnumerator SequenzaCadutaVittoria()
    {
        if (colliderCarta != null) colliderCarta.enabled = false;

        Rigidbody2D rbCaduta = spriteCartaGigante.gameObject.AddComponent<Rigidbody2D>();
        rbCaduta.bodyType = RigidbodyType2D.Dynamic;
        rbCaduta.gravityScale = 3f;

        yield return new WaitForSeconds(1f);

        if (taskManager != null) taskManager.CompletaTask();
        GameManager.Instance.VinciMinigioco();
    }
}