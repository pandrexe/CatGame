using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomTrigger : MonoBehaviour
{
    [Header("Nemici controllati da questa stanza")]
    [Tooltip("Trascina qui gli SCRIPT dei nemici (es. BirdEnemy, RoombaEnemy) che devono muoversi SOLO quando il gatto è in questa stanza.")]
    public MonoBehaviour[] nemiciDellaStanza;

    private void Awake()
    {
        // Ci assicuriamo che il collider della stanza sia impostato su Trigger
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void Start()
    {
        // All'avvio del gioco, spegniamo tutti i nemici in questa stanza
        ToggleNemici(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ToggleNemici(true); // Il gatto entra: i nemici si svegliano!
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ToggleNemici(false); // Il gatto esce: i nemici si ibernano!
        }
    }

    private void ToggleNemici(bool stato)
    {
        foreach (MonoBehaviour nemico in nemiciDellaStanza)
        {
            if (nemico != null)
            {
                nemico.enabled = stato;
            }
        }
    }
}