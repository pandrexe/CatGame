using UnityEngine;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    private int numeroTotaleTask = 0;
    private int taskCompletati = 0;

    // Serve solo per evitare di contare due volte lo stesso task
    private List<TaskType> tipologieCompletate = new List<TaskType>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // Trova quanti task interattivi ci sono attivi nella scena
        InteractableTask[] tuttiI_Task = Object.FindObjectsByType<InteractableTask>(FindObjectsSortMode.None);
        numeroTotaleTask = tuttiI_Task.Length;

        Debug.Log($"[TaskManager] Trovati {numeroTotaleTask} task da completare in questa run.");
    }

    public void SegnalaTaskCompletato(TaskType taskSvolto)
    {
        if (!tipologieCompletate.Contains(taskSvolto))
        {
            tipologieCompletate.Add(taskSvolto);
            taskCompletati++;

            Debug.Log($"Task completato: {taskSvolto} ({taskCompletati}/{numeroTotaleTask})");

            if (taskCompletati >= numeroTotaleTask)
            {
                GameManager.Instance.VittoriaGioco();
            }
        }
    }
}