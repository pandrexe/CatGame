using UnityEngine;

public class BollaLavatrice : MonoBehaviour
{
    [Header("Impostazioni Movimento")]
    public float velocitaSalita = 2f;
    public float ampiezzaZigZag = 1f;
    public float frequenzaZigZag = 2f;

    [Header("Impostazioni Knockback")]
    public float distanzaSbalzo = 2f;
    public float durataStun = 0.5f;

    private Vector3 posizioneIniziale;
    private float tempoDiVita = 0f;

    // --- RIFERIMENTO AL SUONO DELLA BOLLA ---
    private AudioSource audioSourceBolla;

    void Start()
    {
        posizioneIniziale = transform.position;

        // Peschiamo l'Audio Source attaccato a QUESTA specifica bolla
        audioSourceBolla = GetComponent<AudioSource>();

        // Facciamo partire il suono del "blop" appena spawna!
        if (audioSourceBolla != null)
        {
            audioSourceBolla.Play();
        }

        Destroy(gameObject, 10f);
    }

    void Update()
    {
        tempoDiVita += Time.deltaTime;

        float nuovaX = posizioneIniziale.x + Mathf.Sin(tempoDiVita * frequenzaZigZag) * ampiezzaZigZag;
        float nuovaY = transform.position.y + (velocitaSalita * Time.deltaTime);

        transform.position = new Vector3(nuovaX, nuovaY, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PerdiVita();
            }

            PlayerMovement scriptGatto = collision.GetComponent<PlayerMovement>();

            if (scriptGatto != null)
            {
                scriptGatto.SubisciKnockback(transform, distanzaSbalzo, durataStun);
            }

            Destroy(gameObject);
        }
    }
}