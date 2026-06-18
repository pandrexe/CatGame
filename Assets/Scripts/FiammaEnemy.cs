using UnityEngine;

public class FlameEnemy : MonoBehaviour
{
    [Header("Impostazioni Danno")]
    [Tooltip("Quanto viene sbalzato lontano il gatto quando tocca il fuoco?")]
    public float forzaKnockback = 3f;
    [Tooltip("Per quanti secondi il gatto non può muoversi dopo essersi bruciato?")]
    public float durataStordimento = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Se lo script è spento, non fare nulla
        if (!enabled) return;

        // --- IL BLOCCO MINIGIOCO ---
        // Se sei in un minigioco, la fiamma è innocua!
        if (GameManager.Instance != null && GameManager.Instance.inMinigioco)
        {
            return;
        }

        // Se a toccare il fuoco è il gatto...
        if (collision.CompareTag("Player"))
        {
            // 1. Perde una vita
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PerdiVita();
            }

            // 2. Viene sbalzato via
            PlayerMovement scriptGatto = collision.gameObject.GetComponent<PlayerMovement>();
            if (scriptGatto != null)
            {
                scriptGatto.SubisciKnockback(transform, forzaKnockback, durataStordimento);
            }
        }
    }
}