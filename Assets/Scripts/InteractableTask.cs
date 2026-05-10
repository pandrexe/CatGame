using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Events; 

public class InteractableTask : Interactable
{
    [Header("Identità Task")]
    public TaskType tipoDiTask; // Assicurati di avere il tuo enum TaskType!

    [Header("Impostazioni Task")]
    public CinemachineCamera telecameraDelMinigioco;

    [Header("Impostazioni Cursore")]
    public TipoCursore cursoreRichiesto = TipoCursore.Singola;

    [Header("Conseguenze Vittoria")]
    public UnityEvent azioniAllaVittoria;

    protected override void EseguiInterazione()
    {
        puoInteragire = false; 
        GameManager.Instance.IniziaMinigioco(telecameraDelMinigioco, this);
        if (MinigameCursor.Instance != null)
        {
            MinigameCursor.Instance.ImpostaCursore(cursoreRichiesto);
        }
    }

    public void CompletaTask()
    {
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.SegnalaTaskCompletato(tipoDiTask);
        }
        azioniAllaVittoria?.Invoke();
    }
}