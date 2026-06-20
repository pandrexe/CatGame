using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class BedInteraction : Interactable
{
    [Header("Animazione")]
    [Tooltip("Trascina qui l'oggetto del Gatto che contiene l'Animator")]
    public Animator gattoAnimator;
    [Tooltip("Quanti secondi deve durare l'animazione prima di finire il gioco?")]
    public float tempoDiAttesaAnimazione = 2f;

    [Header("Eventi Fine Gioco")]
    [Tooltip("Cosa succede se il gatto dorme e TUTTI i task sono finiti?")]
    public UnityEvent azioniVittoria;

    [Tooltip("Cosa succede se il gatto dorme ma MANCANO dei task?")]
    public UnityEvent azioniSconfitta;

    protected override void EseguiInterazione()
    {
        puoInteragire = false;

        // --- 1. FERMIAMO SUBITO IL TIMER! ---
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.FermaTimer();
        }

        Debug.Log("Il gatto si mette a dormire...");

        // --- 2. FACCIAMO PARTIRE L'ANIMAZIONE ---
        if (gattoAnimator != null)
        {
            gattoAnimator.SetBool("isSleeping", true);
        }

        // --- 3. ASPETTIAMO PRIMA DI DARE IL RISULTATO ---
        StartCoroutine(AspettaEControllaVittoria());
    }

    private IEnumerator AspettaEControllaVittoria()
    {
        // Aspetta che il gatto dorma un po'
        yield return new WaitForSeconds(tempoDiAttesaAnimazione);

        // Ora facciamo i controlli
        Debug.Log("Controllo i task!");

        if (TaskManager.Instance != null)
        {
            if (TaskManager.Instance.ControllaSeTuttoFinito())
            {
                Debug.Log("VITTORIA! Giornata completata!");

                if (GameManager.Instance != null) GameManager.Instance.VittoriaGioco();

                azioniVittoria?.Invoke();
            }
            else
            {
                Debug.Log("SCONFITTA! Manca qualcosa!");

                azioniSconfitta?.Invoke();

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.GameOver("Sei andato a letto senza finire i tuoi doveri da gatto!");
                }
            }
        }
    }
}