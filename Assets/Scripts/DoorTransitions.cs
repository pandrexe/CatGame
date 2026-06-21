using UnityEngine;
using Unity.Cinemachine;

// Guarda qui: eredita da Interactable, non da MonoBehaviour!
public class PortaInterattiva : Interactable
{
    [Header("Impostazioni Stanza Segreta")]
    [SerializeField] BoxCollider2D mapBoundry; // I limiti della nuova stanza
    [SerializeField] Transform puntoDiArrivo;  // Dove appare il gatto

    private CinemachineConfiner2D confiner;
    private GameObject playerRef;

    private AudioSource audioSourcePorta;

    protected override void Start()
    {
        base.Start();
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();

        audioSourcePorta = GetComponent<AudioSource>();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.CompareTag("Player"))
        {
            playerRef = collision.gameObject;
        }
    }

    protected override void EseguiInterazione()
    {
        // --- FACCIAMO PARTIRE IL SUONO DELLA PORTA! ---
        if (audioSourcePorta != null)
        {
            audioSourcePorta.Play();
        }

        // 1. Cambiamo i limiti della telecamera
        if (confiner != null && mapBoundry != null)
        {
            confiner.BoundingShape2D = mapBoundry;
            confiner.InvalidateBoundingShapeCache();
        }

        // 2. Teletrasportiamo il gatto E la telecamera istantaneamente!
        if (playerRef != null && puntoDiArrivo != null)
        {
            // Calcoliamo di quanto si sta spostando il gatto
            Vector3 deltaMovimento = puntoDiArrivo.position - playerRef.transform.position;

            // Spostiamo fisicamente il gatto
            playerRef.transform.position = puntoDiArrivo.position;

            // Diciamo alla telecamera di saltare all'istante senza fare la transizione fluida
            if (GameManager.Instance != null && GameManager.Instance.telecameraGatto != null)
            {
                GameManager.Instance.telecameraGatto.OnTargetObjectWarped(playerRef.transform, deltaMovimento);
            }
        }
    }
}