using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CarryableObject : Interactable
{
    [Header("Impostazioni Trasporto")]
    [Tooltip("Trascina qui l'oggetto PuntoBocca che sta dentro al Gatto")]
    public Transform puntoBoccaGatto;

    private bool inBocca = false;
    private Rigidbody2D rb;
    
    // Creiamo un array per salvare tutti i collider del cuscino
    private Collider2D[] tuttiICollider; 

    protected override void Start()
    {
        base.Start(); 
        rb = GetComponent<Rigidbody2D>();
        
        // Salva sia il collider fisico che quello Trigger in un colpo solo
        tuttiICollider = GetComponents<Collider2D>(); 
    }

    protected override void EseguiInterazione()
    {
        if (!inBocca)
        {
            Raccogli();
        }
        else
        {
            Rilascia();
            if (testoInterazioneUI != null) testoInterazioneUI.SetActive(true);
        }
    }

    private void Raccogli()
    {
        inBocca = true;

        if (rb != null) 
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }

        // SPEGNIAMO IL COLLIDER SOLIDO! (Lasciando acceso il Trigger)
        foreach (Collider2D col in tuttiICollider)
        {
            if (!col.isTrigger) 
            {
                col.enabled = false;
            }
        }

        transform.SetParent(puntoBoccaGatto);
        transform.localPosition = Vector3.zero; 
        transform.localRotation = Quaternion.identity; 
    }

    private void Rilascia()
    {
        inBocca = false;
        transform.SetParent(null);

        if (rb != null) 
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        // RIACCENDIAMO IL COLLIDER SOLIDO per farlo sbattere a terra!
        foreach (Collider2D col in tuttiICollider)
        {
            if (!col.isTrigger) 
            {
                col.enabled = true;
            }
        }
     
    }
}