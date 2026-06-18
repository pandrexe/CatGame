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
        // 1. Troviamo TUTTI i task sparsi per la casa
        InteractableTask[] tuttiI_Task = Object.FindObjectsByType<InteractableTask>(FindObjectsSortMode.None);

        // 2. Azzeriamo il contatore prima del ciclo di filtro
        numeroTotaleTask = 0;

        // 3. --- IL FILTRO INTELLIGENTE ---
        // Controlliamo i task uno per uno: se sono veri task li contiamo, se sono "Nessuno" li ignoriamo
        foreach (InteractableTask task in tuttiI_Task)
        {
            if (task.tipoDiTask != TaskType.Nessuno)
            {
                numeroTotaleTask++; // Lo aggiungiamo al totale della run solo se è valido!

                // --- RIGA AGGIUNTA PER TROVARE L'INTRUSO ---
                Debug.Log($"Task #{numeroTotaleTask} contato: '{task.gameObject.name}' (Tipo: {task.tipoDiTask})");
            }
            else
            {
                Debug.Log($"[TaskManager] Ignorato l'oggetto '{task.gameObject.name}' perché impostato su TaskType.Nessuno");
            }
        }

        Debug.Log($"[TaskManager] Trovati {numeroTotaleTask} task REALI da completare in questa run (Esclusi i 'Nessuno').");
    }

    public void SegnalaTaskCompletato(TaskType taskSvolto)
    {
        // Un ulteriore blocco di sicurezza: se per qualche motivo si completa un task "Nessuno", lo ignoriamo
        if (taskSvolto == TaskType.Nessuno) return;

        if (!tipologieCompletate.Contains(taskSvolto))
        {
            tipologieCompletate.Add(taskSvolto);
            taskCompletati++;

            Debug.Log($"Task completato: {taskSvolto} ({taskCompletati}/{numeroTotaleTask})");

            // Ricordati che abbiamo tolto la vittoria automatica da qui!
            // Ora si vince solo convalidando la run dal Letto (BedInteraction)
        }
    }

    // Questa funzione verrà usata dal tuo BedInteraction per capire se la run è valida
    public bool ControllaSeTuttoFinito()
    {
        return taskCompletati >= numeroTotaleTask;
    }
}