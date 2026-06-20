using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TasksUI : MonoBehaviour
{
    public static TasksUI Instance;

    public TextMeshProUGUI testoContatore;
    public GameObject pannelloLista;
    public TextMeshProUGUI testoListaCompleta;

    public KeyCode tastoPerAprireLista = KeyCode.Tab;

    private List<TaskType> taskDaFare = new List<TaskType>();
    private List<TaskType> taskCompletati = new List<TaskType>();

    private bool introInCorso = true;
    private CanvasGroup gruppoCanvasLista;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (pannelloLista != null)
        {
            gruppoCanvasLista = pannelloLista.GetComponent<CanvasGroup>();
            if (gruppoCanvasLista == null)
            {
                gruppoCanvasLista = pannelloLista.AddComponent<CanvasGroup>();
            }
        }

        TrovaTuttiITask();
        AggiornaGrafica();

        StartCoroutine(IntroListaTask());
    }

    void Update()
    {
        if (Time.timeScale == 0f || introInCorso) return;

        if (Input.GetKeyDown(tastoPerAprireLista))
        {
            if (pannelloLista != null)
            {
                pannelloLista.SetActive(!pannelloLista.activeSelf);
            }
        }
    }

    private IEnumerator IntroListaTask()
    {
        introInCorso = true;

        if (pannelloLista != null && gruppoCanvasLista != null)
        {
            pannelloLista.SetActive(true);
            gruppoCanvasLista.alpha = 1f;

            yield return new WaitForSeconds(2f);

            float durataSfumo = 2.5f;
            float timerSfumo = 0f;

            while (timerSfumo < durataSfumo)
            {
                timerSfumo += Time.deltaTime;
                gruppoCanvasLista.alpha = Mathf.Lerp(1f, 0f, timerSfumo / durataSfumo);
                yield return null;
            }

            pannelloLista.SetActive(false);
            gruppoCanvasLista.alpha = 1f;
        }

        introInCorso = false;
    }

    private void TrovaTuttiITask()
    {
        InteractableTask[] tuttiI_Task = Object.FindObjectsByType<InteractableTask>(FindObjectsSortMode.None);

        foreach (InteractableTask task in tuttiI_Task)
        {
            // --- MODIFICA: ESCLUDIAMO VAI A DORMIRE DALLA LISTA INIZIALE ---
            if (task.tipoDiTask != TaskType.Nessuno &&
                task.tipoDiTask != TaskType.VaiADormire &&
                !taskDaFare.Contains(task.tipoDiTask))
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
        string testoFinale = "";
        int taskMancanti = taskDaFare.Count - taskCompletati.Count;

        // Costruiamo la lista testuale nascondendo quelli fatti
        if (testoListaCompleta != null)
        {
            foreach (TaskType tipo in taskDaFare)
            {
                if (!taskCompletati.Contains(tipo))
                {
                    string nomeCarino = FormattaNomeTask(tipo);
                    testoFinale += $"- {nomeCarino}\n";
                }
            }

            // --- LA MAGIA FINALE ---
            // Se la lista è vuota (hai finito i 10 task), facciamo apparire la missione finale!
            if (testoFinale == "")
            {
                testoFinale = "- Go to sleep\n";
                taskMancanti = 1; // Forziamo il contatore a 1 per logica visiva
            }

            testoListaCompleta.text = testoFinale;
        }

        // Aggiorniamo il numero totale in alto
        if (testoContatore != null)
        {
            testoContatore.text = $"Tasks to do: {taskMancanti}";
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