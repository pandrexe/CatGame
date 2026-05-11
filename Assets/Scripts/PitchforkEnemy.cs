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

    private Transform gatto;
    private float startY;
    private bool staAttaccando = false;
    private bool staTornando = false;
    private bool staTremando = false;

    void Start()
    {
        startY = transform.position.y;
        if (GameManager.Instance != null && GameManager.Instance.gatto != null)
        {
            gatto = GameManager.Instance.gatto.transform;
        }
    }

    void Update()
    {
        // Ora controlliamo solo se il gatto esiste e se non siamo in pausa menu
        // Rimosso il controllo inMinigioco per permettere l'attacco continuo
        if (gatto == null || Time.timeScale == 0)
            return;

        float distanzaX = Mathf.Abs(transform.position.x - gatto.position.x);

        if (!staAttaccando && !staTornando && !staTremando && distanzaX < activationRangeX)
        {
            Vector3 targetPos = new Vector3(gatto.position.x, startY, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, followSpeed * Time.deltaTime);

            if (distanzaX < 0.2f)
            {
                StartCoroutine(SequenzaAttacco());
            }
        }
    }

    IEnumerator SequenzaAttacco()
    {
        staTremando = true;
        Vector3 posOriginale = transform.position;
        float timer = 0f;

        while (timer < tempoDiAttesa)
        {
            transform.position = posOriginale + (Vector3)Random.insideUnitCircle * shakeAmount;
            timer += Time.deltaTime;
            yield return null;
        }

        staTremando = false;
        staAttaccando = true;
        transform.position = posOriginale; 

        while (transform.position.y > yAttacco)
        {
            transform.position += Vector3.down * diveSpeed * Time.deltaTime;
            yield return null;
        }

        staAttaccando = false;
        yield return new WaitForSeconds(0.5f);
        staTornando = true;
        
        while (transform.position.y < startY)
        {
            transform.position += Vector3.up * returnSpeed * Time.deltaTime;
            yield return null;
        }

        staTornando = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Il danno viene applicato sempre, indipendentemente dallo stato del minigioco
        if (collision.CompareTag("Player") && staAttaccando)
        {
            GameManager.Instance.PerdiVita();

            float direzioneX = (collision.transform.position.x > transform.position.x) ? 1f : -1f;

            PlayerMovement scriptGatto = collision.gameObject.GetComponent<PlayerMovement>();
            if (scriptGatto != null) 
            {
                scriptGatto.ApplicaStordimento(durataStordimento);
            }

            Vector3 nuovaPosizione = collision.transform.position;
            nuovaPosizione.x += (distanzaTeletrasporto * direzioneX);
            nuovaPosizione.y += 0.5f; 
            collision.transform.position = nuovaPosizione;
        }
    }
}