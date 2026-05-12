using UnityEngine;
using System.Collections;

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

    // Variabili per l'alternanza
    private int prossimoTastoAtteso = 0;
    private int tastoInUso = -1;

    private bool zampeInizializzate = false;

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.inMinigioco || taskFinito)
        {
            zampeInizializzate = false;
            return;
        }

        // SETUP INIZIALE
        if (!zampeInizializzate)
        {
            ImpostaVisibilitaZampe(true, false); // Sx accesa, Dx spenta
            prossimoTastoAtteso = 0;
            zampeInizializzate = true;
        }

        // 1. INIZIO CLICK
        if (!staTrascinando)
        {
            // Proviamo col Tasto SINISTRO (0)
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

                        // NOTA: Qui NON cambiamo più la zampa. La teniamo visibile mentre trascini!
                    }
                }
            }
            // Proviamo col Tasto DESTRO (1)
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

                        // NOTA: Anche qui, non cambiamo ancora la zampa.
                    }
                }
            }
        }

        // 2. RILASCIO (E CAMBIO ZAMPA GRAFICO!)
        if (tastoInUso != -1 && Input.GetMouseButtonUp(tastoInUso))
        {
            // Appena mollo la presa, guardo con che tasto stavo trascinando e faccio lo "switch" visivo
            if (tastoInUso == 0)
            {
                // Ho appena mollato la zampa Sinistra -> Mostro la Destra per il prossimo colpo
                ImpostaVisibilitaZampe(false, true);
            }
            else if (tastoInUso == 1)
            {
                // Ho appena mollato la zampa Destra -> Mostro la Sinistra
                ImpostaVisibilitaZampe(true, false);
            }

            staTrascinando = false;
            tastoInUso = -1;
        }

        // 3. TRASCINAMENTO
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

        if (MinigameCursor.Instance.zampaSx != null)
        {
            SpriteRenderer srSx = MinigameCursor.Instance.zampaSx.GetComponent<SpriteRenderer>();
            if (srSx != null) srSx.enabled = visibileSx;
        }

        if (MinigameCursor.Instance.zampaDx != null)
        {
            SpriteRenderer srDx = MinigameCursor.Instance.zampaDx.GetComponent<SpriteRenderer>();
            if (srDx != null) srDx.enabled = visibileDx;
        }
    }

    void FineTask()
    {
        taskFinito = true;
        staTrascinando = false;

        // Controlliamo con quale zampa abbiamo dato il colpo di grazia
        if (tastoInUso == 0)
        {
            // Stavamo usando la Sinistra -> Lasciamo accesa la Sinistra
            ImpostaVisibilitaZampe(true, false);
        }
        else if (tastoInUso == 1)
        {
            // Stavamo usando la Destra -> Lasciamo accesa la Destra
            ImpostaVisibilitaZampe(false, true);
        }
        else
        {
            // Se per qualche motivo strano non c'era nessun tasto premuto, le spegniamo entrambe per sicurezza
            ImpostaVisibilitaZampe(false, false);
        }

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

        GameManager.Instance.VinciMinigioco();
    }
}