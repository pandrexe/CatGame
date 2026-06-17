using UnityEngine;
using UnityEngine.Events;

public class BedInteraction : Interactable
{
    [Header("Eventi Fine Gioco")]
    [Tooltip("Cosa succede se il gatto dorme e TUTTI i task sono finiti?")]
    public UnityEvent azioniVittoria;

    [Tooltip("Cosa succede se il gatto dorme ma MANCANO dei task?")]
    public UnityEvent azioniSconfitta;

    protected override void EseguiInterazione()
    {
        // Non si può premere due volte
        puoInteragire = false;

        Debug.Log("Il gatto si mette a dormire... Controllo i task!");

        if (TaskManager.Instance != null)
        {
            // Il letto chiede al TaskManager: "Ha fatto tutto?"
            if (TaskManager.Instance.ControllaSeTuttoFinito())
            {
                Debug.Log("VITTORIA! Giornata completata!");

                // Se vuoi, puoi chiamare direttamente il GameManager qui:
                if (GameManager.Instance != null) GameManager.Instance.VittoriaGioco();

                // Chiama anche gli eventi se vuoi accendere UI ecc.
                azioniVittoria?.Invoke();
            }
            else
            {
                Debug.Log("SCONFITTA! Manca qualcosa!");

                if (GameManager.Instance != null)
                {
                    // Usa il tuo metodo GameOver personalizzato
                    GameManager.Instance.GameOver("Sei andato a letto senza finire i tuoi doveri da gatto!");
                }

                azioniSconfitta?.Invoke();
            }
        }
    }
}