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

    private float tempoInizioTask;

    // --- IL LUCCHETTO ANTIDOPPIONE ---
    private bool taskGiaCompletato = false;

    protected override void EseguiInterazione()
    {
        puoInteragire = false;

        // Resettiamo il lucchetto ogni volta che si inizia il minigioco
        taskGiaCompletato = false;

        GameManager.Instance.IniziaMinigioco(telecameraDelMinigioco, this);

        if (MinigameCursor.Instance != null)
        {
            MinigameCursor.Instance.ImpostaCursore(cursoreRichiesto);
        }

        if (scriptDelTask != null)
        {
            scriptDelTask.enabled = true;
        }

        // SNAPSHOT A
        tempoInizioTask = Time.time;
    }

    public void CompletaTask()
    {
        // Se è già stato completato un millisecondo fa, ignoriamo questa chiamata!
        if (taskGiaCompletato) return;

        taskGiaCompletato = true; // Chiudiamo il lucchetto

        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.SegnalaTaskCompletato(tipoDiTask);
        }

        if (scriptDelTask != null)
        {
            scriptDelTask.enabled = false;
        }

        azioniAllaVittoria?.Invoke();

        // --- CALCOLO DEL BONUS (SNAPSHOT B) ---
        if (tipoDiTask != TaskType.Nessuno && GameTimer.Instance != null)
        {
            float durataTask = Time.time - tempoInizioTask;
            AssegnaBonusTempo(durataTask);
        }
    }

    private void AssegnaBonusTempo(float durata)
    {
        float secondiBonus = 0f;

        if (durata <= 3f)
        {
            secondiBonus = 5f;
            Debug.Log($"[Bonus] Task Perfetto! Fatto in {durata:F1}s -> +5 secondi!");
        }
        else if (durata <= 6f)
        {
            secondiBonus = 3f;
            Debug.Log($"[Bonus] Task Veloce! Fatto in {durata:F1}s -> +3 secondi!");
        }
        else if (durata <= 10f)
        {
            secondiBonus = 1f;
            Debug.Log($"[Bonus] Task Normale. Fatto in {durata:F1}s -> +1 secondo!");
        }
        else
        {
            Debug.Log($"[Bonus] Task Lento. Fatto in {durata:F1}s -> Nessun bonus.");
        }

        if (secondiBonus > 0f)
        {
            GameTimer.Instance.AggiungiTempo(secondiBonus);
        }
    }
}