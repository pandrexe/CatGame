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
    private int prossimoTastoAtteso = -1;
    private int tastoInUso = -1;

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.inMinigioco || taskFinito)
            return;

        // 1. INIZIO CLICK (Ora con precisione millimetrica sulle zampe!)
        if (!staTrascinando)
        {
            // Proviamo col Tasto SINISTRO (0) - ZAMPA SX
            if (Input.GetMouseButtonDown(0) && (prossimoTastoAtteso == -1 || prossimoTastoAtteso == 0))
            {
                // Controlliamo se esiste il cursore globale e la sua zampa sinistra
                if (MinigameCursor.Instance != null && MinigameCursor.Instance.zampaSx != null)
                {
                    // Prendiamo la posizione ESATTA della zampa sinistra visibile!
                    Vector2 posZampaSx = MinigameCursor.Instance.zampaSx.position;

                    // Controlliamo se LA ZAMPA (non il mouse) sta toccando la carta
                    if (colliderCarta != null && colliderCarta.OverlapPoint(posZampaSx))
                    {
                        staTrascinando = true;
                        tastoInUso = 0;
                        prossimoTastoAtteso = 1; // Prossima: Destra!
                        ultimaPosizioneMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    }
                }
            }
            // Proviamo col Tasto DESTRO (1) - ZAMPA DX
            else if (Input.GetMouseButtonDown(1) && (prossimoTastoAtteso == -1 || prossimoTastoAtteso == 1))
            {
                if (MinigameCursor.Instance != null && MinigameCursor.Instance.zampaDx != null)
                {
                    // Prendiamo la posizione ESATTA della zampa destra visibile!
                    Vector2 posZampaDx = MinigameCursor.Instance.zampaDx.position;

                    if (colliderCarta != null && colliderCarta.OverlapPoint(posZampaDx))
                    {
                        staTrascinando = true;
                        tastoInUso = 1;
                        prossimoTastoAtteso = 0; // Prossima: Sinistra!
                        ultimaPosizioneMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    }
                }
            }
        }

        // 2. RILASCIO
        if (tastoInUso != -1 && Input.GetMouseButtonUp(tastoInUso))
        {
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

    void FineTask()
    {
        taskFinito = true;
        staTrascinando = false;
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