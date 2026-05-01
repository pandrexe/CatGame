using UnityEngine;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    [Header("Impostazioni")]
    public int numeroTaskDaAssegnare = 5;

    public List<TaskType> taskAssegnati = new List<TaskType>();

    public List<TaskType> taskCompletati = new List<TaskType>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        AssegnaTaskCasuali();
    }

    private void AssegnaTaskCasuali()
    {
        List<TaskType> tuttiITask = new List<TaskType>();
        foreach (TaskType task in System.Enum.GetValues(typeof(TaskType)))
        {
            if (task != TaskType.Nessuno)
            {
                tuttiITask.Add(task);
            }
        }

        for (int i = 0; i < numeroTaskDaAssegnare; i++)
        {
            if (tuttiITask.Count == 0) break;

            int indiceCasuale = Random.Range(0, tuttiITask.Count);
            TaskType taskScelto = tuttiITask[indiceCasuale];

            taskAssegnati.Add(taskScelto);
            tuttiITask.RemoveAt(indiceCasuale); // Lo rimuoviamo per non pescarlo due volte
        }

        Debug.Log("Task assegnati per questa partita: " + string.Join(", ", taskAssegnati));
    }

    public void SegnalaTaskCompletato(TaskType taskSvolto)
    {
        if (taskAssegnati.Contains(taskSvolto))
        {
            if (!taskCompletati.Contains(taskSvolto))
            {
                taskCompletati.Add(taskSvolto);
                Debug.Log($"Grande! Hai completato il task richiesto: {taskSvolto}");

                ControllaVittoria();
            }
        }
        else
        {
            Debug.Log($"Hai fatto {taskSvolto}, ma non era sulla lista. Nessun punto, ma bel caos!");
        }
    }

    private void ControllaVittoria()
    {
        if (taskCompletati.Count >= numeroTaskDaAssegnare)
        {
            Debug.Log("HAI VINTO! Tutti i task completati in tempo!");
        }
    }
}