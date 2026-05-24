using UnityEngine;
using Unity.Cinemachine;

public class RoomsTransition : MonoBehaviour
{
    [Header("Limiti Nuova Stanza")]
    [SerializeField] BoxCollider2D mapBoundry;
    private CinemachineConfiner2D confiner;

    public enum TipoTransizione { Adiacente, Teletrasporto }
    
    [Header("Tipo di Spostamento")]
    [SerializeField] TipoTransizione tipoTransizione;

    [Header("Impostazioni: Adiacente")]
    [SerializeField] Direction direction;
    [SerializeField] float additivePos = 2f;
    public enum Direction { Left, Right }

    [Header("Impostazioni: Teletrasporto")]
    [SerializeField] Transform puntoDiArrivo;

    private void Awake()
    {
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 1. Aggiorna i confini della telecamera
            if (confiner != null && mapBoundry != null)
            {
                confiner.BoundingShape2D = mapBoundry;
                confiner.InvalidateBoundingShapeCache();
            }

            // 2. Sposta il gatto in base alla tua scelta
            if (tipoTransizione == TipoTransizione.Adiacente)
            {
                Vector3 newPosition = collision.transform.position;
                if (direction == Direction.Left) newPosition.x -= additivePos;
                else if (direction == Direction.Right) newPosition.x += additivePos;
                
                collision.transform.position = newPosition;
            }
            else if (tipoTransizione == TipoTransizione.Teletrasporto)
            {
                if (puntoDiArrivo != null)
                {
                    collision.transform.position = puntoDiArrivo.position;
                }
            }
        }
    }
}