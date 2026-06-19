using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TasksUI : MonoBehaviour
{
    public static TasksUI Instance;

    public TextMeshProUGUI testoContatore; 
    public GameObject pannelloLista; 
    public TextMeshProUGUI testoListaCompleta; 

    public KeyCode tastoPerAprireLista = KeyCode.Return;

    private List<TaskType> taskDaFare = new List<TaskType>();
    private List<TaskType> taskCompletati = new List<TaskType>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (pannelloLista != null) pannelloLista.SetActive(false); 
        
        TrovaTuttiITask();
        AggiornaGrafica();
    }

    void Update()
    {
        if (Input.GetKeyDown(tastoPerAprireLista))
        {
            if (pannelloLista != null)
            {
                pannelloLista.SetActive(!pannelloLista.activeSelf);
            }
        }
    }

    private void TrovaTuttiITask()
    {
        InteractableTask[] tuttiI_Task = Object.FindObjectsByType<InteractableTask>(FindObjectsSortMode.None);

        foreach (InteractableTask task in tuttiI_Task)
        {
            if (task.tipoDiTask != TaskType.Nessuno && !taskDaFare.Contains(task.tipoDiTask))
            {
                taskDaFare.Add(task.tipoDiTask);
            }
        }
    }

    public void SegnalaCompletamentoUI(TaskType taskSvolto)
    {
        if (taskDaFare.Contains(taskSvolto) && !taskCompletati.Contains(taskSvolto))
        {
            taskCompletati.Add(taskSvolto);
            AggiornaGrafica();
        }
    }

    private void AggiornaGrafica()
    {
        // 1. Aggiorniamo il numero totale in alto
        if (testoContatore != null)
        {
            int taskMancanti = taskDaFare.Count - taskCompletati.Count;
            testoContatore.text = $"Tasks to do: {taskMancanti}";
        }

        // 2. Costruiamo la lista testuale nascondendo quelli fatti
        if (testoListaCompleta != null)
        {
            string testoFinale = "";

            foreach (TaskType tipo in taskDaFare)
            {
                // Se il task NON è completato, lo aggiungiamo alla scritta a schermo
                if (!taskCompletati.Contains(tipo))
                {
                    string nomeCarino = FormattaNomeTask(tipo);
                    testoFinale += $"- {nomeCarino}\n";
                }
            }

            // Chicca: se la lista è vuota (tutto completato), mostriamo un messaggio di vittoria
            if (testoFinale == "")
            {
                testoFinale = "All tasks completed!";
            }

            testoListaCompleta.text = testoFinale;
        }
    }

    private string FormattaNomeTask(TaskType tipo)
    {
        string nomeEnum = tipo.ToString();

        switch (nomeEnum)
        {
            case "InstallaVirus":
            case "InstallVirus": 
                return "Install the virus on the PC";

            case "RovinaColazione":
            case "RuinBreakfast":
            case "SpoilBreakfast":
                return "Ruin the breakfast";

            case "SrotolaCartaIgienica":
            case "UnrollToiletPaper":
                return "Unroll the toilet paper";

            case "GraffiaDivano":
            case "ScratchSofa":
            case "ScratchCouch":
                return "Scratch the sofa";

            case "AbbattiBicchiere":
            case "KnockOverGlass":
            case "BreakGlass":
                return "Knock over the glass";

            case "SuonaPianoforte":
            case "PlayPiano":
                return "Play the piano scale";

            case "FaiCanestro":
            case "MakeBasket":
                return "Make a basket";

            case "ButtaSpazzatura":
            case "DumpTrash":
            case "TrashTask":
                return "Knock over the trash bin";

            case "TogliRuota":
            case "RemoveWheel":
            case "WheelTask":
                return "Remove the car bolts";

            case "RubaCibo":
            case "StealFood":
            case "SardineTask":
                return "Steal the sardine";

            case "VaiADormire":
            case "GoToSleep":
            case "Sleep":
                return "Go to sleep";

            default: 
                return nomeEnum;
        }
    }
}