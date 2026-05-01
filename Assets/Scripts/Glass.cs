using UnityEngine;

public class Bicchiere : MonoBehaviour
{
    // Questa è la funzione che chiamerai dall'UnityEvent di InteractableTask!
    public void Frantumati()
    {
        // Il bicchiere sparisce semplicemente dalla scena principale
        Destroy(gameObject);
    }
}