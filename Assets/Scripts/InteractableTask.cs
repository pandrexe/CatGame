using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Events; 

public class InteractableTask : Interactable
{
    [Header("Identità Task")]
    public TaskType tipoDiTask; 

    [Header("Impostazioni Task")]
    public CinemachineCamera telecameraDelMinigioco;

    [Header("Logica del Minigioco")]
    public MonoBehaviour scriptDelTask;

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

        // ACCENDIAMO LO SCRIPT DEL MINIGIOCO SOLO ADESSO!
        if (scriptDelTask != null)
        {
            scriptDelTask.enabled = true;
        }
    }

    public void CompletaTask()
    {
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.SegnalaTaskCompletato(tipoDiTask);
        }

        if (scriptDelTask != null)
        {
            scriptDelTask.enabled = false;
        }

        azioniAllaVittoria?.Invoke();
    }
}