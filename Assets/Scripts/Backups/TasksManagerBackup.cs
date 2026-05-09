/*
using UnityEngine;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    [Header("Impostazioni Generali")]
    public int numeroTaskDaAssegnare = 6; // Ora sono 6 per livello

    [Header("Pool di Difficoltà (Compila su Unity)")]
    public List<TaskType> poolFacili;
    public List<TaskType> poolMedi;
    public List<TaskType> poolDifficili;

    // Queste liste sono nascoste, se le gestisce lui
    [HideInInspector] public List<TaskType> taskAssegnati = new List<TaskType>();
    [HideInInspector] public List<TaskType> taskCompletati = new List<TaskType>();

    void Awake()
    {
        // Questo NON è immortale. Ad ogni reload della scena ne nasce uno nuovo, ed è giusto così!
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        AssegnaTaskInBaseAlLivello();
        PreparaGliOggettiNellaScena();
    }

    private void AssegnaTaskInBaseAlLivello()
    {
        int livello = GameManager.Instance.livelloCorrente;
        List<TaskType> poolAttuale = new List<TaskType>();

        // Sceglie da quale "sacco" pescare
        if (livello == 1) poolAttuale = new List<TaskType>(poolFacili);
        else if (livello == 2) poolAttuale = new List<TaskType>(poolMedi);
        else if (livello == 3) poolAttuale = new List<TaskType>(poolDifficili);

        // Pesca 6 task casuali
        for (int i = 0; i < numeroTaskDaAssegnare; i++)
        {
            if (poolAttuale.Count == 0) break;

            int indiceCasuale = Random.Range(0, poolAttuale.Count);
            taskAssegnati.Add(poolAttuale[indiceCasuale]);
            poolAttuale.RemoveAt(indiceCasuale);
        }

        Debug.Log($"[Livello {livello}] Task segreti assegnati: " + string.Join(", ", taskAssegnati));
    }

    private void PreparaGliOggettiNellaScena()
    {
        // Trova TUTTI i task sparsi per la casa
        InteractableTask[] tuttiGliOggettiTask = Object.FindObjectsByType<InteractableTask>(FindObjectsSortMode.None);

        foreach (InteractableTask oggetto in tuttiGliOggettiTask)
        {
            // Se questo oggetto NON è nei 6 scelti, lo trasformiamo in semplice arredamento
            if (!taskAssegnati.Contains(oggetto.tipoDiTask))
            {
                // Spegne il collider (così il gatto non lo urta e non legge "Premi E")
                Collider2D col = oggetto.GetComponent<Collider2D>();
                if (col != null) col.enabled = false;

                // Spegne lo script del task
                oggetto.enabled = false;
            }
        }
    }

    public void SegnalaTaskCompletato(TaskType taskSvolto)
    {
        if (taskAssegnati.Contains(taskSvolto) && !taskCompletati.Contains(taskSvolto))
        {
            taskCompletati.Add(taskSvolto);
            Debug.Log($"Task segreto completato: {taskSvolto} ({taskCompletati.Count}/{numeroTaskDaAssegnare})");

            ControllaVittoria();
        }
    }

    private void ControllaVittoria()
    {
        if (taskCompletati.Count >= numeroTaskDaAssegnare)
        {
            GameManager.Instance.VittoriaLivello();
        }
    }
}
*/