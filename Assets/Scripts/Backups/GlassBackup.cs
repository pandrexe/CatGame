/*
using UnityEngine;

public class Bicchiere : MonoBehaviour
{
    [Header("Controllo Cuscino (Raycast)")]
    public float lunghezzaRaycast = 5f;
    public bool cuscinoPiazzatoSotto = false;

    void Update()
    {
        // Spara un raggio che attraversa tutto verso il basso
        RaycastHit2D[] oggettiColpiti = Physics2D.RaycastAll(transform.position, Vector2.down, lunghezzaRaycast);
        bool trovato = false;

        foreach (RaycastHit2D hit in oggettiColpiti)
        {
            if (hit.collider != null && hit.collider.CompareTag("Cuscino"))
            {
                trovato = true;
                break; // Trovato!
            }
        }

        cuscinoPiazzatoSotto = trovato;
    }

    // Questa è la linea rossa che vedrai nella scena di Unity anche a gioco fermo!
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.down * lunghezzaRaycast);
    }

    // Il tuo vecchio metodo richiamato in caso di fallimento
    public void Frantumati()
    {
        Destroy(gameObject);
    }
}
*/