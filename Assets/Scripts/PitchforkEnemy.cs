using UnityEngine;
using System.Collections;

public class PitchforkEnemy : MonoBehaviour
{
    public float followSpeed = 5f;
    public float diveSpeed = 18f;
    public float returnSpeed = 3f;
    public float shakeAmount = 0.15f;
    public float activationRangeX = 8f;
    public float yAttacco = -4f;
    public float tempoDiAttesa = 1.0f;
    public float distanzaTeletrasporto = 3f;
    public float durataStordimento = 0.5f;

    [Header("Audio")]
    public AudioSource audioSourceMovimento;
    public AudioSource audioSourceAttacco;

    private Transform gatto;
    private float startY;
    private bool staAttaccando = false;
    private bool staTornando = false;
    private bool staTremando = false;

    private bool eMortoDefinitivamente = false;

    void Start()
    {
        startY = transform.position.y;
        if (GameManager.Instance != null && GameManager.Instance.gatto != null)
        {
            gatto = GameManager.Instance.gatto.transform;
        }
    }

    void OnEnable()
    {
        if (eMortoDefinitivamente)
        {
            this.enabled = false;
        }
    }

    void OnDisable()
    {
        if (audioSourceMovimento != null) audioSourceMovimento.Stop();
        if (audioSourceAttacco != null) audioSourceAttacco.Stop();
    }

    void Update()
    {
        if (gatto == null || Time.timeScale == 0)
            return;

        // --- LA MAGIA: PAUSA DURANTE I MINIGIOCHI ---
        if (GameManager.Instance != null && GameManager.Instance.inMinigioco)
        {
            // Usciamo semplicemente dall'Update!
            // NON spegniamo l'audio e NON stoppiamo le Coroutine. 
            // La forca si frizza, l'audio stridente continua.
            return;
        }

        float distanzaX = Mathf.Abs(transform.position.x - gatto.position.x);

        bool staInseguendo = !staAttaccando && !staTornando && !staTremando && distanzaX < activationRangeX;

        if (staInseguendo)
        {
            Vector3 targetPos = new Vector3(gatto.position.x, startY, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, followSpeed * Time.deltaTime);

            if (audioSourceMovimento != null && !audioSourceMovimento.isPlaying)
            {
                audioSourceMovimento.Play();
            }

            if (distanzaX < 0.2f)
            {
                StartCoroutine(SequenzaAttacco());
            }
        }
        else
        {
            if (audioSourceMovimento != null && audioSourceMovimento.isPlaying)
            {
                audioSourceMovimento.Stop();
            }
        }
    }

    IEnumerator SequenzaAttacco()
    {
        staTremando = true;
        Vector3 posOriginale = transform.position;
        float timer = 0f;

        // FASE 1: TREMOLIO
        while (timer < tempoDiAttesa)
        {
            // Se entri in un minigioco mentre trema, frizza il timer e il tremolio!
            if (GameManager.Instance != null && GameManager.Instance.inMinigioco) { yield return null; continue; }

            transform.position = posOriginale + (Vector3)Random.insideUnitCircle * shakeAmount;
            timer += Time.deltaTime;
            yield return null;
        }

        staTremando = false;
        staAttaccando = true;
        transform.position = posOriginale;

        if (audioSourceAttacco != null)
        {
            audioSourceAttacco.Play();
        }

        // FASE 2: ATTACCO IN PICCHIATA
        while (transform.position.y > yAttacco)
        {
            // Se entri in un minigioco mentre cade, resta a mezz'aria!
            if (GameManager.Instance != null && GameManager.Instance.inMinigioco) { yield return null; continue; }

            transform.position += Vector3.down * diveSpeed * Time.deltaTime;
            yield return null;
        }

        staAttaccando = false;

        // FASE 3: PAUSA A TERRA
        float waitTimer = 0f;
        while (waitTimer < 0.5f)
        {
            // Se entri in un minigioco mentre è conficcata a terra, ferma il tempo di attesa
            if (GameManager.Instance != null && GameManager.Instance.inMinigioco) { yield return null; continue; }

            waitTimer += Time.deltaTime;
            yield return null;
        }

        staTornando = true;

        // FASE 4: RITORNO AL SOFFITTO
        while (transform.position.y < startY)
        {
            // Se entri in un minigioco mentre risale, fermati lì!
            if (GameManager.Instance != null && GameManager.Instance.inMinigioco) { yield return null; continue; }

            transform.position += Vector3.up * returnSpeed * Time.deltaTime;
            yield return null;
        }

        staTornando = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance != null && GameManager.Instance.inMinigioco)
        {
            return;
        }

        if (collision.CompareTag("Player") && staAttaccando)
        {
            GameManager.Instance.PerdiVita();

            PlayerMovement scriptGatto = collision.gameObject.GetComponent<PlayerMovement>();
            if (scriptGatto != null)
            {
                scriptGatto.SubisciKnockback(transform, distanzaTeletrasporto, durataStordimento);
            }
        }
    }

    public void SpegnimentoDefinitivo()
    {
        eMortoDefinitivamente = true;
        this.enabled = false;

        if (audioSourceMovimento != null) audioSourceMovimento.Stop();
        if (audioSourceAttacco != null) audioSourceAttacco.Stop();
    }
}