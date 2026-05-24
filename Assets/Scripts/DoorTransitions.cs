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

    protected override void Start()
    {
        // Importante: chiama lo Start del padre (Interactable) per nascondere la UI
        base.Start(); 
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // Chiama la logica del padre per accendere la scritta "Premi E"
        base.OnTriggerEnter2D(collision); 

        if (collision.CompareTag("Player"))
        {
            playerRef = collision.gameObject;
        }
    }

    // Ecco la magia: qui mettiamo solo cosa succede DOPO aver premuto E
    protected override void EseguiInterazione()
    {
        // 1. Cambiamo i limiti della telecamera
        if (confiner != null && mapBoundry != null)
        {
            confiner.BoundingShape2D = mapBoundry;
            confiner.InvalidateBoundingShapeCache();
        }

        // 2. Teletrasportiamo il gatto!
        if (playerRef != null && puntoDiArrivo != null)
        {
            playerRef.transform.position = puntoDiArrivo.position;
        }
    }
}